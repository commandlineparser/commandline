using System;
using System.Collections.Generic;
using System.Globalization;
using CommandLine.Core;
using System.Linq;
using System.Reflection;
using CommandLine.Infrastructure;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;
using System.Text;
using Xunit.Sdk;

namespace CommandLine.Tests.Unit
{
    public class Issue482Tests
    {
        [Fact]
        public void AutoBuild_with_ordering()
        {
            string expectedCompany = "Company";


            var parser = Parser.Default;
            var parseResult = parser.ParseArguments<Options_HelpText_Ordering_Verb1, Options_HelpText_Ordering_Verb2>(
                    new[] {"verb1", "--alpha", "alpaga", "--alpha2", "alala", "--charlie", "charlot"})
                .WithNotParsed(errors => { throw new InvalidOperationException("Must be parsed."); })
                .WithParsed(args => { ; });

            Comparison<OptionAttribute> comparison = (OptionAttribute option1, OptionAttribute option2) =>
            {
                if (option1 == null)
                {
                    return -1;
                }

                if (option2 == null)
                {
                    return 1;
                }

                if (option1.Required && !option2.Required)
                {
                    return -1;
                }
                else if (!option1.Required && option2.Required)
                {
                    return 1;
                }
                else
                {
                    return String.Compare(option1.LongName, option2.LongName, StringComparison.CurrentCulture);
                }
            };


            var toto = HelpText.AutoBuild(parseResult, comparison,
                err => { throw new InvalidOperationException($"help text build failed. {err.ToString()}"); },
                ex =>
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                });

            string helpMessage = toto.ToString();
            var helps = helpMessage.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).Skip(2).ToList<string>();
            List<string> expected = new List<string>()
            {
                "  -a, --alpha      Required.",
                "  -b, --alpha2     Required.",
                "  -d, --charlie",
                "  -c, --bravo",
                "-f, --foxtrot",
                "-e, --echo",
                "--help           Display this help screen.",
                "--version        Display version information."
            };
            Assert.Equal(expected.Count,helps.Count);
            int i = 0;
            foreach (var expect in expected)
            {
                Assert.Equal(expect.Trim(),helps[i].Trim());
                i++;
            }

            ;
        }
    }
}