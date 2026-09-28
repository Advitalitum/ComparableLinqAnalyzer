// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using System.Linq;

namespace ComparableLinqAnalyzer.Sample;

public class OrderByUsage
{
    public void Run()
    {
        var words = new List<string> { "aa", "b", "ccc" };
        var ordered = words.OrderBy(w => w.Length);
        var orderedDesc = words.OrderByDescending(w => w.Length);

        var items = new List<NotComparable>();
        var byKey = items.OrderBy(i => i);

        var withComparer = items.OrderBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value)));
        var withComparerDesc = items.OrderByDescending(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value)));
    }
}
