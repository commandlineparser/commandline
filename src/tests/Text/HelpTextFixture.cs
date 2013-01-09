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
using NUnit.Framework;
using Should.Fluent;
using CommandLine.Tests.Mocks;
#endregion

namespace CommandLine.Text.Tests
{
    [TestFixture]
    public sealed class HelpTextFixture
    {
        #region Mock Objects
        class MockOptions
        {
            [Option('v', "verbose")]
            public bool Verbose { get; set; }

            [Option("input-file")]
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

        private HelpText _helpText;

        [SetUp]
        public void SetUp()
        {
            _helpText = new HelpText(new HeadingInfo(_ThisAssembly.Title, _ThisAssembly.Version));
        }

        [Test]
        public void AddAnEmptyPreOptionsLineIsAllowed()
        {
            _helpText.AddPreOptionsLine(string.Empty);
        }

        /// <summary>
        /// Ref.: #REQ0002
        /// </summary>
        [Test]
        public void PostOptionsLinesFeatureAdded()
        {
            var local = new HelpText("Heading Info.");
            local.AddPreOptionsLine("This is a first pre-options line.");
            local.AddPreOptionsLine("This is a second pre-options line.");
            local.AddOptions(new MockOptions());
            local.AddPostOptionsLine("This is a first post-options line.");
            local.AddPostOptionsLine("This is a second post-options line.");

            string help = local.ToString();

            string[] lines = help.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines[lines.Length - 2].Should().Equal("This is a first post-options line.");
            lines[lines.Length - 1].Should().Equal("This is a second post-options line.");
        }

        [Test]
        public void WhenHelpTextIsLongerThanWidthItWillWrapAroundAsIfInAColumn()
        {
            _helpText.MaximumDisplayWidth = 40;
            _helpText.AddOptions(new MockOptionsWithLongDescription());
            string help = _helpText.ToString();

            string[] lines = help.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            lines[2].Should().Equal("  v, verbose    This is the description"); //"The first line should have the arguments and the start of the Help Text.");
            //string formattingMessage = "Beyond the second line should be formatted as though it's in a column.";
            lines[3].Should().Equal("                of the verbosity to ");
            lines[4].Should().Equal("                test out the wrapping ");
            lines[5].Should().Equal("                capabilities of the ");
            lines[6].Should().Equal("                Help Text.");
        }

        [Test]
        public void LongHelpTextWithoutSpaces()
        {
            _helpText.MaximumDisplayWidth = 40;
            _helpText.AddOptions(new MockOptionsWithLongDescriptionAndNoSpaces());
            string help = _helpText.ToString();

            string[] lines = help.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines[2].Should().Equal("  v, verbose    Before ");
            lines[3].Should().Equal("                012345678901234567890123");
            lines[4].Should().Equal("                After");
            lines[5].Should().Equal("  input-file    Before ");
            lines[6].Should().Equal("                012345678901234567890123");
            lines[7].Should().Equal("                456789 After");
        }

        [Test]
        public void LongPreAndPostLinesWithoutSpaces()
        {
            var local = new HelpText("Heading Info.");
            local.MaximumDisplayWidth = 40;
            local.AddPreOptionsLine("Before 0123456789012345678901234567890123456789012 After");
            local.AddOptions(new MockOptions());
            local.AddPostOptionsLine("Before 0123456789012345678901234567890123456789 After");

            string help = local.ToString();

            string[] lines = help.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines[1].Should().Equal("Before ");
            lines[2].Should().Equal("0123456789012345678901234567890123456789");
            lines[3].Should().Equal("012 After");
            lines[lines.Length - 3].Should().Equal("Before ");
            lines[lines.Length - 2].Should().Equal("0123456789012345678901234567890123456789");
            lines[lines.Length - 1].Should().Equal(" After");
        }

        [Test]
        public void CustomizeOptionsFormat()
        {
            var local = new HelpText("Customizing Test.");
            local.FormatOptionHelpText += new EventHandler<FormatOptionHelpTextEventArgs>(CustomizeOptionsFormat_FormatOptionHelpText);
            local.AddPreOptionsLine("Pre-Options.");
            local.AddOptions(new MockOptionsWithDescription());
            local.AddPostOptionsLine("Post-Options.");

            string help = local.ToString();

            Console.WriteLine(help);

            string[] lines = help.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines[0].Should().Equal("Customizing Test.");
            lines[1].Should().Equal("Pre-Options.");
            lines[3].Should().Equal("  v, verbose       Kommentar umfassend Operationen.");
            lines[4].Should().Equal("  i, input-file    Erforderlich. Gibt den Eingang an zu bearbeitenden Datei.");
            lines[6].Should().Equal("Post-Options.");
        }

        [Test]
        public void InstancingWithParameterlessConstructor()
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
            lines[0].Should().Equal("Parameterless Constructor Test.");
            lines[1].Should().Equal(string.Format(CultureInfo.InvariantCulture, "Copyright (C) {0} Author", year));
            lines[2].Should().Equal("Pre-Options.");
            lines[4].Should().Equal("  s, something    Input something here.");
            lines[6].Should().Equal("Post-Options.");
        }

        [Test]
        public void AddOptionsWithDashes()
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
            lines[3].Should().Equal("  -s, --something    Input something here.");
        }

        [Test]
        public void CreateBasicInstance()
        {
            var local = new HelpText();

            local.ToString().Should().Equal("");
        }

        [Test]
        public void InvokeRenderParsingErrorsText()
        {
            var sw = new StringWriter();
            var options = new RPEOptions();
            var parser = new CommandLineParser(new CommandLineParserSettings {
                MutuallyExclusive = true, CaseSensitive = true, HelpWriter = sw});
            var result = parser.ParseArguments(new string[] {"--option-b", "hello", "-cWORLD"}, options);

            result.Should().Be.False();

            var outsw = sw.ToString();

            Console.WriteLine(outsw);

            var lines = outsw.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            lines[0].Should().Equal("--option-b option violates format.");
            lines[1].Should().Equal("-c/--option-c option violates format.");
            lines[2].Should().Equal("-a required option is missing.");
        }

        /*
        [Test]
        public void AutoBuildWithRenderParsingErrorsHelper()
        {
            var sw = new StringWriter();
            var options = new RPEOptionsForAutoBuild();
            var parser = new CommandLineParser(new CommandLineParserSettings {
                MutuallyExclusive = true, CaseSensitive = true, HelpWriter = sw});
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


        [Test]
        public void AutoBuild()
        {
            var sw = new StringWriter();
            var options = new SimpleOptionsForAutoBuid();
            var parser = new CommandLineParser(new CommandLineParserSettings {
                MutuallyExclusive = true, CaseSensitive = true, HelpWriter = sw});
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
        [Test]
        public void DetailedHelpWithBadFormat()
        {
            var options = new ComplexOptionsWithHelp();

            bool result = new CommandLineParser(new CommandLineParserSettings(Console.Out)).ParseArguments(
                new string[] { "-iIN.FILE", "-oOUT.FILE", "--offset", "abc" }, options);

            result.Should().Be.False();
        }

        [Test]
        public void DetailedHelpWithMissingRequired()
        {
            var options = new ComplexOptionsWithHelp();

            bool result = new CommandLineParser(new CommandLineParserSettings(Console.Out)).ParseArguments(
                new string[] { "-j0" }, options);

            result.Should().Be.False();
        }

        [Test]
        public void DetailedHelpWithMissingRequiredAndBadFormat()
        {
            var options = new ComplexOptionsWithHelp();

            bool result = new CommandLineParser(new CommandLineParserSettings(Console.Out)).ParseArguments(
                new string[] { "-i0" }, options);

            result.Should().Be.False();
        }

        [Test]
        public void DetailedHelpWithBadMutualExclusiveness()
        {
            var options = new ComplexOptionsWithHelp();

            bool result = new CommandLineParser(new CommandLineParserSettings(true, true, Console.Out)).ParseArguments(
                new string[] { "-iIN.FILE", "-oOUT.FILE", "--offset", "0", "-ap" }, options);

            result.Should().Be.False();
        }
        
        [Test]
        public void DetailedHelpWithBadFormatAndMutualExclusiveness()
        {
            var options = new ComplexOptionsWithHelp();

            bool result = new CommandLineParser(new CommandLineParserSettings(true, true, Console.Out)).ParseArguments(
                new string[] { "-iIN.FILE", "-oOUT.FILE", "--offset", "zero", "-pa" }, options);

            result.Should().Be.False();
        }


        [Test]
        public void MultipleRequiredFields_WithMoreThanOneRequiredFieldNotSpecified_ReportsAllMissingRequiredFields()
        {
          var options = new ComplexOptions();
          using (var writer = new StringWriter())
          {
            new CommandLineParser(new CommandLineParserSettings(false,  false, writer)).ParseArguments(new string[0], options, writer);

            options.InternalLastPostParsingState.Errors.Should().Count.Exactly(2);
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