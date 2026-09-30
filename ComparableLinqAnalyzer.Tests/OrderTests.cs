using System.Threading.Tasks;
using Xunit;
using Verifier =
    ComparableLinqAnalyzer.Tests.CustomAnalyzerVerifier<
        ComparableLinqAnalyzer.ComparableKeyAnalyzer>;

namespace ComparableLinqAnalyzer.Tests;

public class OrderTests
{
    [Fact]
    public async Task OrderWithNonComparableElement_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var ordered = items.Order();
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 36)
            .WithArguments("NotComparable", "Order");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderWithNonComparableStructElement_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparableStruct>();
        var ordered = items.Order();
    }
}

public struct NotComparableStruct
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 36)
            .WithArguments("NotComparableStruct", "Order");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderWithComparableStructElement_NoDiagnostic()
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
        var ordered = items.Order();
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
    public async Task OrderDescendingWithNonComparableElement_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var ordered = items.OrderDescending();
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 46)
            .WithArguments("NotComparable", "OrderDescending");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderDescendingWithNonComparableStructElement_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparableStruct>();
        var ordered = items.OrderDescending();
    }
}

public struct NotComparableStruct
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 46)
            .WithArguments("NotComparableStruct", "OrderDescending");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderDescendingWithComparableStructElement_NoDiagnostic()
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
        var ordered = items.OrderDescending();
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
    public async Task OrderWithComparerAndNonComparableElement_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var ordered = items.Order(Comparer<NotComparable>.Default);
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
    public async Task OrderWithNullComparerAndNonComparableElement_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var ordered = items.Order(null);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 40)
            .WithArguments("NotComparable", "Order");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderDescendingWithComparerAndNonComparableElement_NoDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var ordered = items.OrderDescending(Comparer<NotComparable>.Default);
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
    public async Task OrderDescendingWithNullComparerAndNonComparableElement_AlertDiagnostic()
    {
        const string text = @"
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public void Main()
    {
        var items = new List<NotComparable>();
        var ordered = items.OrderDescending(null);
    }
}

public class NotComparable
{
    public int Value { get; set; }
}
";

        var expected = Verifier.Diagnostic("CLA0001")
            .WithSpan(10, 23, 10, 50)
            .WithArguments("NotComparable", "OrderDescending");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task OrderWithNonGenericComparableClass_NoDiagnostic()
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
        var sorted = items.Order();
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
    public async Task OrderWithGenericComparableClass_NoDiagnostic()
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
        var sorted = items.Order();
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
    public async Task OrderWithNonGenericComparableStruct_NoDiagnostic()
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
        var sorted = items.Order();
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
    public async Task OrderDescendingWithNonGenericComparableClass_NoDiagnostic()
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
        var sorted = items.OrderDescending();
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
    public async Task OrderDescendingWithGenericComparableClass_NoDiagnostic()
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
        var sorted = items.OrderDescending();
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
    public async Task OrderDescendingWithNonGenericComparableStruct_NoDiagnostic()
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
        var sorted = items.OrderDescending();
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
