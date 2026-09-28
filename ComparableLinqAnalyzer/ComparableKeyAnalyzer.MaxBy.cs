using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string MaxByMethodName = "MaxBy";

    public const string MaxByDiagnosticId = "MBA0002";

    private static readonly DiagnosticDescriptor MaxByRule = new(
        MaxByDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetMaxByTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType,
        out DiagnosticDescriptor rule)
    {
        rule = MaxByRule;
        targetType = null!;

        if (methodSymbol.Name != MaxByMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, out targetType);
    }
}
