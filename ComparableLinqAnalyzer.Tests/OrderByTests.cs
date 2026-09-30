using System.Threading.Tasks;
using Xunit;
using Verifier =
    ComparableLinqAnalyzer.Tests.CustomAnalyzerVerifier<
        ComparableLinqAnalyzer.ComparableKeyAnalyzer>;

namespace ComparableLinqAnalyzer.Tests;

public class OrderByTests
{
    [Fact]
    public async Task OrderByWithNonComparableKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var ordered = items.OrderBy(i => i);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 44)
            .WithArguments("NotComparable", "OrderBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderByWithComparerAndNonComparableKey_NoDiagnostic()
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
        var ordered = items.OrderBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value)));
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
    public async Task OrderByWithNullComparerAndNonComparableKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var ordered = items.OrderBy(i => i, null);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 50)
            .WithArguments("NotComparable", "OrderBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderByWithNonComparableStruct_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparableStruct>();
        var ordered = items.OrderBy(i => i);
    }
}

public struct NotComparableStruct
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 44)
            .WithArguments("NotComparableStruct", "OrderBy");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderByDescendingWithComparableStructKey_NoDiagnostic()
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
        var ordered = items.OrderByDescending(i => i);
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
    public async Task OrderByDescendingWithComparerAndNonComparableKey_NoDiagnostic()
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
        var ordered = items.OrderByDescending(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value)));
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
    public async Task OrderByDescendingWithNullComparerAndNonComparableKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var ordered = items.OrderByDescending(i => i, null);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 60)
            .WithArguments("NotComparable", "OrderByDescending");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderByWithGenericComparableClass_NoDiagnostic()
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
        var ordered = items.OrderBy(i => i);
        var orderedDesc = items.OrderByDescending(i => i);
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
    public async Task OrderByWithNonGenericComparableClass_NoDiagnostic()
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
        var ordered = items.OrderBy(i => i);
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
    public async Task OrderByWithGenericComparableStruct_NoDiagnostic()
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
        var ordered = items.OrderBy(i => i);
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
    public async Task OrderByWithNonGenericComparableStruct_NoDiagnostic()
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
        var ordered = items.OrderBy(i => i);
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

    [Fact]
    public async Task OrderByDescendingWithNonGenericComparableClass_NoDiagnostic()
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
        var ordered = items.OrderByDescending(i => i);
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
    public async Task OrderByDescendingWithNonGenericComparableStruct_NoDiagnostic()
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
        var ordered = items.OrderByDescending(i => i);
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

    [Fact]
    public async Task OrderByDescendingWithNonComparableClassKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var ordered = items.OrderByDescending(i => i);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 54)
            .WithArguments("NotComparable", "OrderByDescending");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderByDescendingWithNonComparableStructKey_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparableStruct>();
        var ordered = items.OrderByDescending(i => i);
    }
}

public struct NotComparableStruct
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 54)
            .WithArguments("NotComparableStruct", "OrderByDescending");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }
}
