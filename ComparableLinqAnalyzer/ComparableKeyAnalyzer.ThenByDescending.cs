using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string ThenByDescendingMethodName = "ThenByDescending";

    public const string ThenByDescendingDiagnosticId = "CLA0010";

    private static readonly DiagnosticDescriptor ThenByDescendingRule = new(
        ThenByDescendingDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetThenByDescendingTarget(IMethodSymbol methodSymbol, bool nullComparer,
        out ITypeSymbol targetType, out DiagnosticDescriptor rule)
    {
        rule = ThenByDescendingRule;
        targetType = null!;

        if (methodSymbol.Name != ThenByDescendingMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, nullComparer: nullComparer, out targetType);
    }
}
