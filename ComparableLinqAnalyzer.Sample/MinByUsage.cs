// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace ComparableLinqAnalyzer.Sample;

public class MinByUsage
{
    public void Run()
    {
        var words = new List<string> { "aa", "b", "ccc" };
        var shortest = words.MinBy(w => w.Length);

        var items = new List<NotComparable>();
        var first = items.MinBy(i => i);

        var withComparer = items.MinBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value)));

        var nonGeneric = new List<ImplementsComparable>();
        var byNonGeneric = nonGeneric.MinBy(i => i);

        var generic = new List<ImplementsGenericComparable>();
        var byGeneric = generic.MinBy(i => i);
    }
}
