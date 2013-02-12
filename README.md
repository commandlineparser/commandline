Command Line Parser Library 1.9.4.221 beta for CLR.
===
The Command Line Parser Library offers to CLR applications a clean and concise API for manipulating command line arguments and related tasks defining switches, options and verb commands. It allows you to display an help screen with an high degree of customization and a simple way to report syntax errors to the end user. Everything that is boring and repetitive to be programmed stands up on library shoulders, letting developers concentrate on core logic.
__The search for the command line parser for your application is over, with this library you got a solid parsing API constantly updated since 2005.__

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
  - Create parser instance using lambda expressions with __fluent builder__.

To install:
---
  - NuGet way (latest stable): ``Install-Package CommandLineParser``
  - NuGet way (latest version): ``Install-Package CommandLineParser -pre``
  - XCOPY way: ``cp CommandLine/src/libcmdline/*.cs To/Your/Project/Dir``

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

Create a class to receive parsed values:

```csharp
    class Options {
      [Option('r', "read", Required = true,
        HelpText = "Input file to be processed.")]
      public string InputFile { get; set; }
    
      [Option('v', "verbose", DefaultValue = true,
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

Add few lines to your Main method:

```csharp
    static void Main(string[] args) {
      var options = new Options();
      if (CommandLine.Parser.Default.ParseArguments(args, options)) {
        // Consume values here
        if (options.Verbose) Console.WriteLine("Filename: {0}", options.InputFile);
      }
    }
```

Acknowledgements:
---
I want to thank all the people who in recent years have taken an interest in this project here on GitHub, on CodePlex and also those who contacted me directly. In particular Steven Evans for improving the help subsystem, Kevin Moore that has introduced a plugin friendly architecture and finally Dan Nemec that with its contribution has made possible the introduction of verb commands from version 1.9.4.91. Thanks also to JetBrains for providing an open source license for [ReSharper](http://www.jetbrains.com/resharper/).

Resources for newcomers:
---
  - [CodePlex](http://commandline.codeplex.com)
  - [Quickstart](https://github.com/gsscoder/commandline/wiki/Quickstart)
  - [Wiki](https://github.com/gsscoder/commandline/wiki)
  - [GNU getopt](http://www.gnu.org/software/libc/manual/html_node/Getopt.html)

Latest Changes: 
---
  - Merged pull request #52 from @mizipzor (Alexander Fast).
  - Extracted interface ``IParserSettings`` from ``ParserSettings``; see [Public API](https://github.com/gsscoder/commandline/blob/master/doc/PublicAPI.md) document.
  - Prefix ``CommandLine`` removed from main types to achieve name consistency between all library types.
  - Removed compilation symbol UNIT_TESTS and DebugTests configuration.
  - All names of test fixture methods changed to a new convention (using https://gist.github.com/4655503).
  - Started refactoring on test project (migrating from NUnit to Xunit).
  - Thanks to @marcells, we can reference CommandLine.dll from CommandLine.Tests.dll keeping strong name.
  - Introduced ``ValueOptionAttribute`` enhancement of issue #33.
  - ``CommandLineParser`` refactored (also using new ``ParserContext`` type).
  - ``ReflectionUtil`` now caches data using ``ReflectionCache``.
  - Internal refactoring on ``OptionMap`` and ``OptionInfo``.
  - Refactoring in respect of FxCop rules (see ChangeLog). ``HandleParsingErrorsDelegate`` renamed to ``ParsingErrorsHandler``, ``MultiLineTextAttribute`` renamed to ``MultilineTextAttribute``.
  - Removed synchronization from ``OptionInfo`` and ``TargetWrapper`` (parsing should occur in one thread;
      if not, synchronization must be provided by developer not by the library).
  - Merged pull request #44 from @dbaileychess (Derek Bailey) that adds ``BaseOptionAttribute::MetaKey`` similar to python [argparse](http://docs.python.org/2/library/argparse.html#module-argparse).
  - Implemented [strict parsing](https://github.com/gsscoder/commandline/blob/master/src/tests/Parser/StrictFixture.cs) (see issue #32 by @nemec).

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)
