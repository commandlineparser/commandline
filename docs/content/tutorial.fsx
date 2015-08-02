(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../build"
#r "CommandLine.dll"

open System

(**

# Tutorial

## Introduction

The library parses command line arguments to a record decorated with attributes:

*)

type options = {
  [<Value(0, MetaName = "ROOT", Required = true, HelpText = "Root directory.")>] root : string;
  [<Option('i', "index", Required = true, HelpText = "Valid index files.")>] indexFiles : seq<string>;
  [<Option(HelpText = "Describe every operation.")>] verbose : bool;
  [<Option("timeout", HelpText = "Session timeout in seconds.")>] timeout : int option;
  [<Option('e', HelpText = "Error file.")] errorFile : string;
  [<Option('x', HelpText = "Run in debug mode.")] debugMode : bool;
}

(**

Previous record defines the above parsing schemes:

    [lang=bash]
    ./www -i index.html index.py --verbose --timeout 2 -e ./log/err.log -x
    ./www -i index.html index.py --verbose --timeout 2 -xe./log/err.log

As you can see using attributes you can model your syntax using short name, long name or both,
following *nix [getopt](http://man7.org/linux/man-pages/man3/getopt.3.html) specification.

and it will be used to build up this record instance:
*)

{root = "./www"; indexFiles = seq {yield "index.html"; yield "index.py"}; verbose = true;
 timeout = Some 2; errorFile = "./log/err.log"; debugMode = true}

(**

CommandLineParser supports mandatory options without name (called values, as options.root) via Required property,
mandatory options and values and mutually exclusive options (more on this later).

## Basic Usage

Parsing a complex command line syntax is easy as defining a record with attributes and calling a single method:
*)

open CommandLine

type options = {
  [<Value(0, MetaName = "ROOT", Required = true, HelpText = "Root directory.")>] root : string;
  [<Option('i', "index", Required = true, HelpText = "Valid index files.")>] indexFiles : seq<string>;
  [<Option(HelpText = "Describe every operation.")>] verbose : bool;
  [<Option("timeout", HelpText = "Session timeout in seconds.")>] timeout : int option;
  [<Option('e', HelpText = "Error file.")] errorFile : string;
  [<Option('x', HelpText = "Run in debug mode.")] debugMode : bool;
}
 
// parse the command line using default singleton
let result = Parser.Default.ParseArguments<options>(argv)
 
match result with 
| :? Parsed<'a> as parsed -> runLogic(parsed.Value)
| :? NotParsed<'a> as notParsed -> exitApp(notParsed.Errors)
| _ -> failwith "invalid parser result"

(**

The parsed record instance is wrapped in a ParserResult<'a> type. Normally you don't need to match against NotParsed<'a>
since errors automatically used by the library to generated the help screen.

To disable this feature, just build and configure the Parser instance by your own.

## Syntax

Syntax is defined by attribute attached to record fields:

*)

type options = {
    [<Option("fn", Required = true)>] fileName : string;
    [<Option(SetName = "ftp")] ftpUrl : System.Uri;
    [<Option(SetName = "ftp", Default = 20)] ftpPort : int;
    [<Option(SetName = "web")] httpUrl : System.Uri;
    [<Option(SetName = "web", Default = 80)] httpPort : int;
    [<Value(0)>] section : string;
}

(**

In this case,

  * `fileName`: is mandatory since it's marked with `Required=true`.

  * `ftpUrl`, `ftpPort`: can't be specified along with `httpUrl` and `httpPort`, since set are mutually exclusive.

  * `section`: it's a value with positional index equal to `0`.

Few things to note:

  * `fptUrl` and `httpUrl` are defined using a non primitive datatype like `System.Uri`; the library automatically supports
     any type that owns a constructor that accepts a string.

  * Index of positional arguments defined with `Value` attribute is calculated excluding named options and their values.

## Unparsing Machinery

CommandLineParser allows you to create a command line string from a record instance, (obtained from parsing or hand-crafted):
*)

open System.Diagnostics

let arguments : string = 
    Parser.FormatCommandLine { inputFile = "file.xml"; outputFile = "file.bin"; verbose = true }

Process.Start("app.exe", arguments)
