using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string MinByMethodName = "MinBy";

    public const string MinByDiagnosticId = "MBA0001";

    private static readonly DiagnosticDescriptor MinByRule = new(
        MinByDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetMinByTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType,
        out DiagnosticDescriptor rule)
    {
        rule = MinByRule;
        targetType = null!;

        if (methodSymbol.Name != MinByMethodName)
            return false;

        return TryGetKeySelectorTarget(methodSymbol: methodSymbol, out targetType);
    }
}
