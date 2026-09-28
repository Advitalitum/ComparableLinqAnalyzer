// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace ComparableLinqAnalyzer.Sample;

public class OrderByUsage
{
    public void Run()
    {
        // OrderBy - class types
        var notComparable = new List<NotComparable>();
        var first = notComparable.OrderBy(i => i); // diagnostic

        var implementingComparable = new List<ImplementsComparable>();
        var second = implementingComparable.OrderBy(i => i); // ok

        var implementingGenericComparable = new List<ImplementsGenericComparable>();
        var third = implementingGenericComparable.OrderBy(i => i); // ok

        // OrderBy - struct types
        var notComparableStruct = new List<NonComparableStruct>();
        var fourth = notComparableStruct.OrderBy(i => i); // diagnostic

        var implementingComparableStruct = new List<ImplementsComparableStruct>();
        var fifth = implementingComparableStruct.OrderBy(i => i); // ok

        var implementingGenericComparableStruct = new List<GenericComparableStruct>();
        var sixth = implementingGenericComparableStruct.OrderBy(i => i); // ok

        // OrderByDescending - class types
        var firstDesc = notComparable.OrderByDescending(i => i); // diagnostic

        var secondDesc = implementingComparable.OrderByDescending(i => i); // ok

        var thirdDesc = implementingGenericComparable.OrderByDescending(i => i); // ok

        // OrderByDescending - struct types
        var fourthDesc = notComparableStruct.OrderByDescending(i => i); // diagnostic

        var fifthDesc = implementingComparableStruct.OrderByDescending(i => i); // ok

        var sixthDesc = implementingGenericComparableStruct.OrderByDescending(i => i); // ok

        // Overloads with a comparer (not analyzed)
        var withComparer = notComparable.OrderBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value))); // ok
        var withComparerDesc = notComparable.OrderByDescending(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value))); // ok
    }
}
