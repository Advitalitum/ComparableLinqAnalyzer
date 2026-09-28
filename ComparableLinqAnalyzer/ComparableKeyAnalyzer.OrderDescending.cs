using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string OrderDescendingMethodName = "OrderDescending";

    public const string OrderDescendingDiagnosticId = "CLA0006";

    private static readonly DiagnosticDescriptor OrderDescendingRule = new(
        OrderDescendingDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetOrderDescendingTarget(IMethodSymbol methodSymbol, bool nullComparer,
        out ITypeSymbol targetType, out DiagnosticDescriptor rule)
    {
        rule = OrderDescendingRule;
        targetType = null!;

        if (methodSymbol.Name != OrderDescendingMethodName)
            return false;

        return TryGetElementTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
