using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ComparableLinqAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ComparableKeyAnalyzer : DiagnosticAnalyzer
{
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AB0003Title),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.AB0003MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.AB0003Description), Resources.ResourceManager,
            typeof(Resources));

    private const string Category = "Usage";

    private const string DiagnosticId = "CLA0001";

    private static readonly DiagnosticDescriptor ComparableRule = new(
        DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private const string LinqNamespace = "System.Linq";
    private const string GenericComparableMetadataName = "System.IComparable`1";
    private const string NonGenericComparableMetadataName = "System.IComparable";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(ComparableRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
        if (context.Operation is not IInvocationOperation invocationOperation)
        {
            return;
        }

        IMethodSymbol methodSymbol = invocationOperation.TargetMethod;

        ITypeSymbol? targetType = GetComparableTarget(methodSymbol: methodSymbol, invocationOperation: invocationOperation);

        if (targetType is null or ITypeParameterSymbol or IErrorTypeSymbol)
        {
            return;
        }

        targetType = UnwrapNullable(targetType);

        if (IsComparable(type: targetType, compilation: context.Compilation))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            descriptor: ComparableRule,
            location: invocationOperation.Syntax.GetLocation(),
            messageArgs: [targetType.ToDisplayString(), methodSymbol.Name]);

        context.ReportDiagnostic(diagnostic);
    }

    private static ITypeSymbol? GetComparableTarget(IMethodSymbol methodSymbol, IInvocationOperation invocationOperation)
    {
        if (!IsLinqMethod(methodSymbol))
        {
            return null;
        }

        ITypeSymbol? result = methodSymbol.Name switch
        {
            "MinBy" or "MaxBy" or "OrderBy" or "OrderByDescending" or "ThenBy" or "ThenByDescending"
                => GetKeySelectorTarget(methodSymbol: methodSymbol, invocationOperation: invocationOperation),
            "Order" or "OrderDescending"
                => GetElementTarget(methodSymbol: methodSymbol, invocationOperation: invocationOperation),
            "Min" or "Max"
                => GetMinMaxTarget(methodSymbol: methodSymbol, invocationOperation: invocationOperation),
            _ => null,
        };

        return result;
    }

    // Determines whether the comparer overload was passed a literal null (or default). In such
    // case comparison is done with Comparer<T>.Default, so the key type must still be comparable.
    private static bool HasNullComparerArgument(IMethodSymbol methodSymbol, IInvocationOperation invocationOperation)
    {
        bool result = methodSymbol.Parameters
            .Select((parameter, index) => (Parameter: parameter, Index: index))
            .Where(item => IsComparerType(item.Parameter))
            .Select(item => invocationOperation.Arguments[item.Index].Value)
            .Any(argumentValue => argumentValue.ConstantValue is { HasValue: true, Value: null });

        return result;
    }

    private static bool IsLinqMethod(IMethodSymbol methodSymbol)
    {
        if (!methodSymbol.IsExtensionMethod)
        {
            return false;
        }

        if (methodSymbol.ContainingType?.ContainingNamespace is not { } namespaceSymbol)
        {
            return false;
        }

        bool result = namespaceSymbol.ToDisplayString() == LinqNamespace;

        return result;
    }

    // The comparer overload has three parameters. With a real comparer comparison is controlled
    // by the caller, so the key does not need to be comparable. But when the comparer is a
    // literal null, Comparer<T>.Default is used at runtime and the key must still be comparable.
    private static ITypeSymbol? GetKeySelectorTarget(IMethodSymbol methodSymbol, IInvocationOperation invocationOperation)
    {
        bool hasNullComparerArgument = HasNullComparerArgument(methodSymbol: methodSymbol, invocationOperation: invocationOperation);

        if (methodSymbol.Parameters.Length >= 3 && !hasNullComparerArgument)
        {
            return null;
        }

        if (methodSymbol.TypeArguments.Length < 2)
        {
            return null;
        }

        ITypeSymbol result = methodSymbol.TypeArguments[1];

        return result;
    }

    // Order/OrderDescending comparer overload has two parameters. As with the key selector
    // above, it is analyzed only when the comparer is a literal null.
    private static ITypeSymbol? GetElementTarget(IMethodSymbol methodSymbol, IInvocationOperation invocationOperation)
    {
        bool hasNullComparerArgument = HasNullComparerArgument(methodSymbol: methodSymbol, invocationOperation: invocationOperation);

        if (methodSymbol.Parameters.Length >= 2 && !hasNullComparerArgument)
        {
            return null;
        }

        if (methodSymbol.TypeArguments.Length < 1)
        {
            return null;
        }

        ITypeSymbol result = methodSymbol.TypeArguments.First();

        return result;
    }

    // Min/Max also have comparer overloads: (source, comparer). A real comparer makes the element
    // valid regardless of comparability, so the element is analyzed only when the comparer is
    // literal null (Comparer<T>.Default) or there is no comparer at all.
    private static ITypeSymbol? GetMinMaxTarget(IMethodSymbol methodSymbol, IInvocationOperation invocationOperation)
    {
        if (methodSymbol.TypeArguments.Length != 1)
        {
            return null;
        }

        if (methodSymbol.Parameters.Any(IsFuncType))
        {
            return null;
        }

        bool hasNullComparerArgument = HasNullComparerArgument(methodSymbol: methodSymbol, invocationOperation: invocationOperation);

        if (methodSymbol.Parameters.Any(IsComparerType) && !hasNullComparerArgument)
        {
            return null;
        }

        ITypeSymbol result = methodSymbol.TypeArguments.Single();

        return result;
    }

    private static bool IsFuncType(IParameterSymbol parameter)
    {
        bool result = parameter.Type.TypeKind == TypeKind.Delegate
            && parameter.Type.MetadataName.StartsWith("Func`", StringComparison.Ordinal);

        return result;
    }

    private static bool IsComparerType(IParameterSymbol parameter)
    {
        bool result = parameter.Type.MetadataName == "IComparer`1";

        return result;
    }

    private static ITypeSymbol UnwrapNullable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable)
        {
            return typeSymbol;
        }

        ITypeSymbol result = nullable.TypeArguments.Single();

        return result;
    }

    private static bool IsComparable(ITypeSymbol type, Compilation compilation)
    {
        ITypeSymbol? nonGenericComparable = compilation.GetTypeByMetadataName(NonGenericComparableMetadataName);
        ITypeSymbol? genericComparableOpen = compilation.GetTypeByMetadataName(GenericComparableMetadataName);

        if (nonGenericComparable is null && genericComparableOpen is null)
        {
            return false;
        }

        bool result = type.AllInterfaces.Any(interfaceSymbol =>
            IsComparableImplementation(
                interfaceSymbol: interfaceSymbol,
                type: type,
                nonGenericComparable: nonGenericComparable,
                genericComparableOpen: genericComparableOpen));

        return result;
    }

    private static bool IsComparableImplementation(
        INamedTypeSymbol interfaceSymbol,
        ITypeSymbol type,
        ITypeSymbol? nonGenericComparable,
        ITypeSymbol? genericComparableOpen)
    {
        bool result = SymbolEqualityComparer.Default.Equals(interfaceSymbol, nonGenericComparable)
            || (genericComparableOpen is not null
                && SymbolEqualityComparer.Default.Equals(interfaceSymbol.OriginalDefinition, genericComparableOpen)
                && interfaceSymbol.TypeArguments.Length == 1
                && SymbolEqualityComparer.Default.Equals(interfaceSymbol.TypeArguments.Single(), type));

        return result;
    }
}
