[![Build status](https://img.shields.io/appveyor/ci/gsscoder/commandline.svg)](https://ci.appveyor.com/project/gsscoder/commandline)
[![Nuget](https://img.shields.io/nuget/dt/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)
[![Nuget](https://img.shields.io/nuget/v/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)
[![Nuget](https://img.shields.io/nuget/vpre/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)

Command Line Parser Library 2.0.21.0 alpha for CLR.
===
The Command Line Parser Library offers to CLR applications a clean and concise API for manipulating command line arguments and related tasks defining switches, options and verb commands. It allows you to display an help screen with an high degree of customization and a simple way to report syntax errors to the end user.

Everything that is boring and repetitive to be programmed stands up on library shoulders, letting developers concentrate on core logic. It's written in **C#** and doesn't depend on other packages.

__This library provides _hassle free_ command line parsing with a constantly updated API since 2005.__

Compatibility:
---
  - .NET Framework 4.0+
  - Mono 2.1+ Profile

Current Release:
---
  - This is a __pre-release__, for documentation please read appropriate [wiki section](https://github.com/gsscoder/commandline/wiki/Latest-Version). From version **2.0.x-pre+** parsing kernel was rewritten and public API simplified.

At glance:
---
  - One line parsing using default singleton: ``CommandLine.Parser.Default.ParseArguments(...)``.
  - Automatic or one line help screen generator: ``HelpText.AutoBuild(...)``.
  - Map to sequences (``IEnumerable<T>``) or standard scalar types, including enum and ``Nullable<T>``.
  - __Plug-In friendly__ architecture as explained [here](https://github.com/gsscoder/commandline/wiki/Plug-in-Friendly-Architecture).
  - Define [verb commands](https://github.com/gsscoder/commandline/wiki/Latest-Version#verbs) as ``git commit -a``.
  - Most of features applies with a [CoC](http://en.wikipedia.org/wiki/Convention_over_configuration) philosophy.
  - F#-friendly with support for ``option<'a>``, see [demo](https://github.com/gsscoder/commandline/blob/master/demo/fsharp-demo.fsx).

To install:
---
  - NuGet way (latest stable): ``Install-Package CommandLineParser``
  - NuGet way (latest version): ``Install-Package CommandLineParser -pre``
  - XCOPY way: ``cp -r ClonedRepo/src/CommandLine To/Your/Project/Dir``

To build:
---
- [FAKE](http://fsharp.github.io/FAKE/) Script (under development)
- MS Visual Studio
- Xamarin Studio (not tested)

Public API:
---
Latest changes are recorded from Version 1.9.4.91, please refer to [this document](https://github.com/gsscoder/commandline/blob/master/doc/PublicAPI.md).

Notes:
---
The project is and well suited to be included in your application. If you don't merge it to your project tree, you must reference ``CommandLine.dll`` and import ``CommandLine`` and ``CommandLine.Text`` namespaces (or install via NuGet). The help text builder and its support types lives in ``CommandLine.Text`` namespace that is loosely coupled with the parser. However is good to know that ``HelpText`` class will avoid a lot of repetitive coding.

**C#:**

Define a class to receive parsed values:
```csharp
class Options {
  [Option('r', "read", Required = true,
    HelpText = "Input files to be processed.")]
  public IEnumerable<string> InputFiles { get; set; }

  // Omitting long name, default --verbose
  [Option(
    HelpText = "Prints all messages to standard output.")]
  public bool Verbose { get; set; }

  [Option(DefaultValue = "中文",
    HelpText = "Content language.")]
  public Language { get; set; }

  [Value(0)]
  public long? Offset { get; set;}
  }
}
```
Consume them:
```csharp
static void Main(string[] args) {
  var result = CommandLine.Parser.Default.ParseArguments<Options>(args);
  if (!result.Errors.Any()) {
    // Values are available here
    if (result.Value.Verbose) Console.WriteLine("Filenames: {0}", string.Join(",", result.Value.InputFiles.ToArray()));
  }
}
```
**F#:**
```fsharp
type options = {
  [<Option('r', "read", Required = true, HelpText = "Input files.")>] files : seq<string>;
  [<Option(HelpText = "Prints all messages to standard output.")>] verbose : bool;
  [<Option(DefaultValue = "русский", HelpText = "Content language.")>] language : string;
  [<Value(0)>] offset : int64 option;
}
```
Consume them:
```fsharp
let result = CommandLine.Parser.Default.ParseArguments<options>(args)
// Values passed to your run(o : options) function
if Seq.isEmpty result.Errors then run result.Value
else fail result.Errors
```

Acknowledgements:
---
[![Jet Brains ReSharper](/art/resharper-logo.png)](http://www.jetbrains.com/resharper/)

Thanks to JetBrains for providing an open source license for [ReSharper](http://www.jetbrains.com/resharper/).

Main Contributors (alphabetical order):
- Alexander Fast (@mizipzor)
- Dan Nemec (@nemec)
- Kevin Moore (@gimmemoore)
- Steven Evans

Resources for newcomers:
---
  - [CodePlex](http://commandline.codeplex.com)
  - [Quickstart](https://github.com/gsscoder/commandline/wiki/Quickstart)
  - [Wiki](https://github.com/gsscoder/commandline/wiki)
  - [GNU getopt](http://www.gnu.org/software/libc/manual/html_node/Getopt.html)

Latest Changes:
---
  - Implemented issue #31 (double dash), thanks also to PR #77 by Tom Glastonbury (@tg73).
  - Merged pull request #87, thanks to @randacc.
  - Merged pull request #95, thanks to Dan Nemec (@nemec).
  - Merged pull request #97, thanks to @bolha7 and @nemec.
  - Merged pull request #103, thanks to @krs43.
  - Merged pull request #119, thanks to @andrecarlucci.
  - Added F# demo (as a simple script); removed specific API (work-in-progress) from solution.
  - Merged pull request #144, thanks to @JLRishe.
  - Merged pull request #154, thanks to @txdv.
  - Implemented issue #68 (option list behaviour).
  - Fixed issue #157 (range problems).
  - Fixed issue #159 (min constraint).
  - Fixed issue #160 (max constraint).
  - Fixed issue #161 (min/max constraint for values).
  - Increased test coverage.
  - Fixed issue #149 (valid numeric input for enums).
  - Fixed issue #164 (fixed bug in required value constraint).
  - Important fix on scalar string value adjacent to string sequence (without constraints).
  - Adding more tests for token partitioners.
  - Fix in `Sequence.Partition()`.
  - `Sequence.Partition()` rewritten.
  - Refactoring for Increase Testability.
  - Little change to allow .NET 4.5 build.
  - Better `GetHashCode()` implementations.
  - New tests added.
  - Fixing FAKE build script.
  - Issue #172 fixed (`Max` constraint when `Min=Max`).
  - Merged PR #171 from @mizipzor.
  - Issue #155 Fixed (fix from @guyzeug).
  - Added support for `FSharpOption<T>` (if not used no need to distribute `FSharp.Core.dll`).
  - Disallowed `Min=Max=0` for sequences, raising exception.
  - Issue #177 Fixed.
  - Issue #112 Fixed.
  - Internal refactorings.
  - Support for **immutable types**.
  - PR #123 by @smbecker Merged.
  - Fixes.

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)
