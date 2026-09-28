using Microsoft.CodeAnalysis;

namespace MinByAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string MinMethodName = "Min";

    public const string MinDiagnosticId = "MBA0007";

    private static readonly DiagnosticDescriptor MinRule = new(
        MinDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetMinTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType,
        out DiagnosticDescriptor rule)
    {
        rule = MinRule;
        targetType = null!;

        if (methodSymbol.Name != MinMethodName)
            return false;

        return TryGetMinMaxTarget(methodSymbol, out targetType);
    }
}
