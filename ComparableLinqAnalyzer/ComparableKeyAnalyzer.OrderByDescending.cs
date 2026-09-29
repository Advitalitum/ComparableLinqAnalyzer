using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string OrderByDescendingMethodName = "OrderByDescending";

    private static bool TryGetOrderByDescendingTarget(IMethodSymbol methodSymbol, bool nullComparer,
        out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Name != OrderByDescendingMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
