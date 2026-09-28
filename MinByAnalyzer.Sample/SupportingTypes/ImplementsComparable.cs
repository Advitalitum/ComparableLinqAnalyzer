using System;

namespace MinByAnalyzer.Sample;

public class ImplementsComparable : IComparable
{
    public int Value { get; set; }

    public int CompareTo(object? obj)
    {
        return Value.CompareTo(((ImplementsComparable)obj!).Value);
    }
}
