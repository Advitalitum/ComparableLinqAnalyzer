// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using System.Linq;

namespace MinByAnalyzer.Sample;

public class OrderByUsage
{
    public void Run()
    {
        var words = new List<string> { "aa", "b", "ccc" };
        var ordered = words.OrderBy(w => w.Length);
        var orderedDesc = words.OrderByDescending(w => w.Length);

        var items = new List<NotComparable>();
        var byKey = items.OrderBy(i => i);
    }
}
