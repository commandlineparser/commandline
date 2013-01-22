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
//#define EXEC_TESTS
#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CommandLine;
using CommandLine.Text;
#if EXEC_TESTS
using CommandLine.Tests;
using CommandLine.Text.Tests;
#endif
#endregion

namespace CommandLine.Demo
{
    sealed partial class Program
    {
        private static readonly HeadingInfo _headingInfo = new HeadingInfo("sampleapp", "1.8");

        /// <summary>
        /// Application's Entry Point.
        /// </summary>
        /// <param name="args">Command line arguments splitted by the system.</param>
        private static void Main(string[] args)
        {
#if EXEC_TESTS
            RunATestForDebugging();
#endif
            var options = new Options();
            var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))            
                Environment.Exit(1);            

            DoCoreTask(options);           
            Environment.Exit(0);
        }

        private static void DoCoreTask(Options options)
        {
            if (options.VerboseLevel == null)
                Console.WriteLine("verbose [off]");
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
            if (!string.IsNullOrEmpty(options.OutputFile))
                _headingInfo.WriteMessage(string.Format("writing elaborated data: {0} ...", options.OutputFile));
            else
            {
                _headingInfo.WriteMessage("elaborated data:");
                Console.WriteLine("[...]");
            }
        }

#if EXEC_TESTS
        private static void RunATestForDebugging()
        {
            //OptionArrayAttributeParsingFixture f = new OptionArrayAttributeParsingFixture();
            //ArgumentParserFixture f2 = new ArgumentParserFixture();
            //HelpTextFixture f3 = new HelpTextFixture();
            //AttributesFixture f6 = new AttributesFixture();
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
            //f3.DetailedHelpWithBadFormatAndMutualExclusiveness();
            //f3.DetailedHelpWithBadMutualExclusiveness();
            //f3.DetailedHelpWithMissingRequired();
            //f3.DetailedHelpWithBadFormat();
            //f3.CreateBasicInstance();
            //f3.InvokeRenderParsingErrorsText();
            //f3.AutoBuildWithRenderParsingErrorsHelper();
            //var f4 = new OptionArrayAttributeParsingFixture();
            //f4.ParseTwoUIntConsecutiveArray();
            //CommandLineParserFixture f5 = new CommandLineParserFixture();
            //f5.ParseNegativeIntegerValue();
            //f5.ParseNegativeIntegerValue_InputStyle4();
            //f5.ParseNegativeFloatingPointValue_InputStyle4();
            //f5.PassingLongValueToIntegerOptionMustFailGracefully();
            //f6.AllOptionsAllowOneCharacterInShortName();
            Console.Write("press any key");
            Console.ReadKey();
            Environment.Exit(1);
        }
#endif
    }
}