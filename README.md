Command Line Parser Library for CLR.
===

Compatibility:
---
  - C# 3.0+ compiler
  - .NET Framework 2.0+
  - Mono 2.1+ Profile

### To build:

Unix/Macintosh:

    ./configure
    make
    sudo make install

### To use:

The project is small and well suited to be included in your application. If you don't merge it to your project tree, you must reference CommandLine.dll and import CommandLine and CommandLine.Text namespaces.

Create a class to receive parsed values:

    class Options {
      [Option("r", "read", Required=True, HelpText="Input file to be processed.")]
      public string InputFile = "";
    
      [Option("v", "verbose", HelpText="Output all messages to standard output.")]
      public bool Verbose = false;

      [HelpOption(HelpText="Display this help screen.")]
      public string GetUsage() {
        var help = new HelpText(new HeadingInfo("git-sample", "0.1"));
        help.Copyright = new CopyrightInfo("mr the author", 2005, 2012);
        help.AddPreOptionsLine("some custom stuff here");
        help.AddOptions(this);
        return help;
      }
    }

Add few lines to your Main method:

    static void Main(string[] args) {
      var options = new Options();
      var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
      if (parser.ParseArguments(args, options)) {
        // Consume values here
        if (options.Verbose) Console.WriteLine("Filename: {0}", options.InputFile);
      }
    }

Notes on installation:
---
The installation by default copy the library (CommandLine.dll) and the sample (SampleApp.exe) to /usr/local/commandline. It will not installs the assembly in the GAC. The make step will build all binaries in standard project folders (/src/libcmdline/bin/Release/ and /src/sample/sample/bin/Release).

Resources for newcomers:
---
  - [CodePlex](http://commandline.codeplex.com)
  - [Quickstart](http://commandline.codeplex.com/wikipage?title=Quickstart&referringTitle=Documentation)
  - [GNU getopt](http://www.gnu.org/software/libc/manual/html_node/Getopt.html)
