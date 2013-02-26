#region License
//
// Command Line Library: HelpTextFixture.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
// Contributor(s):
//   Steven Evans
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
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Globalization;
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
#endregion

namespace CommandLine.Tests.Unit.Text
{
    public class HelpTextFixture
    {
        #region Mock Objects
        class MockOptions
        {
            [Option('v', "verbose")]
            public bool Verbose { get; set; }

            [Option("input-file")]
            public string FileName { get; set; }
        }

        class MockOptionsWithMetaValue
        {
            [Option('v', "verbose", HelpText = "Comment extensively every operation.")]
            public bool Verbose { get; set; }

            [Option('i', "input-file", MetaValue="FILE", Required = true, HelpText = "Specify input FILE to be processed.")]
            public string FileName { get; set; }
        }

        class MockOptionsWithDescription
        {
            [Option('v', "verbose", HelpText = "Comment extensively every operation.")]
            public bool Verbose { get; set; }

            [Option('i', "input-file", Required = true, HelpText = "Specify input file to be processed.")]
            public string FileName { get; set; }
        }

        private class MockOptionsWithLongDescription
        {
            [Option('v', "verbose", HelpText = "This is the description of the verbosity to test out the wrapping capabilities of the Help Text.")]
            public bool Verbose { get; set; }

            [Option("input-file", HelpText = "This is a very long description of the Input File argument that gets passed in.  It should  be passed in as a string.")]
            public string FileName { get; set; }
        }

        private class MockOptionsWithLongDescriptionAndNoSpaces
        {
            [Option('v', "verbose", HelpText = "Before 012345678901234567890123 After")]
            public bool Verbose { get; set; }

            [Option("input-file", HelpText = "Before 012345678901234567890123456789 After")]
            public string FileName { get; set; }
        }

        public class MockOptionsSimple
        {
            [Option('s', "something", HelpText = "Input something here.")]
            public string Something { get; set; }
        }

        public class ComplexOptionsWithHelp : ComplexOptions
        {
            [Option('a', "all", HelpText = "Read the file completely.", MutuallyExclusiveSet = "reading")]
            public bool ReadAll { get; set; }

            [Option('p', "part", HelpText = "Read the file partially.", MutuallyExclusiveSet = "reading")]
            public bool ReadPartially { get; set; }

            [HelpOption(HelpText ="Displays this help screen.")]
            public string GetUsage()
            {
                var help = new HelpText(new HeadingInfo("unittest", "1.9"));
                help.AdditionalNewLineAfterOption = true;
                help.Copyright = new CopyrightInfo("CommandLine.dll Author", 2005, 2011);

                // handling parsing error code
                string errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
                    help.AddPreOptionsLine(errors);
                }

                help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
                help.AddPreOptionsLine("the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
                help.AddPreOptionsLine("Usage: Please run the unit...");
                help.AddOptions(this);

                return help;
            }
        }
        #endregion

        [Fact]
        public void Add_an_empty_pre_options_line_is_allowed()
        {
            var helpText = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131"));
            helpText.AddPreOptionsLine(string.Empty);
        }

        /// <summary>
        /// Ref.: #REQ0002
        /// </summary>
        [Fact]
        public void Post_options_lines_feature_added()
        {
            var local = new HelpText("Heading Info.");
            local.AddPreOptionsLine("This is a first pre-options line.");
            local.AddPreOptionsLine("This is a second pre-options line.");
            local.AddOptions(new MockOptions());
            local.AddPostOptionsLine("This is a first post-options line.");
            local.AddPostOptionsLine("This is a second post-options line.");

            string help = local.ToString();

            string[] lines = help.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines[lines.Length - 2].Should().Be("This is a first post-options line.");
            lines[lines.Length - 1].Should().Be("This is a second post-options line.");
        }

        [Fact]
        public void Meta_value()
        {
            var local = new HelpText("Meta Value.");
            local.AddOptions(new MockOptionsWithMetaValue());

            string help = local.ToString();
            string[] lines = help.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines[lines.Length - 2].Should().Be("  i FILE, input-file=FILE    Required. Specify input FILE to be processed.");
        }

        [Fact]
        public void When_help_text_is_longer_than_width_it_will_wrap_around_as_if_in_a_column()
        {
            var helpText = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131"));
            helpText.MaximumDisplayWidth = 40;
            helpText.AddOptions(new MockOptionsWithLongDescription());
            string help = helpText.ToString();

            string[] lines = help.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            lines[2].Should().Be("  v, verbose    This is the description"); //"The first line should have the arguments and the start of the Help Text.");
            //string formattingMessage = "Beyond the second line should be formatted as though it's in a column.";
            lines[3].Should().Be("                of the verbosity to ");
            lines[4].Should().Be("                test out the wrapping ");
            lines[5].Should().Be("                capabilities of the ");
            lines[6].Should().Be("                Help Text.");
        }

        [Fact]
        public void Long_help_text_without_spaces()
        {
            var helpText = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131"));
            helpText.MaximumDisplayWidth = 40;
            helpText.AddOptions(new MockOptionsWithLongDescriptionAndNoSpaces());
            string help = helpText.ToString();

            string[] lines = help.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines[2].Should().Be("  v, verbose    Before ");
            lines[3].Should().Be("                012345678901234567890123");
            lines[4].Should().Be("                After");
            lines[5].Should().Be("  input-file    Before ");
            lines[6].Should().Be("                012345678901234567890123");
            lines[7].Should().Be("                456789 After");
        }

        [Fact]
        public void Long_pre_and_post_lines_without_spaces()
        {
            var local = new HelpText("Heading Info.");
            local.MaximumDisplayWidth = 40;
            local.AddPreOptionsLine("Before 0123456789012345678901234567890123456789012 After");
            local.AddOptions(new MockOptions());
            local.AddPostOptionsLine("Before 0123456789012345678901234567890123456789 After");

            string help = local.ToString();

            string[] lines = help.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines[1].Should().Be("Before ");
            lines[2].Should().Be("0123456789012345678901234567890123456789");
            lines[3].Should().Be("012 After");
            lines[lines.Length - 3].Should().Be("Before ");
            lines[lines.Length - 2].Should().Be("0123456789012345678901234567890123456789");
            lines[lines.Length - 1].Should().Be(" After");
        }

        [Fact]
        public void Customize_options_format()
        {
            var local = new HelpText("Customizing Test.");
            local.FormatOptionHelpText += new EventHandler<FormatOptionHelpTextEventArgs>(CustomizeOptionsFormat_FormatOptionHelpText);
            local.AddPreOptionsLine("Pre-Options.");
            local.AddOptions(new MockOptionsWithDescription());
            local.AddPostOptionsLine("Post-Options.");

            string help = local.ToString();

            Console.WriteLine(help);

            string[] lines = help.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines[0].Should().Be("Customizing Test.");
            lines[1].Should().Be("Pre-Options.");
            lines[3].Should().Be("  v, verbose       Kommentar umfassend Operationen.");
            lines[4].Should().Be("  i, input-file    Erforderlich. Gibt den Eingang an zu bearbeitenden Datei.");
            lines[6].Should().Be("Post-Options.");
        }

        [Fact]
        public void Instancing_with_parameterless_constructor()
        {
            var year = DateTime.Now.Year;
            var local = new HelpText();
            local.Heading = new HeadingInfo("Parameterless Constructor Test.");
            local.Copyright = new CopyrightInfo("Author", year);
            local.AddPreOptionsLine("Pre-Options.");
            local.AddOptions(new MockOptionsSimple());
            local.AddPostOptionsLine("Post-Options.");

            string help = local.ToString();

            Console.WriteLine(help);

            string[] lines = help.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines[0].Should().Be("Parameterless Constructor Test.");
            lines[1].Should().Be(string.Format(CultureInfo.InvariantCulture, "Copyright (C) {0} Author", year));
            lines[2].Should().Be("Pre-Options.");
            lines[4].Should().Be("  s, something    Input something here.");
            lines[6].Should().Be("Post-Options.");
        }

        [Fact]
        public void Add_options_with_dashes()
        {
            var local = new HelpText {
                AddDashesToOption = true,
                Heading = new HeadingInfo("AddOptionsWithDashes"),
                Copyright = new CopyrightInfo("Author", DateTime.Now.Year)
            };
            local.AddOptions(new MockOptionsSimple());
            
            string help = local.ToString();
            
            Console.WriteLine(help);
            
            string[] lines = help.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines[3].Should().Be("  -s, --something    Input something here.");
        }

        [Fact]
        public void Create_basic_instance()
        {
            var local = new HelpText();

            local.ToString().Should().Be("");
        }

        [Fact]
        public void Invoke_render_parsing_errors_text()
        {
            var sw = new StringWriter();
            var options = new OptionsForErrorsScenario();
            var parser = new CommandLine.Parser(new ParserSettings {
                MutuallyExclusive = true, CaseSensitive = true, HelpWriter = sw});
            var result = parser.ParseArguments(new string[] {"--option-b", "hello", "-cWORLD"}, options);

            result.Should().BeFalse();

            var outsw = sw.ToString();

            Console.WriteLine(outsw);

            var lines = outsw.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            lines[0].Should().Be("--option-b option violates format.");
            lines[1].Should().Be("-c/--option-c option violates format.");
            lines[2].Should().Be("-a required option is missing.");
        }

        /*
        [Fact]
        public void Auto_build_with_render_parsing_errors_helper()
        {
            var sw = new StringWriter();
            var options = new RPEOptionsForAutoBuild();
            var parser = new CommandLine.Parser(new ParserSettings {
                MutuallyExclusive = true, CaseSensitive = true, UseHelpWriter = sw});
            var result = parser.ParseArguments(new string[] {"--option-b", "hello", "-cWORLD"}, options);

            Assert.IsFalse(result);

            var outsw = sw.ToString();

            Console.WriteLine(outsw);

            var lines = outsw.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            Assert.AreEqual(lines[0], "CommandLine.Tests 1.9");
            Assert.AreEqual(lines[1], "Copyright (C) 2005 - 2012 Giacomo Stelluti Scala");
            Assert.AreEqual(lines[3], "ERROR(S):");
            Assert.AreEqual(lines[4], "  --option-b option violates format.");
            Assert.AreEqual(lines[5], "  -c/--option-c option violates format.");
            Assert.AreEqual(lines[6], "  -a required option is missing.");
            Assert.AreEqual(lines[8], "This is free software. You may redistribute copies of it under the terms of");
            Assert.AreEqual(lines[9], "the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
            Assert.AreEqual(lines[10], "[no usage, this is a dll]");
            Assert.AreEqual(lines[12], "  -a                Required. This string option is defined A.");
            Assert.AreEqual(lines[14], "  --option-b        This integer option is defined B.");
            Assert.AreEqual(lines[16], "  -c, --option-c    This double option is defined C.");
            Assert.AreEqual(lines[18], "  --help            Display this help screen.");
        }


        [Fact]
        public void Auto_build()
        {
            var sw = new StringWriter();
            var options = new SimpleOptionsForAutoBuid();
            var parser = new CommandLine.Parser(new ParserSettings {
                MutuallyExclusive = true, CaseSensitive = true, UseHelpWriter = sw});
            var result = parser.ParseArguments(new string[] {}, options);

            Assert.IsFalse(result);

            var outsw = sw.ToString();

            Console.WriteLine(outsw);

            var lines = outsw.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            Assert.AreEqual(lines[0], "CommandLine.Tests 1.9");
            Assert.AreEqual(lines[1], "Copyright (C) 2005 - 2012 Giacomo Stelluti Scala");
            Assert.AreEqual(lines[2], "This is free software. You may redistribute copies of it under the terms of");
            Assert.AreEqual(lines[3], "the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
            Assert.AreEqual(lines[4], "[no usage, this is a dll]");
            Assert.AreEqual(lines[6], "  -m, --mock      Required. Force required.");
            Assert.AreEqual(lines[8], "  -s, --string    ");
            Assert.AreEqual(lines[10], "  -i              ");
            Assert.AreEqual(lines[12], "  --switch        ");
            Assert.AreEqual(lines[14], "  --help          Display this help screen.");
        }*/

        #region Parsing Errors Subsystem Test, related to Help Text building
        [Fact]
        public void Detailed_help_with_bad_format()
        {
            var options = new ComplexOptionsWithHelp();

            bool result = new CommandLine.Parser(new ParserSettings(Console.Out)).ParseArguments(
                new string[] { "-iIN.FILE", "-oOUT.FILE", "--offset", "abc" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Detailed_help_with_missing_required()
        {
            var options = new ComplexOptionsWithHelp();

            bool result = new CommandLine.Parser(new ParserSettings(Console.Out)).ParseArguments(
                new string[] { "-j0" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Detailed_help_with_missing_required_and_bad_format()
        {
            var options = new ComplexOptionsWithHelp();

            bool result = new CommandLine.Parser(new ParserSettings(Console.Out)).ParseArguments(
                new string[] { "-i0" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Detailed_help_with_bad_mutual_exclusiveness()
        {
            var options = new ComplexOptionsWithHelp();

            bool result = new CommandLine.Parser(new ParserSettings(true, true, Console.Out)).ParseArguments(
                new string[] { "-iIN.FILE", "-oOUT.FILE", "--offset", "0", "-ap" }, options);

            result.Should().BeFalse();
        }
        
        [Fact]
        public void Detailed_help_with_bad_format_and_mutual_exclusiveness()
        {
            var options = new ComplexOptionsWithHelp();

            bool result = new CommandLine.Parser(new ParserSettings(true, true, Console.Out)).ParseArguments(
                new string[] { "-iIN.FILE", "-oOUT.FILE", "--offset", "zero", "-pa" }, options);

            result.Should().BeFalse();
        }


        [Fact]
        public void Multiple_required_fields_with_more_than_one_required_field_not_specified_reports_all_missing_required_fields()
        {
            var options = new ComplexOptions();
            using (var writer = new StringWriter())
            {
                new CommandLine.Parser(new ParserSettings(false,  false, writer)).ParseArguments(new string[0], options);

                options.LastParserState.Errors.Should().HaveCount(n => n == 2);
            }
        }
        #endregion

        private void CustomizeOptionsFormat_FormatOptionHelpText(object sender, FormatOptionHelpTextEventArgs e)
        {
            // Simulating a localization process.
            string optionHelp = null;

            switch (e.Option.ShortName.Value)
            {
                case 'v':
                    optionHelp = "Kommentar umfassend Operationen.";
                    break;

                case 'i':
                    optionHelp = "Gibt den Eingang an zu bearbeitenden Datei.";
                    break;
            }
            
            if (e.Option.Required)
            {
                optionHelp = "Erforderlich. " + optionHelp;
            }

            e.Option.HelpText = optionHelp;
        }
    }
}

