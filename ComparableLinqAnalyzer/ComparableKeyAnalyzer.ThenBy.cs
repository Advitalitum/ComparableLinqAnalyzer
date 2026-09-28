using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string ThenByMethodName = "ThenBy";

    public const string ThenByDiagnosticId = "CLA0009";

    private static readonly DiagnosticDescriptor ThenByRule = new(
        ThenByDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetThenByTarget(IMethodSymbol methodSymbol, bool nullComparer, out ITypeSymbol targetType,
        out DiagnosticDescriptor rule)
    {
        rule = ThenByRule;
        targetType = null!;

        if (methodSymbol.Name != ThenByMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
