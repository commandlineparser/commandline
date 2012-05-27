using System;
using CommandLine;
using CommandLine.Text;

namespace CommandLine.Tests.Mocks
{
    public class RPEOptions : CommandLineOptionsBase
    {
        [Option("a", null, Required = true, HelpText = "This string option is defined A.")]
        public string OptionA { get; set; }

        [Option(null, "option-b", HelpText = "This integer option is defined B.")]
        public int OptionB { get; set; }

        [Option("c", "option-c", HelpText = "This double option is defined C.")]
        public double OptionC { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            if (LastPostParsingState.Errors.Count > 0)
            {
                return new HelpText().RenderParsingErrorsText(this, 0);
            }
            return "";
        }
    }
}

