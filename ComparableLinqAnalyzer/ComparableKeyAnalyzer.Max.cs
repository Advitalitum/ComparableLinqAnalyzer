using Microsoft.CodeAnalysis;

namespace ComparableLinqAnalyzer;

public partial class ComparableKeyAnalyzer
{
    private const string MaxMethodName = "Max";

    public const string MaxDiagnosticId = "MBA0008";

    private static readonly DiagnosticDescriptor MaxRule = new(
        MaxDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
        isEnabledByDefault: true, description: Description);

    private static bool TryGetMaxTarget(IMethodSymbol methodSymbol, out ITypeSymbol targetType,
        out DiagnosticDescriptor rule)
    {
        rule = MaxRule;
        targetType = null!;

        if (methodSymbol.Name != MaxMethodName)
            return false;

        return TryGetMinMaxTarget(methodSymbol, out targetType);
    }
}
