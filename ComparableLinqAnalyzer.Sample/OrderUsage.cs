// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace ComparableLinqAnalyzer.Sample;

public class OrderUsage
{
    public void Run()
    {
        // Order - class types
        var notComparable = new List<NotComparable>();
        var first = notComparable.Order(); // diagnostic

        var implementingComparable = new List<ImplementsComparable>();
        var second = implementingComparable.Order(); // ok

        var implementingGenericComparable = new List<ImplementsGenericComparable>();
        var third = implementingGenericComparable.Order(); // ok

        // Order - struct types
        var notComparableStruct = new List<NonComparableStruct>();
        var fourth = notComparableStruct.Order(); // diagnostic

        var implementingComparableStruct = new List<ImplementsComparableStruct>();
        var fifth = implementingComparableStruct.Order(); // ok

        var implementingGenericComparableStruct = new List<GenericComparableStruct>();
        var sixth = implementingGenericComparableStruct.Order(); // ok

        // OrderDescending - class types
        var firstDesc = notComparable.OrderDescending(); // diagnostic

        var secondDesc = implementingComparable.OrderDescending(); // ok

        var thirdDesc = implementingGenericComparable.OrderDescending(); // ok

        // OrderDescending - struct types
        var fourthDesc = notComparableStruct.OrderDescending(); // diagnostic

        var fifthDesc = implementingComparableStruct.OrderDescending(); // ok

        var sixthDesc = implementingGenericComparableStruct.OrderDescending(); // ok

        // Overloads with a comparer (not analyzed)
        var withComparer = notComparable.Order(Comparer<NotComparable>.Default); // ok
        var withComparerDesc = notComparable.OrderDescending(Comparer<NotComparable>.Default); // ok
    }
}
