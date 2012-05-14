Command Line Parser Library 1.9.0.7 for CLR.
===

Compatibility:
---
  - C# 3.0+ compiler
  - .NET Framework 2.0+
  - Mono 2.1+ Profile

News:
--- 
  - Test code moved to external project CommandLine.Tests.
  - All code refactored in two files:
    - CommandLine.cs (namespace CommandLine): the parser
    - CommandLineText.cs (namespace CommandLine.Text): help text builder

To build:
---
MonoDevelop or Visual Studio.

To use:
---
The project is small and well suited to be included in your application. If you don't merge it to your project tree, you must reference CommandLine.dll and import CommandLine and CommandLine.Text namespaces.
I recommend you prefer source inclusion over assembly referencing.
The help text builder (CommandLine.Text.HelpText) is not coupled with the parser, so, if you don't need it, don't include it in your project.
Anyway using HelpText class will avoid you a lot of repetitive coding.

Create a class to receive parsed values:

```csharp
    class Options {
      [Option("r", "read", Required=True, HelpText="Input file to be processed.")]
      public string InputFile { get; set; }
    
      [Option("v", "verbose", HelpText="Output all messages to standard output.")]
      public bool Verbose { get; set; }

      [HelpOption(HelpText="Display this help screen.")]
      public string GetUsage() {
        var help = new HelpText(new HeadingInfo("git-sample", "0.1"));
        help.Copyright = new CopyrightInfo("mr the author", 2005, 2012);
        help.AddPreOptionsLine("some custom stuff here");
        help.AddOptions(this);
        return help;
      }
    }
```

Add few lines to your Main method:

```csharp
    static void Main(string[] args) {
      var options = new Options();
      var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
      if (parser.ParseArguments(args, options)) {
        // Consume values here
        if (options.Verbose) Console.WriteLine("Filename: {0}", options.InputFile);
      }
    }
```

Resources for newcomers:
---
  - [CodePlex](http://commandline.codeplex.com)
  - [Quickstart](http://commandline.codeplex.com/wikipage?title=Quickstart&referringTitle=Documentation)
  - [GNU getopt](http://www.gnu.org/software/libc/manual/html_node/Getopt.html)

Author:
  - Giacomo Stelluti Scala (https://github.com/gsscoder)

Contributors:
  - Steven Evans (http://sleeplessmonkey.blogspot.it/)
  - Kevin Moore (https://github.com/gimmemoore)
