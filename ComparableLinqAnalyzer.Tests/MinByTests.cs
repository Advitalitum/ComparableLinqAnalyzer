using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        ComparableLinqAnalyzer.ComparableKeyAnalyzer>;

namespace ComparableLinqAnalyzer.Tests;

public class MinByTests
{
    [Fact]
    public async Task MinByWithComparableStructKey_NoDiagnostic()
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
        var first = items.MinBy(i => i);
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
    public async Task MinByWithNonComparableKey_AlertDiagnostic()
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
        var first = items.MinBy(i => i);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(11, 21, 11, 40)
            .WithArguments("NotComparable", "MinBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinByWithNonComparableStruct_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparableStruct>();
        var first = items.MinBy(i => i);
    }
}

public struct NotComparableStruct
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 21, 10, 40)
            .WithArguments("NotComparableStruct", "MinBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinByWithComparableInterfaceKey_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<Comparable> { new Comparable() };
        var first = items.MinBy(i => i);
    }
}

public class Comparable : IComparable<Comparable>
{
    public int CompareTo(Comparable other) => 0;
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinByWithNonGenericComparableKey_NoDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<Comparable> { new Comparable() };
        var first = items.MinBy(i => i);
    }
}

public class Comparable : IComparable
{
    public int CompareTo(object obj) => 0;
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinByWithComparableOfDifferentType_AlertDiagnostic()
    {
        const string text = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<Item> { new Item() };
        var first = items.MinBy(i => i);
    }
}

public class Item : IComparable<Other>
{
    public int Value { get; set; }

    public int CompareTo(Other other) => 0;
}

public class Other
{
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(11, 21, 11, 40)
            .WithArguments("Item", "MinBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinByWithNullableValueTypeKey_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<decimal>();
        var first = items.MinBy(i => (decimal?)i);
    }
}
" + TestSources.Linq;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinByWithNonComparableStructPropertyKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<Item>();
        var first = items.MinBy(i => i.ShiftStart);
    }
}

public class Item
{
    public DateTimeUtc ShiftStart { get; set; }
}

public readonly record struct DateTimeUtc
{
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 21, 10, 51)
            .WithArguments("DateTimeUtc", "MinBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinByWithComparerAndNonComparableKey_NoDiagnostic()
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
        var first = items.MinBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value)));
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
    public async Task MinByWithNonComparablePropertyViaGroupBySelect_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var wrappers = new List<ShiftWrapper>();
        var byProperty = wrappers.GroupBy(w => w.ShiftStart.Value).Select(g => g.MinBy(x => x.ShiftStart));
    }
}

public class ShiftWrapper
{
    public NotComparable ShiftStart { get; set; } = new();
}

public class NotComparable
{
    public int Value { get; set; }
}
" + TestSources.Linq;

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 80, 10, 106)
            .WithArguments("NotComparable", "MinBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MinByWithNonGenericComparableStruct_NoDiagnostic()
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
        var first = items.MinBy(i => i);
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
