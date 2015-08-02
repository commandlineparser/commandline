(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../build"
#r "CommandLine.dll"

open System

(**

# Command Line Parser Library

Terse syntax C# command line parser for .NET with F# support.

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      CommandLineParser can be <a href="https://nuget.org/packages/CommandLineParser">installed from NuGet</a>:
      <pre>PM> Install-Package CommandLineParser -Pre</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

## Introduction

The library parses command line arguments to a record decorated with attributes:
*)

type options = {
  [<Option>] files : seq<string>;
  [<Option>] verbose : bool;
  [<Option>] offset : int64 option;
}

(**

Previous record defines the above parsing scheme:

    [lang=bash]
    --files file1.bin file2.txt file3.xml --verbose --offset 11

and it will be used to build up this record instance:
*)

{ files = seq { yield "file1.bin"; yield "file2.txt"; yield "file3.xml"}; verbose = true; offset = Some 11L }

(**

## Who uses CommandLineParser?

* [FSharp.Formatting](http://tpetricek.github.io/FSharp.Formatting/)

* Various commercial products

## Documentation

  * [Tutorial](tutorial.html) A short walkthrough of CommandLineParser features.

  * [GitHub Wiki](https://github.com/gsscoder/commandline/wiki/Latest-Version)

## Contributing and copyright

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork 
the project and submit pull requests.

The library is available under the MIT License. 
For more information see the [License file][license] in the GitHub repository. 

  [gh]: https://github.com/gsscoder/commandline
  [issues]: https://github.com/gsscoder/commandline/issues
  [license]: https://github.com/gsscoder/commandline/blob/master/License.md

*)