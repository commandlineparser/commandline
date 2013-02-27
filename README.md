Command Line Parser Library 1.9.71.2 stable for CLR.
===
The Command Line Parser Library offers to CLR applications a clean and concise API for manipulating command line arguments and related tasks defining switches, options and verb commands. It allows you to display an help screen with an high degree of customization and a simple way to report syntax errors to the end user. Everything that is boring and repetitive to be programmed stands up on library shoulders, letting developers concentrate on core logic.
__This library provides _hassle free_ command line parsing with a constantly updated API since 2005.__

Compatibility:
---
  - .NET Framework 3.5+
  - Mono 2.1+ Profile

At glance:
---
  - One line parsing using default singleton: ``CommandLine.Parser.Default.ParseArguments(...)``.
  - One line help screen generator: ``HelpText.AutoBuild(...)``.
  - Map command line arguments to ``IList<string>``, arrays, enum or standard scalar types.
  - __Plug-In friendly__ architecture as explained [here](https://github.com/gsscoder/commandline/wiki/Plug-in-Friendly-Architecture).
  - Define [verb commands](https://github.com/gsscoder/commandline/wiki/Verb-Commands) as ``git commit -a``.
  - Create parser instance using lambda expressions.
  - Most of features applies with a [CoC](http://en.wikipedia.org/wiki/Convention_over_configuration) philosophy.

To install:
---
  - NuGet way (latest stable): ``Install-Package CommandLineParser``
  - NuGet way (latest version): ``Install-Package CommandLineParser -pre``
  - XCOPY way: ``cp -r CommandLine/src/libcmdline To/Your/Project/Dir``

To build:
---
You can use still use MonoDevelop or Visual Studio, but the project can aslo be built using Ruby [Rake](http://rake.rubyforge.org/) with a script that depends on [Albacore](https://github.com/derickbailey/Albacore).
```
$ gem install albacore
$ git clone https://github.com/gsscoder/commandline.git CommandLine
$ cd CommandLine
$ rake
```

To start:
---
  - [CSharp Template](https://github.com/gsscoder/commandline/blob/master/src/templates/CSharpTemplate/Program.cs)
  - [VB.NET Template](https://github.com/gsscoder/commandline/blob/master/src/templates/VBNetTemplate/Program.vb)

Public API:
---
Latest changes are recorded from Version 1.9.4.91, please refer to [this document](https://github.com/gsscoder/commandline/blob/master/doc/PublicAPI.md).

Verb Commands:
---
Since introduction of verb commands is a very new feature, templates and sample application are not updated to illustrate it. Please refer this [wiki section](https://github.com/gsscoder/commandline/wiki/Verb-Commands) and unit tests code to learn how to [define](https://github.com/gsscoder/commandline/blob/master/src/tests/Mocks/OptionsWithVerbsHelp.cs), how to [respond](https://github.com/gsscoder/commandline/blob/master/src/tests/Parser/VerbsFixture.cs) and how they [relate to help subsystem](https://github.com/gsscoder/commandline/blob/master/src/tests/Text/VerbsHelpTextFixture.cs). Give a look also at this [blog article](http://gsscoder.blogspot.it/2013/01/command-line-parser-library-verb.html).

Notes:
---
The project is and well suited to be included in your application. If you don't merge it to your project tree, you must reference ``CommandLine.dll`` and import ``CommandLine`` and ``CommandLine.Text`` namespaces (or install via NuGet). The help text builder and its support types lives in ``CommandLine.Text`` namespace that is loosely coupled with the parser. However is good to know that ``HelpText`` class will avoid a lot of repetitive coding.

Define a class to receive parsed values:

```csharp
class Options {
  [Option('r', "read", Required = true,
    HelpText = "Input file to be processed.")]
  public string InputFile { get; set; }
    
  // omitting long name, default --verbose
  [Option(DefaultValue = true,
    HelpText = "Prints all messages to standard output.")]
  public bool Verbose { get; set; }

  [ParserState]
  public IParserState LastParserState { get; set; }

  [HelpOption]
  public string GetUsage() {
    return HelpText.AutoBuild(this,
      (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
  }
}
```

Consume them:

```csharp
static void Main(string[] args) {
  var options = new Options();
  if (CommandLine.Parser.Default.ParseArguments(args, options)) {
    // Values are available here
    if (options.Verbose) Console.WriteLine("Filename: {0}", options.InputFile);
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
  - Promoted to stable.
  - Implicit name is now available on ``OptionAttribute`` and ``OptionListAttribute``.
  - Fixing version numeration error.

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)
