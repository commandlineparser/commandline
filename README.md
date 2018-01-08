[![Build status](https://img.shields.io/appveyor/ci/commandlineparser/commandline.svg)](https://ci.appveyor.com/project/commandlineparser/commandline)
[![Nuget](https://img.shields.io/nuget/dt/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)
[![Nuget](https://img.shields.io/nuget/v/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)
[![Nuget](https://img.shields.io/nuget/vpre/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)
[![Join the gitter chat!](https://badges.gitter.im/gsscoder/commandline.svg)](https://gitter.im/gsscoder/commandline?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Command Line Parser Library for CLR and NetStandard
===

**Note:** the API surface has changed since 1.9.x and earlier. If you are looking for documentation on 1.9.x, please see [this branch](https://github.com/gsscoder/commandline/tree/stable-1.9.71.2)

The Command Line Parser Library offers CLR applications a clean and concise API for manipulating command line arguments and related tasks, such as defining switches, options and verb commands. It allows you to display a help screen with a high degree of customization and a simple way to report syntax errors to the end user.

```
C:\Project> Nuget Install CommandLineParser

or

C:\Project> paket install CommandLineParser
```

Everything that is boring and repetitive about parsing command line arguments is delegated to the library, letting developers concentrate on core logic. It's written in **C#** and doesn't depend on other packages.

__This library provides _hassle free_ command line parsing with a constantly updated API since 2005.__

Compatibility:
---
  - .NET Framework 4.0+
  - Mono 2.1+ Profile
  - .Net Core

Current Release:
---
  - For documentation please read appropriate [wiki section](https://github.com/gsscoder/commandline/wiki/Latest-Version). 
  - From version **2.0.x-pre+** parsing kernel was rewritten and public API simplified.

At glance:
---
  - One line parsing using default singleton: `CommandLine.Parser.Default.ParseArguments(...)`.
  - Automatic or one line help screen generator: `HelpText.AutoBuild(...)`.
    - Supports `--help`, `--version`, `version` and `help [verb]` by default.
  - Map to sequences (`IEnumerable<T>`) or scalar types, including enum and `Nullable<T>`.
  - You can also map to every type with a constructor that accepts a string (like `System.Uri`).
  - __Plug-In friendly__ architecture as explained [here](https://github.com/gsscoder/commandline/wiki/Plug-in-Friendly-Architecture).
  - Define [verb commands](https://github.com/gsscoder/commandline/wiki/Latest-Version#verbs) as `git commit -a`.
  - Unparsing support: `CommandLine.Parser.Default.FormatCommandLine<T>(T options)`.
  - F#-friendly with support for `option<'a>`, see [demo](https://github.com/gsscoder/commandline/blob/master/demo/fsharp-demo.fsx).
  - Most of features applies with a [CoC](http://en.wikipedia.org/wiki/Convention_over_configuration) philosophy.
  - C# demo: source [here](https://github.com/gsscoder/commandline/tree/master/demo/ReadText.Demo).

Integrate directly into your project
---
It is possible to integrate the CommandLineParser library directly into your project in two ways:

First way is simply copy the .cs files into your project:
```
C:\Projects\MyProject> cp -r ClonedRepo/src/CommandLine To/Your/Project/Dir
```

You can also use ILMerge during your library build process:

```
C:\Projects\MyProject> msbuild MyProject.sln /p:Configuration=Release
C:\Projects\MyProject> ilmerge bin\Release\MyProject.exe bin\Release\CommandLineParser.dll bin\Release\MyProject.merged.exe
```

To build:
---
- [FAKE](http://fsharp.github.io/FAKE/) Script
- MS Visual Studio
- Xamarin Studio

Public API:
---
Latest changes are recorded from Version 1.9.4.91, please refer to [this document](https://github.com/commandlineparser/commandline/blob/master/docs/PublicAPI.md).

Used by:
---
- [FSharp.Formatting](https://github.com/tpetricek/FSharp.Formatting) by @tpetricek.
- [MiniDumper](https://github.com/goldshtn/minidumper) by @goldshtn.
- [Google APIs Client Library for .NET](https://github.com/google/google-api-dotnet-client) by Google.
- [FSpec](https://github.com/PeteProgrammer/fspec) by @PeteProgrammer.
- Various commercial products.

Notes:
---
The project is well suited to be included in your application. If you don't merge it to your project tree, you must reference `CommandLine.dll` and import `CommandLine` and `CommandLine.Text` namespaces (or install via NuGet). The help text builder and support types are in the `CommandLine.Text` namespace that is loosely coupled with the parser. It is good to know that the `HelpText` class will avoid a lot of repetitive coding.

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
  public string Language { get; set; }

  [Value(0, MetaName = "offset",
    HelpText = "File offset.")]
  public long? Offset { get; set; }
}
```
Consume them:
```csharp
static int Main(string[] args) {
  var options = new Options();
  var isValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);
```
**F#:**
```fsharp
type options = {
  [<Option('r', "read", Required = true, HelpText = "Input files.")>] files : seq<string>;
  [<Option(HelpText = "Prints all messages to standard output.")>] verbose : bool;
  [<Option(DefaultValue = "русский", HelpText = "Content language.")>] language : string;
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
**VB.NET:**
```VB.NET
Class Options
	<CommandLine.Option('r', "read", Required := true,
	HelpText:="Input files to be processed.")>
	Public Property InputFiles As IEnumerable(Of String)

    ' Omitting long name, default --verbose
    <CommandLine.Option(
	HelpText:="Prints all messages to standard output.")>
	Public Property Verbose As Boolean

	<CommandLine.Option(DefaultValue:="中文",
	HelpText:="Content language.")>
	Public Property Language As String

	<CommandLine.Value(0, MetaName:="offset",
	HelpText:="File offset.")>
	Public Property Offset As Long?
End Class
```
Consume them:
```VB.NET
TODO
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
    .MapResult(
      (AddOptions opts) => RunAddAndReturnExitCode(opts),
      (CommitOptions opts) => RunCommitAndReturnExitCode(opts),
      (CloneOptions opts) => RunCloneAndReturnExitCode(opts),
      errs => 1);
}
```

**F#:**
```fsharp
open CommandLine

[<Verb("add", HelpText = "Add file contents to the index.")>]
type AddOptions = {
  // normal options here
}
[<Verb("commit", HelpText = "Record changes to the repository.")>]
type CommitOptions = {
  // normal options here
}
[<Verb("clone", HelpText = "Clone a repository into a new directory.")>]
type CloneOptions = {
  // normal options here
}

[<EntryPoint>]
let main args =
  let result = Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions> args
  match result with
  | :? CommandLine.Parsed<obj> as command ->
    match command.Value with
    | :? AddOptions as opts -> RunAddAndReturnExitCode opts
    | :? CommitOptions as opts -> RunCommitAndReturnExitCode opts
    | :? CloneOptions as opts -> RunCloneAndReturnExitCode opts
  | :? CommandLine.NotParsed<obj> -> 1
```
**VB.NET:**
```VB.NET
<CommandLine.Verb("add", HelpText:="Add file contents to the index.")>
Public Class AddOptions
	'Normal options here
End Class
<CommandLine.Verb("commit", HelpText:="Record changes to the repository.")>
Public Class AddOptions
	'Normal options here
End Class
<CommandLine.Verb("clone", HelpText:="Clone a repository into a new directory.")>
Public Class AddOptions
	'Normal options here
End Class

Public Shared Sub Main()
	'TODO
End Sub
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

Contact:
---
- Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com (_use this for everything that is not available via GitHub features_)
  - GitHub: [gsscoder](https://github.com/gsscoder)
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)
- Dan Nemec
- Eric Newton
  - ericnewton76+commandlineparser AT gmail DOT com
  - GitHub: [ericnewton76](https://github.com/ericnewton76)
  - Blog: 
  - Twitter: [enorl76](http://twitter.com/enorl76)
