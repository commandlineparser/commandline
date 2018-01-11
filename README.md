[![Build status](https://img.shields.io/appveyor/ci/commandlineparser/commandline.svg)](https://ci.appveyor.com/project/commandlineparser/commandline)
[![Nuget](https://img.shields.io/nuget/dt/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)
[![Nuget](https://img.shields.io/nuget/v/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)
[![Nuget](https://img.shields.io/nuget/vpre/commandlineparser.svg)](http://nuget.org/packages/commandlineparser)
[![Join the gitter chat!](https://badges.gitter.im/gsscoder/commandline.svg)](https://gitter.im/gsscoder/commandline?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

# Command Line Parser Library for CLR and NetStandard

**Note:** the API surface has changed since v1.9.x and earlier. If you are looking for documentation on v1.9.x, please see [stable-1.9.71.2](https://github.com/gsscoder/commandline/tree/stable-1.9.71.2)

The Command Line Parser Library offers CLR applications a clean and concise API for manipulating command line arguments and related tasks, such as defining switches, options and verb commands. It allows you to display a help screen with a high degree of customization and a simple way to report syntax errors to the end user.

```
C:\Project> Nuget Install CommandLineParser
```

_NOTE: Mentioned F# Support is provided via ```CommandLineParser.FSharp``` package with FSharp dependencies._

__This library provides _hassle free_ command line parsing with a constantly updated API since 2005.__

# At a glance:

- Compatible with __.NET Framework 4.0+__, __Mono 2.1+ Profile__, and __.Net Core__
- Doesn't depend on other packages (No dependencies beyond standard base libraries)
- One line parsing using default singleton: `CommandLine.Parser.Default.ParseArguments(...)`.
- Automatic or one line help screen generator: `HelpText.AutoBuild(...)`.
- Supports `--help`, `--version`, `version` and `help [verb]` by default.
- Map to sequences (via `IEnumerable<T>` and similar) and scalar types, including Enums and `Nullable<T>`.
- You can also map to every type with a constructor that accepts a string (like `System.Uri`).
- __Plug-In friendly__ architecture as explained [here](https://github.com/gsscoder/commandline/wiki/Plug-in-Friendly-Architecture).
- Define [verb commands](https://github.com/gsscoder/commandline/wiki/Latest-Version#verbs) similar to `git commit -a`.
- Unparsing support: `CommandLine.Parser.Default.FormatCommandLine<T>(T options)`.
- CommandLineParser.FSharp package is F#-friendly with support for `option<'a>`, see [demo](https://github.com/gsscoder/commandline/blob/master/demo/fsharp-demo.fsx).  _NOTE: This is a separate Nuget package._
- Most of features applies with a [CoC](http://en.wikipedia.org/wiki/Convention_over_configuration) philosophy.
- C# demo: source [here](https://github.com/commandlineparser/commandline/tree/master/demo/ReadText.Demo).

Used by several open source projects and by various commercial products: See the [wiki for listing](https://github.com/gsscoder/commandline/Used_By)

# Getting Started with the Command Line Parser Library

You can utilize the parser library in several ways:

- Install via Nuget/Paket
- Integrate directly into your project by copying the .cs files into your project.
- ILMerge during your build process.

See more details in the [wiki for direct integrations](https://github.com/gsscoder/commandline/wiki/Direct_Integrations)

## Quick Start Examples

1. Create a class to define valid options, and to receive the parsed options.
2. Call ParseArguments with the args string array.

C# Examples:

```csharp
internal class Options {
  [Option('r',"read", 
	Required = true,
	HelpText = "Input files to be processed.")]
  public IEnumerable<string> InputFiles { get; set; }

  // Omitting long name, defaults to name of property, ie "--verbose"
  [Option(
	DefaultValue = false,
	HelpText = "Prints all messages to standard output.")]
  public bool Verbose { get; set; }
  
  [Option("stdin",
	DefaultValue = false
	HelpText = "Read from stdin")]
   public bool stdin { get; set; }

  [Value(0, MetaName = "offset",
	HelpText = "File offset.")]
  public long? Offset { get; set; }
}

static int Main(string[] args) {
  var options = new Options();
  var isValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);
```

F# Examples:

```fsharp
type options = {
  [<Option('r', "read", Required = true, HelpText = "Input files.")>] files : seq<string>;
  [<Option(HelpText = "Prints all messages to standard output.")>] verbose : bool;
  [<Option(DefaultValue = "русский", HelpText = "Content language.")>] language : string;
  [<Value(0, MetaName="offset", HelpText = "File offset.")>] offset : int64 option;
}

let main argv =
  let result = CommandLine.Parser.Default.ParseArguments<options>(argv)
  match result with
  | :? Parsed<options> as parsed -> run parsed.Value
  | :? NotParsed<options> as notParsed -> fail notParsed.Errors
```

VB.Net:

```VB.NET
Class Options
	<CommandLine.Option('r', "read", Required := true,
	HelpText:="Input files to be processed.")>
	Public Property InputFiles As IEnumerable(Of String)

	' Omitting long name, defaults to name of property, ie "--verbose"
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

'TODO
```

### For verbs:

1. Create separate option classes for each verb.  An options base class is supported.  
2. Call ParseArguments with all the verb attribute decorated options classes.
3. Use MapResult to direct program flow to the verb that was parsed.

C# example:

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

VB.Net example:

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

F# Example:

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

For additional examples, check the [wiki for additional examples](https://gsscoder/commandline/wiki/Examples)

Acknowledgements:
---
[![Jet Brains ReSharper](/art/resharper-logo.png)](http://www.jetbrains.com/resharper/)

Thanks to JetBrains for providing an open source license for [ReSharper](http://www.jetbrains.com/resharper/).

# Contibutors
First off, _Thank you!_  All contributions are welcome.  

Please consider sticking with the GNU getopt standard for command line parsing.  

Additionally, for easiest diff compares, please follow the project's tabs settings.  Utilizing the EditorConfig extension for Visual Studio/your favorite IDE is recommended.

__And most importantly, please target the ```develop``` branch in your pull requests!__

For more info, see the [wiki for details about contributing](https://github.com/gsscoder/commandline/wiki/Building_the_library) and for building the project.

## Main Contributors (alphabetical order):
- Alexander Fast (@mizipzor)
- Dan Nemec (@nemec)
- Kevin Moore (@gimmemoore)
- Steven Evans
- Thomas Démoulins (@Thilas)

## Resources for newcomers:

- [Quickstart](https://github.com/gsscoder/commandline/wiki/Quickstart)
- [Wiki](https://github.com/gsscoder/commandline/wiki)
- [GNU getopt](http://www.gnu.org/software/libc/manual/html_node/Getopt.html)

# Contacts:

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
