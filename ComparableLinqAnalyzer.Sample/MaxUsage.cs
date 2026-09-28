// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace ComparableLinqAnalyzer.Sample;

public class MaxUsage
{
    public void Run()
    {
        // Class types
        var notComparable = new List<NotComparable>();
        var first = notComparable.Max(); // diagnostic

        var implementingComparable = new List<ImplementsComparable>();
        var second = implementingComparable.Max(); // ok

        var implementingGenericComparable = new List<ImplementsGenericComparable>();
        var third = implementingGenericComparable.Max(); // ok

        // Struct types
        var notComparableStruct = new List<NonComparableStruct>();
        var fourth = notComparableStruct.Max(); // diagnostic

        var implementingComparableStruct = new List<ImplementsComparableStruct>();
        var fifth = implementingComparableStruct.Max(); // ok

        var implementingGenericComparableStruct = new List<GenericComparableStruct>();
        var sixth = implementingGenericComparableStruct.Max(); // ok

        // Overload with a selector (not analyzed)
        var withSelector = notComparable.Max(i => i.Value); // ok
    }
}
