using System.Collections.Generic;
using System.Linq;
using Xunit;
using CommandLine.Text;

namespace CommandLine.Tests.Unit
{
    //Reference: PR# 634
    public class Issue543Tests
    {

        private const int ERROR_SUCCESS = 0;

        [Fact]
        public void Parser_GiveHelpArgument_ExpectSuccess()
        {
            var result = Parser.Default.ParseArguments<Options>(new[] { "--help" });

            Assert.Equal(ParserResultType.NotParsed, result.Tag);
            Assert.Null(result.Value);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void Parser_GiveConnectionStringAndJobId_ExpectSuccess()
        {
            var result = Parser.Default.ParseArguments<Options>(new[] {
                "-c", "someConnectionString",
                "-j", "1234",
            });

            Assert.Equal(ParserResultType.Parsed, result.Tag);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Errors);
            Assert.Equal("someConnectionString", result.Value.ConnectionString);
            Assert.Equal(1234, result.Value.JobId);
        }

        [Fact]
        public void Parser_GiveVerb1_ExpectSuccess()
        {
            var result = Parser.Default.ParseArguments<Verb1Options, Verb2Options>(new[] {
                "verb1",
                "-j", "1234",
            });

            Assert.Equal(ParserResultType.Parsed, result.Tag);
            Assert.Empty(result.Errors);
            Assert.NotNull(result.Value);
            Assert.NotNull(result.Value as Verb1Options);
            Assert.Equal(1234, (result.Value as Verb1Options).JobId);
        }

        [Fact]
        public void Parser_GiveVerb2_ExpectSuccess()
        {
            var result = Parser.Default.ParseArguments<Verb1Options, Verb2Options>(new[] {
                "verb2",
                "-c", "someConnectionString",
            });

            Assert.Equal(ParserResultType.Parsed, result.Tag);
            Assert.Empty(result.Errors);
            Assert.NotNull(result.Value);
            Assert.NotNull(result.Value as Verb2Options);
            Assert.Equal("someConnectionString", (result.Value as Verb2Options).ConnectionString);
        }

        // Options
        internal class Options
        {
            [Option('c', "connectionString", Required = true, HelpText = "Texts.ExplainConnection")]
            public string ConnectionString { get; set; }

            [Option('j', "jobId", Required = true, HelpText = "Texts.ExplainJob")]
            public int JobId { get; set; }

            [Usage(ApplicationAlias = "Importer.exe")]
            public static IEnumerable<Example> Examples
            {
                get => new[] {
                    new Example("Texts.ExplainExampleExecution", new Options() {
                        ConnectionString="Server=MyServer;Database=MyDatabase",
                        JobId = 5
                    }),
                };
            }
        }

        // Options
        [Verb("verb1")]
        internal class Verb1Options
        {
            [Option('j', "jobId", Required = false, HelpText = "Texts.ExplainJob")]
            public int JobId { get; set; }
        }

        // Options
        [Verb("verb2")]
        internal class Verb2Options
        {
            [Option('c', "connectionString", Required = false, HelpText = "Texts.ExplainConnection")]
            public string ConnectionString { get; set; }
        }

    }
}

