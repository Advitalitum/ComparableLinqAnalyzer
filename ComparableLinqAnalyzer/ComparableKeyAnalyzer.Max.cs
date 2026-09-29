using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string MaxMethodName = "Max";

    private static bool TryGetMaxTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType)
    {
        targetType = null!;

        if (methodSymbol.Name != MaxMethodName)
            return false;

        return TryGetMinMaxTarget(methodSymbol: methodSymbol, out targetType);
    }
}
