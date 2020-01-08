using System.Collections.Generic;
using System.Linq;
using Xunit;
using CommandLine.Text;

namespace CommandLine.Tests.Unit
{
	//Reference: PR# 392
    public class Issue389Tests
    {

        private const int ERROR_SUCCESS = 0;

        // Test method (xUnit) which fails
        [Fact]
        public void CallMain_GiveHelpArgument_ExpectSuccess()
        {
            var result = Program.__Main(new[] { "--help" });

            Assert.Equal(ERROR_SUCCESS, result);
        }

        // main program
        internal class Program
        {


            internal static int __Main(string[] args)
            {
                bool hasError = false;
                bool helpOrVersionRequested = false;

                ParserResult<Options> parsedOptions = Parser.Default.ParseArguments<Options>(args)
                    .WithNotParsed(errors => {
                        helpOrVersionRequested = errors.Any(
                            x => x.Tag == ErrorType.HelpRequestedError 
                                 || x.Tag == ErrorType.VersionRequestedError);
                        hasError = true;
                    });

                if(helpOrVersionRequested)
                {
                    return ERROR_SUCCESS;
                }

                // Execute as a normal call
                // ...
                return ERROR_SUCCESS;
            }

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
    }
}
