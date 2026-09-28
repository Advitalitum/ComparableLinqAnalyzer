// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace ComparableLinqAnalyzer.Sample;

public class MinUsage
{
    public void Run()
    {
        // Class types
        var notComparable = new List<NotComparable>();
        var first = notComparable.Min(); // diagnostic

        var implementingComparable = new List<ImplementsComparable>();
        var second = implementingComparable.Min(); // ok

        var implementingGenericComparable = new List<ImplementsGenericComparable>();
        var third = implementingGenericComparable.Min(); // ok

        // Struct types
        var notComparableStruct = new List<NonComparableStruct>();
        var fourth = notComparableStruct.Min(); // diagnostic

        var implementingComparableStruct = new List<ImplementsComparableStruct>();
        var fifth = implementingComparableStruct.Min(); // ok

        var implementingGenericComparableStruct = new List<GenericComparableStruct>();
        var sixth = implementingGenericComparableStruct.Min(); // ok

        // Overload with a selector (not analyzed)
        var withSelector = notComparable.Min(i => i.Value); // ok
    }
}
