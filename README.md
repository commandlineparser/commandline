[![Build status](https://img.shields.io/appveyor/ci/gsscoder/commandline.svg)](https://ci.appveyor.com/project/gsscoder/commandline)
[![Nuget](https://img.shields.io/nuget/dt/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)
[![Nuget](https://img.shields.io/nuget/v/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)
[![Nuget](https://img.shields.io/nuget/vpre/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)

Command Line Parser Library 2.0.227.0 beta for CLR.
===
The Command Line Parser Library offers CLR applications a clean and concise API for manipulating command line arguments and related tasks, such as defining switches, options and verb commands. It allows you to display a help screen with a high degree of customization and a simple way to report syntax errors to the end user.

Everything that is boring and repetitive about parsing command line arguments is delegated to the library, letting developers concentrate on core logic. It's written in **C#** and doesn't depend on other packages.

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
    - Supports `--help`, `--version`, `version` and `help [verb]` by default.
  - Map to sequences (``IEnumerable<T>``) or scalar types, including enum and ``Nullable<T>``.
  - You can also map to every type with a constructor that accepts a string (like ``System.Uri``).
  - __Plug-In friendly__ architecture as explained [here](https://github.com/gsscoder/commandline/wiki/Plug-in-Friendly-Architecture).
  - Define [verb commands](https://github.com/gsscoder/commandline/wiki/Latest-Version#verbs) as ``git commit -a``.
  - Unparsing support: ``CommandLine.Parser.Default.FormatCommandLine<T>(T options)``.
  - F#-friendly with support for ``option<'a>``, see [demo](https://github.com/gsscoder/commandline/blob/master/demo/fsharp-demo.fsx).
  - Most of features applies with a [CoC](http://en.wikipedia.org/wiki/Convention_over_configuration) philosophy.
  - C# demo: source [here](https://github.com/gsscoder/commandline/tree/master/demo/ReadText.Demo).

To install:
---
  - NuGet way (latest stable): ``Install-Package CommandLineParser``
  - NuGet way (latest version): ``Install-Package CommandLineParser -pre``
  - XCOPY way: ``cp -r ClonedRepo/src/CommandLine To/Your/Project/Dir``

To build:
---
- [FAKE](http://fsharp.github.io/FAKE/) Script
- MS Visual Studio
- Xamarin Studio

Public API:
---
Latest changes are recorded from Version 1.9.4.91, please refer to [this document](https://github.com/gsscoder/commandline/blob/master/docs/PublicAPI.md).

Used by:
---
- [FSharp.Formatting](https://github.com/tpetricek/FSharp.Formatting) by @tpetricek.

Notes:
---
The project is well suited to be included in your application. If you don't merge it to your project tree, you must reference ``CommandLine.dll`` and import ``CommandLine`` and ``CommandLine.Text`` namespaces (or install via NuGet). The help text builder and support types are in the ``CommandLine.Text`` namespace that is loosely coupled with the parser. It is good to know that the ``HelpText`` class will avoid a lot of repetitive coding.

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

  [Option(Default = "中文",
    HelpText = "Content language.")]
  public Language { get; set; }

  [Value(0, MetaName = "offset",
    HelpText = "File offset.")]
  public long? Offset { get; set;}
  }
}
```
Consume them:
```csharp
static int Main(string[] args) {
  var result = CommandLine.Parser.Default.ParseArguments<Options>(args);
  var exitCode = result
    .Return(
      options = > {
        if (options.Verbose) Console.WriteLine("Filenames: {0}", string.Join(",", options.InputFiles.ToArray()));
        return 0; },
      errors => {
	    LogHelper.Log(errors);
	    return 1; });
  return exitCode;
}
```
**F#:**
```fsharp
type options = {
  [<Option('r', "read", Required = true, HelpText = "Input files.")>] files : seq<string>;
  [<Option(HelpText = "Prints all messages to standard output.")>] verbose : bool;
  [<Option(Default = "русский", HelpText = "Content language.")>] language : string;
  [<Value(0, MetaName="offset", HelpText = "File offset.")>] offset : int64 option;
}
```
Consume them:
```fsharp
let main argv = 
  let result = CommandLine.Parser.Default.ParseArguments<options>(argv)
  match result with
  | :? Parsed<options> as parsed -> run parsed.Value
  | :? NotParsed<options> as notParsed -> fail notParsed.Errors
```

For verbs:

**C#:**
```csharp
[Verb("add", HelpText = "Add file contents to the index.")]
class AddOptions {
  //normal options here
}
[Verb("commit", HelpText = "Record changes to the repository.")]
class CommitOptions {
  //normal options here
}
[Verb("clone", HelpText = "Clone a repository into a new directory.")]
class CloneOptions {
  //normal options here
}

int Main(string[] args) {
  return CommandLine.Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions>(args)
    .Return(
      (AddOptions opts) => RunAddAndReturnExitCode(opts),
      (CommitOptions opts) => RunCommitAndReturnExitCode(opts),
      (CloneOptions opts) => RunCloneAndReturnExitCode(opts),
      errs => 1);
}
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
- Thomas Démoulins (@Thilas)

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
  - Support for **immutable types**.
  - PR #123 by @smbecker Merged.
  - Fixes.
  - Issue #179 Implemented (`Value|Option.DefaultValue` -> `Default`).
  - Issue #180 Implemented (better `ParserResult<T>` model).
  - Issue #181 Implemented.
  - Added `Return<TSource,TResult>(...)` to `ParserResult<TSource>`.
  - Issue #183 (option list in verb scenario) Fixed by @Thilas.
  - Issue #183 (immutable in verb scenario) reported by @Thilas Fixed.
  - Default `--help` command refactored.
  - Added `WithParsed<T>()` for verbs.
  - Added `Return<T1...T16>()` for verbs.
  - Automatic `--version` handling.
  - Added C# demo.
  - Issue #189 Implemented.
  - Issue #190 Fixed/Implemented (`--help`/`--version`/`version` for verbs).
  - Issue #188 (reported by @diversteve) Fixed.
  - Issue #191 (`--version`/`version` added to help screen) Fixed.
  - Issue #162 (`ValueAttribute` handled in help screen) Implemented.
  - PR #197 (by @Thilas) Implemented.
  - Issue #202 (reported by @StevenLiekens) Fixed.
  - Managing deps with Paket.
  - Issue #203 Implemented.
  - Issue #204 (reported by @Kimi-Arthur) Fixed.
  - PR #205 (by @forki) Merged.
  - Refactoring with `RailwaySharp.ErrorHandling`.
  - Test project refactoring.
  - Issue #186 Implemented: Adding unparse support.
  - PR #207 Merged.
  - Using new Paket-friendly CSharpx and RailwaySharp.
  - Added F# option support to `FormatCommandLine()`.
  - `ParserResult<T>` internally refactored, minor breaking changes to `HelpText`.
  - Added `FormatCommandLine()` overload with settings.
  - Issue #208 Implemented (extended usage text support).
  - Internal/external refactorings.
  - Minor `HelpText` refactorings.
  - Issue #210 (reported by @mac2000) Implemented.
  - Test refactorings.
  - Fixing XML comments.
  - Changes in usage text handling.
  - Issue #65 (re) Fixed for 2.0.x library.
  - Issue #218 Fixed (IgnoreUnknownArguments).
  - PR #215 (by @Thilas) Merged.

Contact:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)
