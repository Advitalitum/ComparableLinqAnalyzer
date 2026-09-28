// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace MinByAnalyzer.Sample;

public class Examples
{
    public void MinByUsage()
    {
        var words = new List<string> { "aa", "b", "ccc" };
        var shortest = words.MinBy(w => w.Length);

        var items = new List<NotComparable>();
        var first = items.MinBy(i => i);

        var withComparer = items.MinBy(i => i, Comparer<NotComparable>.Create((x, y) => x.Value.CompareTo(y.Value)));

        var nonGeneric = new List<ImplementsComparable>();
        var byNonGeneric = nonGeneric.MinBy(i => i);

        var generic = new List<ImplementsGenericComparable>();
        var byGeneric = generic.MinBy(i => i);
    }

    private class ImplementsComparable : IComparable
    {
        public int Value { get; set; }

        public int CompareTo(object? obj)
        {
            return Value.CompareTo(((ImplementsComparable)obj!).Value);
        }
    }

    private class ImplementsGenericComparable : IComparable<ImplementsGenericComparable>
    {
        public int Value { get; set; }

        public int CompareTo(ImplementsGenericComparable? other)
        {
            return Value.CompareTo(other!.Value);
        }
    }

    private class NotComparable
    {
        public int Value { get; set; }
    }
}