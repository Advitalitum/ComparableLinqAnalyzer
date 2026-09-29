using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string MinByMethodName = "MinBy";

    private static bool TryGetMinByTarget(IMethodSymbol methodSymbol, bool nullComparer, out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Name != MinByMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
