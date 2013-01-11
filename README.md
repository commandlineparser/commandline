Command Line Parser Library 1.9.4.97 Beta for CLR.
===
The Command Line Parser Library offers to CLR applications a clean and concise API for manipulating command line arguments and related tasks defining switches, options and verb commands. It allows you to display an help screen with an high degree of customization and a simple way to report syntax errors to the end user. Everything that is boring and repetitive to be programmed stands up on library shoulders, letting developers concentrate on core logic.
__The search for the command line parser for your application is over, with this library you got a solid parsing API constantly updated since 2005.__

Compatibility:
---
  - C# 3.0+ compiler
  - .NET Framework 2.0+
  - Mono 2.1+ Profile

News:
---
  - Short name of ordinary options is now defined as character.
  - Introduced support for verb commands (e.g. app verb1 --ordinary-option).
  - Internal refactoring and code reorganization for better maintainability.
  - No member with public visibility was left without XML comments.

To build:
---
MonoDevelop or Visual Studio.

To install:
---
  - NuGet way: ``Install-Package CommandLineParser``
  - XCOPY way: ``cp CommandLine/src/libcmdline/*.cs To/Your/Project/Dir``

To start:
---
  - [CSharp Template](https://github.com/gsscoder/commandline/blob/master/src/templates/CSharpTemplate/Program.cs)
  - [VB.NET Template](https://github.com/gsscoder/commandline/blob/master/src/templates/VBNetTemplate/Program.vb)

Verb Commands:
Since introduction of verb commands is a very new feature, templates and sample application are not updated to illustrate it. Please refer to unit tests code for learn how to [define](https://github.com/gsscoder/commandline/blob/master/src/tests/Mocks/OptionsWithVerbsHelp.cs), how to [respond](https://github.com/gsscoder/commandline/blob/master/src/tests/Parser/VerbsFixture.cs) and how they [relate to help subsystem](https://github.com/gsscoder/commandline/blob/master/src/tests/Text/VerbsHelpTextFixture.cs). Give a look also at this [blog article](http://gsscoder.blogspot.it/2013/01/command-line-parser-library-verb.html).

Notes:
---
The project is and well suited to be included in your application. If you don't merge it to your project tree, you must reference ```CommandLine.dll``` and import ```CommandLine``` and ```CommandLine.Text``` namespaces (or install via NuGet). The help text builder and its support types lives in ```CommandLine.Text``` namespace that is not coupled with the parser, so, if you don't need it, you can exclude it from your project. Anyway using ```HelpText``` class will avoid you a lot of repetitive coding. Finally also the new feature of verb commands con be excluded removing ```libcmdline/verb``` directory.

Create a class to receive parsed values:

```csharp
    class Options : CommandLineOptionsBase {
      [Option('r', "read", Required = true,
        HelpText = "Input file to be processed.")]
      public string InputFile { get; set; }
    
      [Option('v', "verbose", DefaultValue = true,
        HelpText = "Prints all messages to standard output.")]
      public bool Verbose { get; set; }

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
      if (CommandLineParser.Default.ParseArguments(args, options)) {
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

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)
