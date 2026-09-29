using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string ThenByDescendingMethodName = "ThenByDescending";

    private static bool TryGetThenByDescendingTarget(IMethodSymbol methodSymbol, bool nullComparer,
        out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Name != ThenByDescendingMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
