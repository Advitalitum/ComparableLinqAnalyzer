// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace ComparableLinqAnalyzer.Sample;

public class ThenByUsage
{
    public void Run()
    {
        // ThenBy - class types
        var notComparable = new List<NotComparable>();
        var first = notComparable.OrderBy(i => i.Value).ThenBy(i => i); // diagnostic

        var implementingComparable = new List<ImplementsComparable>();
        var second = implementingComparable.OrderBy(i => i.Value).ThenBy(i => i); // ok

        var implementingGenericComparable = new List<ImplementsGenericComparable>();
        var third = implementingGenericComparable.OrderBy(i => i.Value).ThenBy(i => i); // ok

        // ThenBy - struct types
        var notComparableStruct = new List<NonComparableStruct>();
        var fourth = notComparableStruct.OrderBy(i => i.Value).ThenBy(i => i); // diagnostic

        var implementingComparableStruct = new List<ImplementsComparableStruct>();
        var fifth = implementingComparableStruct.OrderBy(i => i.Value).ThenBy(i => i); // ok

        var implementingGenericComparableStruct = new List<GenericComparableStruct>();
        var sixth = implementingGenericComparableStruct.OrderBy(i => i.Value).ThenBy(i => i); // ok

        // ThenByDescending - class types
        var firstDesc = notComparable.OrderBy(i => i.Value).ThenByDescending(i => i); // diagnostic

        var secondDesc = implementingComparable.OrderBy(i => i.Value).ThenByDescending(i => i); // ok

        var thirdDesc = implementingGenericComparable.OrderBy(i => i.Value).ThenByDescending(i => i); // ok

        // ThenByDescending - struct types
        var fourthDesc = notComparableStruct.OrderBy(i => i.Value).ThenByDescending(i => i); // diagnostic

        var fifthDesc = implementingComparableStruct.OrderBy(i => i.Value).ThenByDescending(i => i); // ok

        var sixthDesc = implementingGenericComparableStruct.OrderBy(i => i.Value).ThenByDescending(i => i); // ok

        // Overloads with a comparer
        var withComparer = notComparable.OrderBy(i => i.Value).ThenBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value))); // ok - real comparer
        var withComparerDesc = notComparable.OrderBy(i => i.Value).ThenByDescending(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value))); // ok - real comparer
        var withNullComparer = notComparable.OrderBy(i => i.Value).ThenBy(i => i, null); // diagnostic - falls back to Comparer<T>.Default
        var withNullComparerDesc = notComparable.OrderBy(i => i.Value).ThenByDescending(i => i, null); // diagnostic - falls back to Comparer<T>.Default
    }
}
