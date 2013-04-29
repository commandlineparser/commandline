Command Line Parser Library 2.0.0.0 pre-release for CLR.
===
The Command Line Parser Library offers to CLR applications a clean and concise API for manipulating command line arguments and related tasks defining switches, options and verb commands. It allows you to display an help screen with an high degree of customization and a simple way to report syntax errors to the end user. Everything that is boring and repetitive to be programmed stands up on library shoulders, letting developers concentrate on core logic.
__This library provides _hassle free_ command line parsing with a constantly updated API since 2005.__

Compatibility:
---
  - .NET Framework 3.5+
  - Mono 2.1+ Profile

Current Release:
---
  - This is a __pre-release__, for documentation please read appropriate [wiki section](https://github.com/gsscoder/commandline/wiki/Latest-Beta).

At glance:
---
  - One line parsing using default singleton: ``CommandLine.Parser.Default.ParseArguments(...)``.
  - One line help screen generator: ``HelpText.AutoBuild(...)``.
  - Map command line arguments to sequences (``IEnumerable<T>``), enum or standard scalar types.
  - __Plug-In friendly__ architecture as explained [here](https://github.com/gsscoder/commandline/wiki/Plug-in-Friendly-Architecture).
  - Define [verb commands](https://github.com/gsscoder/commandline/wiki/Verb-Commands) as ``git commit -a``.
  - Most of features applies with a [CoC](http://en.wikipedia.org/wiki/Convention_over_configuration) philosophy.
  - F# specific API (work in progress).

To install:
---
  - NuGet way (latest stable): ``Install-Package CommandLineParser``
  - NuGet way (latest version): ``Install-Package CommandLineParser -pre``
  - XCOPY way: ``cp -r CommandLine/src/libcmdline To/Your/Project/Dir``

To build:
---
MonoDevelop or Visual Studio

Public API:
---
Latest changes are recorded from Version 1.9.4.91, please refer to [this document](https://github.com/gsscoder/commandline/blob/master/doc/PublicAPI.md).

Notes:
---
The project is and well suited to be included in your application. If you don't merge it to your project tree, you must reference ``CommandLine.dll`` and import ``CommandLine`` and ``CommandLine.Text`` namespaces (or install via NuGet). The help text builder and its support types lives in ``CommandLine.Text`` namespace that is loosely coupled with the parser. However is good to know that ``HelpText`` class will avoid a lot of repetitive coding.

Define a class to receive parsed values:

```csharp
class Options {
  [Option('r', "read", Required = true,
    HelpText = "Input files to be processed.")]
  public IEnumerable<string> InputFiles { get; set; }
    
  // omitting long name, default --verbose
  [Option(DefaultValue = true,
    HelpText = "Prints all messages to standard output.")]
  public bool Verbose { get; set; }

  [Value(0)]
  public int Offset { get; set;}
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

Acknowledgements:
---
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

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)
