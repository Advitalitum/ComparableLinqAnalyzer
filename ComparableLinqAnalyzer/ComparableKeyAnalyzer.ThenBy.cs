using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string ThenByMethodName = "ThenBy";

    private static bool TryGetThenByTarget(IMethodSymbol methodSymbol, bool nullComparer, out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Name != ThenByMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
