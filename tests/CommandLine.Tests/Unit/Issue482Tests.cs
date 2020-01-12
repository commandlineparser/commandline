using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using Xunit;
using FluentAssertions;

namespace CommandLine.Tests.Unit
{
    public class Issue482Tests
    {
        [Fact]
        public void AutoBuild_without_ordering()
        {
            string expectedCompany = "Company";


            var parser = Parser.Default;
            var parseResult = parser.ParseArguments<Options_HelpText_Ordering_Verb1, Options_HelpText_Ordering_Verb2>(
                    new[] {  "verb1", "--help" })
                .WithNotParsed(errors => { ; })
                .WithParsed(args => {; });

            var message = HelpText.AutoBuild(parseResult,
                error =>error,
                ex => ex
            );

            string helpMessage = message.ToString();
            var helps = helpMessage.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(2).ToList<string>();
            List<string> expected = new List<string>()
            {
                "  -a, --alpha      Required.",
                "  -b, --alpha2     Required.",
                "  -d, --charlie",
                "  -c, --bravo",
                "-f, --foxtrot",
                "-e, --echo",
                "--help           Display this help screen.",
                "--version        Display version information.",
                "value pos. 0"
            };
            expected.Count.Should().Be(helps.Count);
            int i = 0;
            foreach (var expect in expected)
            {
                expect.Trim().Should().Be(helps[i].Trim());
                i++;
            }

            ;
        }

        [Fact]
        public void AutoBuild_with_ordering()
        {
            string expectedCompany = "Company";


            var parser = Parser.Default;
            var parseResult = parser.ParseArguments<Options_HelpText_Ordering_Verb1, Options_HelpText_Ordering_Verb2>(
                    new[] { "verb1", "--help" })
                .WithNotParsed(errors => { ; })
                .WithParsed(args => {; });

            Comparison<ComparableOption> comparison = HelpText.RequiredThenAlphaComparison;

            string message = HelpText.AutoBuild(parseResult,
                error =>
                {
                    error.OptionComparison = HelpText.RequiredThenAlphaComparison;
                    return error;
                },
                ex => ex);


            string helpMessage = message.ToString();
            var helps = helpMessage.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(2).ToList<string>();
            List<string> expected = new List<string>()
            {
                "  -a, --alpha      Required.",
                "  -b, --alpha2     Required.",
                "  -c, --bravo",
                "  -d, --charlie",
                "-e, --echo",
                "-f, --foxtrot",
                "--help           Display this help screen.",
                "--version        Display version information.",
                "value pos. 0"
            };
            expected.Count.Should().Be(helps.Count);
            int i = 0;
            foreach (var expect in expected)
            {
                expect.Trim().Should().Be(helps[i].Trim());
                i++;
            }

            ;
        }

        [Fact]
        public void AutoBuild_with_ordering_on_shortName()
        {
            string expectedCompany = "Company";


            var parser = Parser.Default;
            var parseResult = parser.ParseArguments<Options_HelpText_Ordering_Verb1, Options_HelpText_Ordering_Verb2>(
                    new[] {  "verb1", "--help" })
                .WithNotParsed(errors => { ; })
                .WithParsed(args => {; });

            Comparison<ComparableOption> orderOnShortName = (ComparableOption attr1, ComparableOption attr2) =>
                   {
                       if (attr1.IsOption && attr2.IsOption)
                       {
                           if (attr1.Required && !attr2.Required)
                           {
                               return -1;
                           }
                           else if (!attr1.Required && attr2.Required)
                           {
                               return 1;
                           }
                           else
                           {
                               if (string.IsNullOrEmpty(attr1.ShortName) && !string.IsNullOrEmpty(attr2.ShortName))
                               {
                                   return 1;
                               }
                               else if (!string.IsNullOrEmpty(attr1.ShortName) && string.IsNullOrEmpty(attr2.ShortName))
                               {
                                   return -1;
                               }
                               return String.Compare(attr1.ShortName, attr2.ShortName, StringComparison.Ordinal);
                           }
                       }
                       else if (attr1.IsOption && attr2.IsValue)
                       {
                           return -1;
                       }
                       else
                       {
                           return 1;
                       }
                   };

            string message = HelpText.AutoBuild(parseResult,
                    error =>
                    {
                        error.OptionComparison = orderOnShortName;
                        return error;
                    },
                    ex => ex,
                        false,
                        80
                        );


            var helps = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(2).ToList<string>();
            List<string> expected = new List<string>()
            {
                "  -a, --alpha      Required.",
                "  -b, --alpha2     Required.",
                "  -c, --bravo",
                "  -d, --charlie",
                "-e, --echo",
                "-f, --foxtrot",
                 "--help           Display this help screen.",
                "--version        Display version information.",
                "value pos. 0"
            };
            expected.Count.Should().Be(helps.Count);
            int i = 0;
            foreach (var expect in expected)
            {
                expect.Trim().Should().Be(helps[i].Trim());
                i++;
            }
        }


    }
}
