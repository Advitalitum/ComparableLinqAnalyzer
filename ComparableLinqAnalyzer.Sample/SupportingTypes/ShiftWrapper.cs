namespace ComparableLinqAnalyzer.Sample;

public sealed record ShiftWrapper
{
    public NotComparable ShiftStart { get; init; } = new();
}
