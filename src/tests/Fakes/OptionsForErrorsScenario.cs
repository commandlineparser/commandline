using System;
using CommandLine;
using CommandLine.Text;

namespace CommandLine.Tests.Fakes
{
    public class OptionsForErrorsScenario : CommandLineOptionsBase
    {
        [Option('a', null, Required = true, HelpText = "This string option is defined A.")]
        public string OptionA { get; set; }

        [Option("option-b", HelpText = "This integer option is defined B.")]
        public int OptionB { get; set; }

        [Option('c', "option-c", HelpText = "This double option is defined C.")]
        public double OptionC { get; set; }

        [HelpOption]
        public virtual string GetUsage()
        {
            if (LastParserState.Errors.Count > 0)
            {
                return new HelpText().RenderParsingErrorsText(this, 0);
            }
            return "";
        }
    }

    public class RPEOptionsForAutoBuild : OptionsForErrorsScenario
    {
        [HelpOption]
        public override string GetUsage ()
        {
            return HelpText.AutoBuild(this, delegate(HelpText current) {
                HelpText.DefaultParsingErrorsHandler(this, current); });
        }
    }
}

