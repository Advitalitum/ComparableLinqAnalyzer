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
        // Class types
        var notComparable = new List<NotComparable>();
        var first = notComparable.MaxBy(i => i); // diagnostic

        var implementingComparable = new List<ImplementsComparable>();
        var second = implementingComparable.MaxBy(i => i); // ok

        var implementingGenericComparable = new List<ImplementsGenericComparable>();
        var third = implementingGenericComparable.MaxBy(i => i); // ok

        // Struct types
        var notComparableStruct = new List<NonComparableStruct>();
        var fourth = notComparableStruct.MaxBy(i => i); // diagnostic

        var implementingComparableStruct = new List<ImplementsComparableStruct>();
        var fifth = implementingComparableStruct.MaxBy(i => i); // ok

        var implementingGenericComparableStruct = new List<GenericComparableStruct>();
        var sixth = implementingGenericComparableStruct.MaxBy(i => i); // ok

        // Overload with a comparer
        var withComparer = notComparable.MaxBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value))); // ok - real comparer
        var withNullComparer = notComparable.MaxBy(i => i, null); // diagnostic - falls back to Comparer<T>.Default
    }
}
