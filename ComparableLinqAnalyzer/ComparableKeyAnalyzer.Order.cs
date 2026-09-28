using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string OrderMethodName = "Order";

    public const string OrderDiagnosticId = "MBA0005";

    private static readonly DiagnosticDescriptor OrderRule = new(
        OrderDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetOrderTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType,
        out DiagnosticDescriptor rule)
    {
        rule = OrderRule;
        targetType = null!;

        if (methodSymbol.Name != OrderMethodName)
            return false;

        return TryGetElementTarget(methodSymbol, out targetType);
    }
}
