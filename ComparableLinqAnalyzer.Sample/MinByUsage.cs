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
        // Class types
        var notComparable = new List<NotComparable>();
        var first = notComparable.MinBy(i => i); // diagnostic

        var implementingComparable = new List<ImplementsComparable>();
        var second = implementingComparable.MinBy(i => i); // ok

        var implementingGenericComparable = new List<ImplementsGenericComparable>();
        var third = implementingGenericComparable.MinBy(i => i); // ok

        // Struct types
        var notComparableStruct = new List<NonComparableStruct>();
        var fourth = notComparableStruct.MinBy(i => i); // diagnostic

        var implementingComparableStruct = new List<ImplementsComparableStruct>();
        var fifth = implementingComparableStruct.MinBy(i => i); // ok

        var implementingGenericComparableStruct = new List<GenericComparableStruct>();
        var sixth = implementingGenericComparableStruct.MinBy(i => i); // ok

        // Overload with a comparer (not analyzed)
        var withComparer = notComparable.MinBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value))); // ok
    }
}
