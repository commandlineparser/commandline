#region License
//
// Command Line Library: Program.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2013 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CommandLine;
using CommandLine.Text;
#endregion

namespace CommandLine.Demo
{
    partial class Program
    {
        private enum OptimizeFor
        {
            Unspecified,
            Speed,
            Accuracy
        }

        //
        // You no longer need to inherit from CommandLineOptionsBase (removed)
        //
        private sealed class Options
        {
            [Option('r', "read", MetaValue = "FILE", Required = true, HelpText = "Input file with data to process.")]
            public string InputFile {get; set;}

            [Option('w', "write", MetaValue = "FILE", HelpText = "Output FILE with processed data (otherwise standard output).")]
            public string OutputFile { get; set; }

            [Option("calculate", HelpText = "Add results in bottom of tabular data.")]
            public bool Calculate { get; set; }

            [Option('v', MetaValue = "INT", HelpText = "Verbose level. Range: from 0 to 2.")]
            public int? VerboseLevel { get; set; }
             
            [Option("i", HelpText = "If file has errors don't stop processing.")]
            public bool IgnoreErrors { get; set; }

            [Option('j', "jump", MetaValue = "INT", DefaultValue = 0, HelpText = "Data processing start offset.")]
            public double StartOffset { get; set; }

            [Option("optimize", HelpText = "Optimize for Speed|Accuracy.")]
            public OptimizeFor Optimization {get;set;}

            [ValueList(typeof(List<string>))]
            public IList<string> DefinitionFiles { get; set; }

            [OptionList('o', "operators", Separator = ';', HelpText = "Operators included in processing (+;-;...)." +
                " Separate each operator with a semicolon." + " Do not include spaces between operators and separator.")]
            public IList<string> AllowedOperators { get; set; }

            //
            // Marking a property of type IParserState with ParserStateAttribute allows you to
            // receive an instance of ParserState (that contains a IList<ParsingError>).
            // This is equivalent from inheriting from CommandLineOptionsBase (of previous versions)
            // with the advantage to not propagating a type of the library.
            //
            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

    }
}