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
        var numbers = new List<int> { 3, 1, 2 };
        var min = numbers.Min();
        var minByKey = numbers.Min(n => n);

        var items = new List<NotComparable>();
        var minElement = items.Min();
        var minBySelector = items.Min(i => i.Value);
    }
}
