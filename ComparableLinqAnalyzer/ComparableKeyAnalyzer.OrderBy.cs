using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string OrderByMethodName = "OrderBy";

    private static bool TryGetOrderByTarget(IMethodSymbol methodSymbol, bool nullComparer, out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Name != OrderByMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
