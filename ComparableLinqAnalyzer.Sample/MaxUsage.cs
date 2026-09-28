// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using System.Linq;

namespace ComparableLinqAnalyzer.Sample;

public class MaxUsage
{
    public void Run()
    {
        var numbers = new List<int> { 3, 1, 2 };
        var max = numbers.Max();

        var items = new List<NotComparable>();
        var maxElement = items.Max();
        var maxBySelector = items.Max(i => i.Value);
    }
}
