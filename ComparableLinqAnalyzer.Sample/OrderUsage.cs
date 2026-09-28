// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using System.Linq;

namespace ComparableLinqAnalyzer.Sample;

public class OrderUsage
{
    public void Run()
    {
        var numbers = new List<int> { 3, 1, 2 };
        var sorted = numbers.Order();
        var sortedDesc = numbers.OrderDescending();

        var items = new List<NotComparable>();
        var byElement = items.Order();
        var byElementDesc = items.OrderDescending();
    }
}
