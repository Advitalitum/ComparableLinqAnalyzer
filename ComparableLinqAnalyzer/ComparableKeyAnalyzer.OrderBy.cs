using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string OrderByMethodName = "OrderBy";

    public const string OrderByDiagnosticId = "MBA0003";

    private static readonly DiagnosticDescriptor OrderByRule = new(
        OrderByDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetOrderByTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType,
        out DiagnosticDescriptor rule)
    {
        rule = OrderByRule;
        targetType = null!;

        if (methodSymbol.Name != OrderByMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, out targetType);
    }
}
