using System;

namespace ComparableLinqAnalyzer.Sample;

public struct ImplementsComparableStruct : IComparable
{
    public int Value { get; set; }

    public int CompareTo(object? obj)
    {
        return Value.CompareTo(((ImplementsComparableStruct)obj!).Value);
    }
}
