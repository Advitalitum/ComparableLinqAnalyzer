using Microsoft.CodeAnalysis;

namespace MinByAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string OrderByDescendingMethodName = "OrderByDescending";

    public const string OrderByDescendingDiagnosticId = "MBA0004";

    private static readonly DiagnosticDescriptor OrderByDescendingRule = new(
        OrderByDescendingDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetOrderByDescendingTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType,
        out DiagnosticDescriptor rule)
    {
        rule = OrderByDescendingRule;
        targetType = null!;

        if (methodSymbol.Name != OrderByDescendingMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol, out targetType);
    }
}
