using System;

namespace ComparableLinqAnalyzer.Sample;

public class ImplementsGenericComparable : IComparable<ImplementsGenericComparable>
{
    public int Value { get; set; }

    public int CompareTo(ImplementsGenericComparable? other)
    {
        return Value.CompareTo(other!.Value);
    }
}
