// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace ComparableLinqAnalyzer.Sample;

public class MaxByUsage
{
    public void Run()
    {
        var words = new List<string> { "aa", "b", "ccc" };
        var longest = words.MaxBy(w => w.Length);

        var items = new List<NotComparable>();
        var first = items.MaxBy(i => i);

        var withComparer = items.MaxBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value)));
    }
}
