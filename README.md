# Roslyn Analyzers

A set of three projects that includes Roslyn analyzers.

## Content
### MinByAnalyzer
A .NET Standard project with the implementation of the MinBy analyzer.
**You must build this project to see the results (errors) in the IDE.**

- [MinByComparableAnalyzer.cs](MinByComparableAnalyzer.cs): An analyzer that reports a `MinBy` call whose key type (`TKey`) does not implement `IComparable` or `IComparable<TKey>`.

### MinByAnalyzer.Sample
A project that references the analyzer. Note the parameters of `ProjectReference` in [MinByAnalyzer.Sample.csproj](../MinByAnalyzer.Sample/MinByAnalyzer.Sample.csproj), they make sure that the project is referenced as a set of analyzers. 

### MinByAnalyzer.Tests
Unit tests for the analyzer. The easiest way to develop language-related features is to start with unit tests.

## How To?
### How to debug?
- Use the [launchSettings.json](Properties/launchSettings.json) profile.
- Debug tests.

### How can I determine which syntax nodes I should expect?
Consider using the Roslyn Visualizer tool window, which allows you to observe the syntax tree.

### Learn more about wiring analyzers
The complete set of information is available at [roslyn github repo wiki](https://github.com/dotnet/roslyn/blob/main/docs/wiki/README.md).