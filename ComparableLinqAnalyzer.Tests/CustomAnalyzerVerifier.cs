using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace ComparableLinqAnalyzer.Tests;

public static partial class CustomAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic(string diagnosticId)
    {
        var result = new DiagnosticResult(diagnosticId, DiagnosticSeverity.Error);

        return result;
    }

    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new Test
        {
            TestCode = source,
        };

        test.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync();
    }

    private sealed class Test : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    {
        public Test()
        {
            // Анализируем код против современного .NET, где MinBy/MaxBy/Order/OrderDescending -
            // реальные методы System.Linq.Enumerable, а не локальная эмуляция.
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
        }
    }
}
