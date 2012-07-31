Command Line Parser Library 1.9.3.19 for CLR.
===
The Command Line Parser Library offers to CLR applications a clean and concise API for manipulating command line arguments and related tasks.
It allows you to display an help screen with an high degree of customization and a simple way to report syntax errors to the user.
Everything that is boring and repetitive to be programmed stands up on library shoulders, letting you concentrate yourself on core logic.
__The search for the command line parser for your application is over, with this library you got a solid parsing API constantly updated since 2005.__

Compatibility:
---
  - C# 3.0+ compiler
  - .NET Framework 2.0+
  - Mono 2.1+ Profile

News:
---
  - Target of projects set to Mono 4.0 Profile (but stills compatible with 2.0).
  - Fixed the LongOptionParser type problem of parsing two consecutive arrays (issue from m1gu3l github user). 
  - Fixed issue in ReflectionUtil::GetAttribute() / HelpText::AutoBuild().
  - Merged code from Kevin Moore for handle unknown arguments.
  - Introduced AssemblyLicenseAttribute and AssemblyUsageAttribute for use with HelpText::AutoBuild().

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

Notes:
---
The project is small and well suited (or better thought) to be included in your application. If you don't merge it to your project tree, you must reference CommandLine.dll and import CommandLine and CommandLine.Text namespaces (or install via NuGet).
I recommend you source inclusion over assembly referencing.
The help text builder (CommandLine.Text.HelpText) is not coupled with the parser, so, if you don't need it, don't include it in your project.
Anyway using HelpText class will avoid you a lot of repetitive coding.

Create a class to receive parsed values:

```csharp
    class Options : CommandLineOptionsBase {
      [Option("r", "read", Required = true,
        HelpText = "Input file to be processed.")]
      public string InputFile { get; set; }
    
      [Option("v", "verbose", DefaultValue = true,
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
