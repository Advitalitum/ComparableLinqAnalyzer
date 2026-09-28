using Microsoft.CodeAnalysis;

namespace MinByAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string OrderDescendingMethodName = "OrderDescending";

    public const string OrderDescendingDiagnosticId = "MBA0006";

    private static readonly DiagnosticDescriptor OrderDescendingRule = new(
        OrderDescendingDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetOrderDescendingTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType,
        out DiagnosticDescriptor rule)
    {
        rule = OrderDescendingRule;
        targetType = null!;

        if (methodSymbol.Name != OrderDescendingMethodName)
            return false;

        return TryGetElementTarget(methodSymbol, out targetType);
    }
}
