namespace ComparableLinqAnalyzer.Tests;

internal static class TestSources
{
    internal const string Linq = @"
namespace System.Linq
{
    public static class TestMinBy
    {
        public static TSource MinBy<TSource, TKey>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector)
            => System.Linq.Enumerable.First(source);

        public static TSource MinBy<TSource, TKey>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector, System.Collections.Generic.IComparer<TKey>? comparer)
            => System.Linq.Enumerable.First(source);

        public static TSource MaxBy<TSource, TKey>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector)
            => System.Linq.Enumerable.First(source);

        public static TSource MaxBy<TSource, TKey>(this System.Collections.Generic.IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector, System.Collections.Generic.IComparer<TKey>? comparer)
            => System.Linq.Enumerable.First(source);

        public static System.Linq.IOrderedEnumerable<TSource> Order<TSource>(this System.Collections.Generic.IEnumerable<TSource> source)
            => System.Linq.Enumerable.OrderBy(source, x => x);

        public static System.Linq.IOrderedEnumerable<TSource> Order<TSource>(this System.Collections.Generic.IEnumerable<TSource> source, System.Collections.Generic.IComparer<TSource>? comparer)
            => System.Linq.Enumerable.OrderBy(source, x => x);

        public static System.Linq.IOrderedEnumerable<TSource> OrderDescending<TSource>(this System.Collections.Generic.IEnumerable<TSource> source)
            => System.Linq.Enumerable.OrderByDescending(source, x => x);

        public static System.Linq.IOrderedEnumerable<TSource> OrderDescending<TSource>(this System.Collections.Generic.IEnumerable<TSource> source, System.Collections.Generic.IComparer<TSource>? comparer)
            => System.Linq.Enumerable.OrderByDescending(source, x => x);
    }
}
";
}
