using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string OrderByDescendingMethodName = "OrderByDescending";

    public const string OrderByDescendingDiagnosticId = "CLA0004";

    private static readonly DiagnosticDescriptor OrderByDescendingRule = new(
        OrderByDescendingDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetOrderByDescendingTarget(IMethodSymbol methodSymbol, bool nullComparer,
        out ITypeSymbol targetType, out DiagnosticDescriptor rule)
    {
        rule = OrderByDescendingRule;
        targetType = null!;

        if (methodSymbol.Name != OrderByDescendingMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
