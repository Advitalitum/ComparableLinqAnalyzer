using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string OrderDescendingMethodName = "OrderDescending";

    private static bool TryGetOrderDescendingTarget(IMethodSymbol methodSymbol, bool nullComparer,
        out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Name != OrderDescendingMethodName)
            return false;

        return TryGetElementTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
