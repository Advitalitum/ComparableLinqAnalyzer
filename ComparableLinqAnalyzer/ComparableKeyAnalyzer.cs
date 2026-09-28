using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ComparableLinqAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public partial class ComparableKeyAnalyzer : DiagnosticAnalyzer
{
    private const string LinqNamespace = "System.Linq";
    private const string GenericComparableMetadataName = "System.IComparable`1";
    private const string NonGenericComparableMetadataName = "System.IComparable";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AB0003Title),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.AB0003MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.AB0003Description), Resources.ResourceManager,
            typeof(Resources));

    private const string Category = "Usage";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            MinByRule,
            MaxByRule,
            OrderByRule,
            OrderByDescendingRule,
            OrderRule,
            OrderDescendingRule,
            MinRule,
            MaxRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext context)
    {
        if (context.Operation is not IInvocationOperation invocationOperation)
            return;

        IMethodSymbol methodSymbol = invocationOperation.TargetMethod;

        ITypeSymbol targetType;
        DiagnosticDescriptor rule;

        if (!TryGetComparableTarget(methodSymbol: methodSymbol, out targetType, out rule))
            return;

        if (targetType is ITypeParameterSymbol or IErrorTypeSymbol)
            return;

        targetType = UnwrapNullable(targetType);

        if (IsComparable(type: targetType, compilation: context.Compilation))
            return;

        var diagnostic = Diagnostic.Create(rule,
            invocationOperation.Syntax.GetLocation(),
            targetType.ToDisplayString(),
            methodSymbol.Name);

        context.ReportDiagnostic(diagnostic);
    }

    private static bool TryGetComparableTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType,
        out DiagnosticDescriptor rule)
    {
        targetType = null!;
        rule = null!;

        if (!IsLinqMethod(methodSymbol))
            return false;

        return TryGetMinByTarget(methodSymbol: methodSymbol, out targetType, out rule)
            || TryGetMaxByTarget(methodSymbol: methodSymbol, out targetType, out rule)
            || TryGetOrderByTarget(methodSymbol: methodSymbol, out targetType, out rule)
            || TryGetOrderByDescendingTarget(methodSymbol: methodSymbol, out targetType, out rule)
            || TryGetOrderTarget(methodSymbol: methodSymbol, out targetType, out rule)
            || TryGetOrderDescendingTarget(methodSymbol: methodSymbol, out targetType, out rule)
            || TryGetMinTarget(methodSymbol: methodSymbol, out targetType, out rule)
            || TryGetMaxTarget(methodSymbol: methodSymbol, out targetType, out rule);
    }

    private static bool IsLinqMethod(IMethodSymbol methodSymbol)
    {
        if (!methodSymbol.IsExtensionMethod)
            return false;

        if (methodSymbol.ContainingType?.ContainingNamespace is not INamespaceSymbol namespaceSymbol)
            return false;

        return namespaceSymbol.ToDisplayString() == LinqNamespace;
    }

    private static bool TryGetKeySelectorTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Parameters.Length >= 3)
            return false;

        if (methodSymbol.TypeArguments.Length < 2)
            return false;

        targetType = methodSymbol.TypeArguments[1];

        return true;
    }

    private static bool TryGetElementTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Parameters.Length >= 2)
            return false;

        if (methodSymbol.TypeArguments.Length < 1)
            return false;

        targetType = methodSymbol.TypeArguments[0];

        return true;
    }

    private static bool TryGetMinMaxTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.TypeArguments.Length != 1)
            return false;

        if (methodSymbol.Parameters.Any(IsFuncType))
            return false;

        targetType = methodSymbol.TypeArguments[0];

        return true;
    }

    private static bool IsFuncType(IParameterSymbol parameter)
    {
        return parameter.Type.TypeKind == TypeKind.Delegate
            && parameter.Type.MetadataName.StartsWith("Func`", StringComparison.Ordinal);
    }

    private static bool IsComparerType(IParameterSymbol parameter)
    {
        return parameter.Type.MetadataName == "IComparer`1";
    }

    private static ITypeSymbol UnwrapNullable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable)
            return typeSymbol;

        return nullable.TypeArguments[0];
    }

    private static bool IsComparable(ITypeSymbol type, Compilation compilation)
    {
        ITypeSymbol? nonGenericComparable = compilation.GetTypeByMetadataName(NonGenericComparableMetadataName);
        ITypeSymbol? genericComparableOpen = compilation.GetTypeByMetadataName(GenericComparableMetadataName);

        foreach (INamedTypeSymbol interfaceSymbol in type.AllInterfaces)
        {
            if (nonGenericComparable is not null &&
                SymbolEqualityComparer.Default.Equals(interfaceSymbol, nonGenericComparable))
            {
                return true;
            }

            if (genericComparableOpen is not null &&
                interfaceSymbol.OriginalDefinition.Equals(genericComparableOpen, SymbolEqualityComparer.Default) &&
                interfaceSymbol.TypeArguments.Length == 1 &&
                SymbolEqualityComparer.Default.Equals(interfaceSymbol.TypeArguments[0], type))
            {
                return true;
            }
        }

        return false;
    }
}
