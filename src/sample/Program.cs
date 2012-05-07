#region License
//
// Command Line Library: Program.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2012 Giacomo Stelluti Scala
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
#define EXEC_TESTS
#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CommandLine;
using CommandLine.Text;
#if UNIT_TESTS && EXEC_TESTS
using CommandLine.Tests;
using CommandLine.Text.Tests;
#endif
#endregion

namespace SampleApp
{
    sealed class Program
    {
        private static readonly HeadingInfo _headingInfo = new HeadingInfo("sampleapp", "1.8");

        private enum OptimizeFor
        {
            Unspecified,
            Speed,
            Accuracy
        }

        private sealed class Options : CommandLineOptionsBase
        {
            #region Standard Option Attribute
            [Option("r", "read", Required = true, HelpText = "Input file with data to process.")]
            [DefaultValue("")]
            public string InputFile {get; set;}

            [Option("w", "write", HelpText = "Output file with processed data (otherwise standard output).")]
            [DefaultValue("")]
            public string OutputFile { get; set; }

            [Option(null, "calculate", HelpText = "Add results in bottom of tabular data.")]
            [DefaultValue(false)]
            public bool Calculate { get; set; }

            [Option("v", null, HelpText = "Verbose level. Range: from 0 to 2.")]
            [DefaultValue(null)]
            public int? VerboseLevel {get;set;}
             
            [Option("i", null, HelpText = "If file has errors don't stop processing.")]
            [DefaultValue(false)]
            public bool IgnoreErrors {get;set;}

            [Option("j", "jump", HelpText = "Data processing start offset.")]
            [DefaultValue(0)]
            public double StartOffset {get;set;}

            [Option(null, "optimize", HelpText = "Optimize for Speed|Accuracy.")]
            [DefaultValue(OptimizeFor.Unspecified)]
            public OptimizeFor Optimization {get;set;}
            #endregion

            #region Specialized Option Attribute

            [ValueList(typeof(List<string>))]
            [DefaultValue(null)]
            public IList<string> DefinitionFiles { get; set; }

            [OptionList("o", "operators", Separator = ';', HelpText = "Operators included in processing (+;-;...)." +
                " Separate each operator with a semicolon." + " Do not include spaces between operators and separator.")]
            [DefaultValue(null)]
            public IList<string> AllowedOperators { get; set; }
            
            [HelpOption(HelpText = "Dispaly this help screen.")]
            public string GetUsage()
            {
                var help = new HelpText(Program._headingInfo);
                help.AdditionalNewLineAfterOption = true;
                help.Copyright = new CopyrightInfo("Giacomo Stelluti Scala", 2005, 2012);
                this.HandleParsingErrorsInHelp(help);
                help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
                help.AddPreOptionsLine("the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
                help.AddPreOptionsLine("Usage: SampleApp -rMyData.in -wMyData.out --calculate");
                help.AddPreOptionsLine(string.Format("       SampleApp -rMyData.in -i -j{0} file0.def file1.def", 9.7));
                help.AddPreOptionsLine("       SampleApp -rMath.xml -wReport.bin -o *;/;+;-");
                help.AddOptions(this);

                return help;
            }

            private void HandleParsingErrorsInHelp(HelpText help)
            {
                if (this.LastPostParsingState.Errors.Count > 0)
                {
                }
                string errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
                if (!string.IsNullOrEmpty(errors))
                {
					help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
					help.AddPreOptionsLine(errors);
                }
            }
            #endregion
        }

        /// <summary>
        /// Application's Entry Point.
        /// </summary>
        /// <param name="args">Command Line Arguments splitted by the System.</param>
        private static void Main(string[] args)
        {
#if UNIT_TESTS && EXEC_TESTS
            RunATestForDebugging();
#endif
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
                Environment.Exit(1);

            DoCoreTask(options);

            Environment.Exit(0);
        }

        private static void DoCoreTask(Options options)
        {
            if (options.VerboseLevel == null)
                Console.Write("verbose [off]");
            else
                Console.WriteLine("verbose [on]: {0}", (options.VerboseLevel < 0 || options.VerboseLevel > 2) ? "#invalid value#" : options.VerboseLevel.ToString());
            Console.WriteLine();
            Console.WriteLine("input file: {0} ...", options.InputFile);
            foreach (string defFile in options.DefinitionFiles)
            {
                Console.WriteLine("  using definition file: {0}", defFile);
            }
            Console.WriteLine("  start offset: {0}", options.StartOffset);
            Console.WriteLine("  tabular data computation: {0}", options.Calculate.ToString().ToLowerInvariant());
            Console.WriteLine("  on errors: {0}", options.IgnoreErrors ? "continue" : "stop processing");
            Console.WriteLine("  optimize for: {0}", options.Optimization.ToString().ToLowerInvariant());
            if (options.AllowedOperators != null)
            {
                var builder = new StringBuilder();
                builder.Append("  allowed operators: ");
                foreach (string op in options.AllowedOperators)
                {
                    builder.Append(op);
                    builder.Append(", ");
                }
                Console.WriteLine(builder.Remove(builder.Length - 2, 2).ToString());
            }
            Console.WriteLine();
            if (options.OutputFile.Length > 0)
                _headingInfo.WriteMessage(string.Format("writing elaborated data: {0} ...", options.OutputFile));
            else
            {
                _headingInfo.WriteMessage("elaborated data:");
                Console.WriteLine("[...]");
            }
        }

#if UNIT_TESTS && EXEC_TESTS
        private static void RunATestForDebugging()
        {
            //OptionArrayAttributeParsingFixture f = new OptionArrayAttributeParsingFixture();
            //ArgumentParserFixture f2 = new ArgumentParserFixture();
            HelpTextFixture f3 = new HelpTextFixture();
            //f.ParseStringArrayOptionUsingShortName();
            //f.ParseStringArrayOptionUsingShortNameWithValueAdjacent();
            //f.ParseStringArrayOptionUsingLongName();
            //f.ParseStringArrayOptionUsingLongNameWithEqualSign();
            //f.ParseStringArrayOptionUsingShortNameAndStringOptionAfter();
            //f.ParseStringArrayOptionUsingLongNameWithValueList();
            //f.PassingBadValueToAnIntegerArrayOptionFails();
            //f.WillThrowExceptionIfOptionArrayAttributeBoundToStringWithShortName();
            //f.WillThrowExceptionIfOptionArrayAttributeBoundToIntegerWithShortName();
            //f2.GetNextInputValues();
            //f3.CustomizeOptionsFormat();
			f3.DetailedHelpWithBadFormatAndMutualExclusiveness();
            //f3.DetailedHelpWithBadMutualExclusiveness();
            //f3.DetailedHelpWithMissingRequired();
            //f3.DetailedHelpWithBadFormat();
            Console.Write("press any key");
            Console.ReadKey();
            Environment.Exit(1);
        }
#endif
    }
}