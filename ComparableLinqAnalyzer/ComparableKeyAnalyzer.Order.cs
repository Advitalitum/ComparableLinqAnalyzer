using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string OrderMethodName = "Order";

    private static bool TryGetOrderTarget(IMethodSymbol methodSymbol, bool nullComparer, out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Name != OrderMethodName)
            return false;

        return TryGetElementTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
