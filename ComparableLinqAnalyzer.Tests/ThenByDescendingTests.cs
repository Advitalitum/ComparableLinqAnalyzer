using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        ComparableLinqAnalyzer.ComparableKeyAnalyzer>;

namespace ComparableLinqAnalyzer.Tests;

public class ThenByDescendingTests
{
    [Fact]
    public async Task ThenByDescendingWithNonComparableClassKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var sorted = items.OrderBy(i => i.Value);
        var ordered = sorted.ThenByDescending(i => i);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(11, 23, 11, 54)
            .WithArguments("NotComparable", "ThenByDescending");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task ThenByDescendingWithNonComparableStructKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparableStruct>();
        var sorted = items.OrderBy(i => i.Value);
        var ordered = sorted.ThenByDescending(i => i);
    }
}

public struct NotComparableStruct
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(11, 23, 11, 54)
            .WithArguments("NotComparableStruct", "ThenByDescending");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task ThenByDescendingWithNonComparableStructPropertyKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<Item>();
        var sorted = items.OrderBy(i => i.Value);
        var ordered = sorted.ThenByDescending(i => i.ShiftStart);
    }
}

public class Item
{
    public int Value { get; set; }
    public DateTimeUtc ShiftStart { get; set; }
}

public readonly record struct DateTimeUtc
{
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(11, 23, 11, 65)
            .WithArguments("DateTimeUtc", "ThenByDescending");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task ThenByDescendingWithComparablePropertyKey_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<Item>();
        var sorted = items.OrderBy(i => i.Value);
        var ordered = sorted.ThenByDescending(i => i.Name);
    }
}

public class Item
{
    public int Value { get; set; }
    public string Name { get; set; }
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task ThenByDescendingWithComparerAndNonComparableKey_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var sorted = items.OrderBy(i => i.Value);
        var ordered = sorted.ThenByDescending(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value)));
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
    public async Task ThenByDescendingWithNullComparerAndNonComparableKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var sorted = items.OrderBy(i => i.Value);
        var ordered = sorted.ThenByDescending(i => i, null);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(11, 23, 11, 60)
            .WithArguments("NotComparable", "ThenByDescending");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task ThenByDescendingWithGenericComparableClass_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<ImplementsGenericComparable>();
        var sorted = items.OrderBy(i => i.Value);
        var ordered = sorted.ThenByDescending(i => i);
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
    public async Task ThenByDescendingWithNonGenericComparableClass_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<ImplementsComparable>();
        var sorted = items.OrderBy(i => i.Value);
        var ordered = sorted.ThenByDescending(i => i);
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
    public async Task ThenByDescendingWithGenericComparableStruct_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<ComparableStruct>();
        var sorted = items.OrderBy(i => i.Value);
        var ordered = sorted.ThenByDescending(i => i);
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
    public async Task ThenByDescendingWithNonGenericComparableStruct_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<ImplementsComparableStruct>();
        var sorted = items.OrderBy(i => i.Value);
        var ordered = sorted.ThenByDescending(i => i);
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
