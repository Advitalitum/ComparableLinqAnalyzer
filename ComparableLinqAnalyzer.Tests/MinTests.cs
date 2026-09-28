using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        ComparableLinqAnalyzer.ComparableKeyAnalyzer>;

namespace ComparableLinqAnalyzer.Tests;

public class MinTests
{
    [Fact]
    public async Task MinWithNonComparableElement_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var min = items.Min();
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0007")
            .WithSpan(10, 19, 10, 30)
            .WithArguments("NotComparable", "Min");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinWithSelectorNonComparable_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var min = items.Min(i => i);
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
    public async Task MinWithNumericSelector_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var min = items.Min(i => i.Value);
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
    public async Task MinWithNonComparableStructElement_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparableStruct>();
        var min = items.Min();
    }
}

public struct NotComparableStruct
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0007")
            .WithSpan(10, 19, 10, 30)
            .WithArguments("NotComparableStruct", "Min");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinWithSelectorNonComparableStruct_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparableStruct>();
        var min = items.Min(i => i);
    }
}

public struct NotComparableStruct
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinWithComparableStructElement_NoDiagnostic()
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
        var min = items.Min();
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
    public async Task MinWithNonGenericComparableClass_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<ImplementsComparable> { new ImplementsComparable() };
        var min = items.Min();
    }
}

public class ImplementsComparable : IComparable
{
    public int Value { get; set; }

    public int CompareTo(object obj) => Value.CompareTo(((ImplementsComparable)obj).Value);
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinWithGenericComparableClass_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<ImplementsGenericComparable> { new ImplementsGenericComparable() };
        var min = items.Min();
    }
}

public class ImplementsGenericComparable : IComparable<ImplementsGenericComparable>
{
    public int Value { get; set; }

    public int CompareTo(ImplementsGenericComparable other) => Value.CompareTo(other.Value);
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinWithNonGenericComparableStruct_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<ImplementsComparableStruct> { new ImplementsComparableStruct() };
        var min = items.Min();
    }
}

public struct ImplementsComparableStruct : IComparable
{
    public int Value { get; set; }

    public int CompareTo(object obj) => Value.CompareTo(((ImplementsComparableStruct)obj).Value);
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }
}
