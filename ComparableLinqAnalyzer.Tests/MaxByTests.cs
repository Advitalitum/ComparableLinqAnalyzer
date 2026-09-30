using System.Threading.Tasks;
using Xunit;
using Verifier =
    ComparableLinqAnalyzer.Tests.CustomAnalyzerVerifier<
        ComparableLinqAnalyzer.ComparableKeyAnalyzer>;

namespace ComparableLinqAnalyzer.Tests;

public class MaxByTests
{
    [Fact]
    public async Task MaxByWithNonComparableKey_AlertDiagnostic()
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
        var first = items.MaxBy(i => i);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(11, 21, 11, 40)
            .WithArguments("NotComparable", "MaxBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxByWithNonComparableStruct_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparableStruct>();
        var first = items.MaxBy(i => i);
    }
}

public struct NotComparableStruct
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 21, 10, 40)
            .WithArguments("NotComparableStruct", "MaxBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxByWithComparableStructKey_NoDiagnostic()
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
        var first = items.MaxBy(i => i);
    }
}

public struct ComparableStruct : IComparable<ComparableStruct>
{
    public int Value { get; set; }

    public int CompareTo(ComparableStruct other) => Value.CompareTo(other.Value);
}
";

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxByWithComparerAndNonComparableKey_NoDiagnostic()
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
        var first = items.MaxBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value)));
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxByWithNullComparerAndNonComparableKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var first = items.MaxBy(i => i, null);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 21, 10, 46)
            .WithArguments("NotComparable", "MaxBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxByWithNullComparerAndComparableKey_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<int>();
        var first = items.MaxBy(i => i, null);
    }
}
";

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxByWithGenericComparableClass_NoDiagnostic()
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
        var first = items.MaxBy(i => i);
    }
}

public class ImplementsGenericComparable : IComparable<ImplementsGenericComparable>
{
    public int Value { get; set; }

    public int CompareTo(ImplementsGenericComparable other) => Value.CompareTo(other.Value);
}
";

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxByWithNonGenericComparableClass_NoDiagnostic()
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
        var first = items.MaxBy(i => i);
    }
}

public class ImplementsComparable : IComparable
{
    public int Value { get; set; }

    public int CompareTo(object obj) => Value.CompareTo(((ImplementsComparable)obj).Value);
}
";

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task MaxByWithNonGenericComparableStruct_NoDiagnostic()
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
        var first = items.MaxBy(i => i);
    }
}

public struct ImplementsComparableStruct : IComparable
{
    public int Value { get; set; }

    public int CompareTo(object obj) => Value.CompareTo(((ImplementsComparableStruct)obj).Value);
}
";

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }
}
