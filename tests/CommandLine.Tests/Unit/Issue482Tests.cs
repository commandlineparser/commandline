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
        public void AutoBuild_without_ordering()
        {
            string expectedCompany = "Company";


            var parser = Parser.Default;
            var parseResult = parser.ParseArguments<Options_HelpText_Ordering_Verb1, Options_HelpText_Ordering_Verb2>(
                    new[] {"verb1", "--alpha", "alpaga", "--alpha2", "alala", "--charlie", "charlot"})
                .WithNotParsed(errors => { throw new InvalidOperationException("Must be parsed."); })
                .WithParsed(args => { ; });

            var toto = HelpText.AutoBuild(parseResult, 
                err => { throw new InvalidOperationException($"help text build failed. {err.ToString()}"); },
                ex =>
                {
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
                "--version        Display version information.",
                "value pos. 0"     
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

        [Fact]
        public void AutoBuild_with_ordering_fluent()
        {
            string expectedCompany = "Company";


            var parser = Parser.Default;
            var parseResult = parser.ParseArguments<Options_HelpText_Ordering_Verb1, Options_HelpText_Ordering_Verb2>(
                    new[] {"verb1", "--alpha", "alpaga", "--alpha2", "alala", "--charlie", "charlot"})
                .WithNotParsed(errors => { throw new InvalidOperationException("Must be parsed."); })
                .WithParsed(args => { ; });

            Comparison<ComparableOption> comparison = HelpText.RequiredThenAlphaComparison;


            var message = HelpText.CreateWith(parseResult)
            .WithComparison(HelpText.RequiredThenAlphaComparison)
            .OnError(error => {
                throw new InvalidOperationException($"help text build failed. {error.ToString()}");
            })
            .OnExample(ex =>
                {
                    return null;
                })
                .Build();
            
        

 

            string helpMessage = message.ToString();
            var helps = helpMessage.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).Skip(2).ToList<string>();
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
