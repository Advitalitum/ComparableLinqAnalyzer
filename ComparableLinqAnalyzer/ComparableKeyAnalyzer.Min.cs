using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string MinMethodName = "Min";

    private static bool TryGetMinTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Name != MinMethodName)
            return false;

        return TryGetMinMaxTarget(methodSymbol: methodSymbol, out targetType);
    }
}
