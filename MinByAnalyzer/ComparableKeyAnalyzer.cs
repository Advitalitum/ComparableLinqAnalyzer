using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MinByAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ComparableKeyAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "MBA0001";

    private const string LinqNamespace = "System.Linq";
    private const string MinByMethodName = "MinBy";
    private const string MaxByMethodName = "MaxBy";
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

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
        DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

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

        if (!IsLinqMinOrMaxBy(methodSymbol))
            return;

        if (methodSymbol.TypeArguments.Length < 2)
            return;

        if (methodSymbol.Parameters.Length >= 3)
            return;

        ITypeSymbol keyType = methodSymbol.TypeArguments[1];

        if (keyType is ITypeParameterSymbol or IErrorTypeSymbol)
            return;

        keyType = UnwrapNullable(keyType);

        if (IsComparable(keyType, context.Compilation))
            return;

        var diagnostic = Diagnostic.Create(Rule,
            invocationOperation.Syntax.GetLocation(),
            keyType.ToDisplayString(),
            methodSymbol.Name);

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsLinqMinOrMaxBy(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.Name != MinByMethodName && methodSymbol.Name != MaxByMethodName)
            return false;

        if (methodSymbol.ContainingType?.ContainingNamespace is not INamespaceSymbol namespaceSymbol)
            return false;

        return namespaceSymbol.ToDisplayString() == LinqNamespace;
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
