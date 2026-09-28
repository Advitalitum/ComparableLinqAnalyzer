using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        MinByAnalyzer.MinByComparableAnalyzer>;

namespace MinByAnalyzer.Tests;

public class MinByComparableAnalyzerTests
{
    private const string TestMinBy = @"
namespace System.Linq
{
    public static class TestMinBy
    {
        public static TSource MinBy<TSource, TKey>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector)
            => System.Linq.Enumerable.First(source);

        public static TSource MinBy<TSource, TKey>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector, System.Collections.Generic.IComparer<TKey> comparer)
            => System.Linq.Enumerable.First(source);
    }
}
";

    [Fact]
    public async Task MinByWithComparableKey_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var words = new List<string> { ""aa"", ""b"", ""ccc"" };
        var shortest = words.MinBy(w => w.Length);
    }
}
" + TestMinBy;

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
" + TestMinBy;

        var expected = Verifier.Diagnostic()
            .WithSpan(11, 21, 11, 40)
            .WithArguments("NotComparable");
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
" + TestMinBy;

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
" + TestMinBy;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
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
" + TestMinBy;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
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
" + TestMinBy;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }
}
