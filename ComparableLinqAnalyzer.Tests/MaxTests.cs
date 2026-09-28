using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        ComparableLinqAnalyzer.ComparableKeyAnalyzer>;

namespace ComparableLinqAnalyzer.Tests;

public class MaxTests
{
    [Fact]
    public async Task MaxWithNonComparableElement_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var max = items.Max();
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0008")
            .WithSpan(10, 19, 10, 30)
            .WithArguments("NotComparable", "Max");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxWithNonComparableStructElement_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparableStruct>();
        var max = items.Max();
    }
}

public struct NotComparableStruct
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0008")
            .WithSpan(10, 19, 10, 30)
            .WithArguments("NotComparableStruct", "Max");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxWithComparableStructElement_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<ComparableStruct> { new ComparableStruct() };
        var max = items.Max();
    }
}

public struct ComparableStruct : IComparable<ComparableStruct>
{
    public int Value { get; set; }

    public int CompareTo(ComparableStruct other) => Value.CompareTo(other.Value);
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxWithSelectorNonComparable_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var max = items.Max(i => i);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxWithNumericSelector_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var max = items.Max(i => i.Value);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }
}
