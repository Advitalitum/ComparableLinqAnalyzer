# ComparableLinqAnalyzer

A set of projects that contain a Roslyn analyzer validating the use of LINQ methods that require comparable keys.

## What the analyzer does

The `ComparableKeyAnalyzer` inspects calls to the following LINQ methods:

- `MinBy`, `MaxBy`
- `OrderBy`, `OrderByDescending`
- `Order`, `OrderDescending`
- `Min`, `Max`

For each call, the analyzer determines the type used for comparison (the selector key for `MinBy`/`MaxBy`/`OrderBy`/`OrderByDescending`, or the sequence element for `Order`/`OrderDescending`/`Min`/`Max`) and checks whether that type implements `IComparable` or `IComparable<T>`.

If the type is **not** comparable, the analyzer reports an error.

### When it fires

```csharp
var items = new List<NotComparable>();   // NotComparable does not implement IComparable

var a = items.MinBy(i => i);              // ❌ diagnostic
var b = items.MaxBy(i => i);              // ❌ diagnostic
var c = items.OrderBy(i => i);            // ❌ diagnostic
var d = items.OrderByDescending(i => i);  // ❌ diagnostic
var e = items.Order();                    // ❌ diagnostic
var f = items.OrderDescending();          // ❌ diagnostic
var g = items.Min();                      // ❌ diagnostic
var h = items.Max();                      // ❌ diagnostic
```

### When it does not fire

- The type implements `IComparable` or `IComparable<T>` (including built-in types):

```csharp
var numbers = new List<int>();            // int : IComparable
var comparable = new List<SomeClass>();   // SomeClass : IComparable<SomeClass>

var a = comparable.MinBy(i => i);             // ✅
var b = comparable.MaxBy(i => i);             // ✅
var c = comparable.OrderBy(i => i);           // ✅
var d = comparable.OrderByDescending(i => i); // ✅
var e = numbers.Order();                      // ✅
var f = numbers.OrderDescending();            // ✅
var g = numbers.Min();                        // ✅
var h = numbers.Max();                        // ✅
```

- An overload with an explicit comparer (the calling code controls the ordering):

```csharp
var items = new List<NotComparable>();
var a = items.OrderBy(i => i, comparer);  // ✅
var b = items.MaxBy(i => i, comparer);    // ✅
var c = items.Order(comparer);            // ✅
```

- `Min`/`Max` with a selector (these overloads are not analyzed).

> Comparability is analyzed on the source type — `UnwrapNullable` unwraps `Nullable<T>` down to the underlying type.

## Installation

The analyzer is available on NuGet: [ComparableLinqAnalyzer](https://www.nuget.org/packages/ComparableLinqAnalyzer).

### NuGet Package Manager

```powershell
Install-Package ComparableLinqAnalyzer
```

### .NET CLI

```bash
dotnet add package ComparableLinqAnalyzer
```

### PackageReference (project file)

```xml
<ItemGroup>
    <PackageReference Include="ComparableLinqAnalyzer" Version="0.0.2" />
</ItemGroup>
```

## Rules

| Rule ID  | Method            | Severity |
|----------|-------------------|----------|
| CLA0001  | `MinBy`           | Error    |
| CLA0002  | `MaxBy`           | Error    |
| CLA0003  | `OrderBy`         | Error    |
| CLA0004  | `OrderByDescending` | Error  |
| CLA0005  | `Order`           | Error    |
| CLA0006  | `OrderDescending` | Error    |
| CLA0007  | `Min`             | Error    |
| CLA0008  | `Max`             | Error    |

## Content

### ComparableLinqAnalyzer
A .NET Standard project with the implementation of the comparable-key analyzer.
**You must build this project to see the results (errors) in the IDE.**

### ComparableLinqAnalyzer.Sample
A project that references the analyzer. Note the parameters of `ProjectReference` in [ComparableLinqAnalyzer.Sample.csproj](ComparableLinqAnalyzer.Sample/ComparableLinqAnalyzer.Sample.csproj) — they make sure that the project is referenced as a set of analyzers.

### ComparableLinqAnalyzer.Tests
Unit tests for the analyzer. The easiest way to develop language-related features is to start with unit tests.

## How to build the package

```bash
dotnet pack ComparableLinqAnalyzer/ComparableLinqAnalyzer.csproj -c Release
```

## How to debug?

- Use the [launchSettings.json](ComparableLinqAnalyzer/Properties/launchSettings.json) profile.
- Debug tests.

## How can I determine which syntax nodes I should expect?

Consider using the Roslyn Visualizer tool window, which allows you to observe the syntax tree.
