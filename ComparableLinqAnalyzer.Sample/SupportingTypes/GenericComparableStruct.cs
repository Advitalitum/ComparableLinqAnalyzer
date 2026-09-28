using System;

namespace ComparableLinqAnalyzer.Sample;

public struct GenericComparableStruct : IComparable<GenericComparableStruct>
{
    public int Value { get; set; }

    public int CompareTo(GenericComparableStruct other)
    {
        return Value.CompareTo(other.Value);
    }
}
