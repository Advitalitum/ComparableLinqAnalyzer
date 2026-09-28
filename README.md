# Roslyn Analyzers

A set of three projects that includes Roslyn analyzers.

## Content
### ComparableLinqAnalyzer
A .NET Standard project with the implementation of the comparable-key analyzer.
**You must build this project to see the results (errors) in the IDE.**

- [ComparableKeyAnalyzer.cs](ComparableLinqAnalyzer/ComparableKeyAnalyzer.cs): An analyzer that reports `MinBy`, `MaxBy`, `OrderBy`, `OrderByDescending`, `Order`, `OrderDescending`, `Min` or `Max` calls whose comparable type does not implement `IComparable` or `IComparable<T>`.

### ComparableLinqAnalyzer.Sample
A project that references the analyzer. Note the parameters of `ProjectReference` in [ComparableLinqAnalyzer.Sample.csproj](../ComparableLinqAnalyzer.Sample/ComparableLinqAnalyzer.Sample.csproj), they make sure that the project is referenced as a set of analyzers.

### ComparableLinqAnalyzer.Tests
Unit tests for the analyzer. The easiest way to develop language-related features is to start with unit tests.

## How To?
### How to debug?
- Use the [launchSettings.json](Properties/launchSettings.json) profile.
- Debug tests.

### How can I determine which syntax nodes I should expect?
Consider using the Roslyn Visualizer tool window, which allows you to observe the syntax tree.

### Learn more about wiring analyzers
The complete set of information is available at [roslyn github repo wiki](https://github.com/dotnet/roslyn/blob/main/docs/wiki/README.md).
