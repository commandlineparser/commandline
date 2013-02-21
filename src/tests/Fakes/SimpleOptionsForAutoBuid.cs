using System;
using CommandLine;
using CommandLine.Text;

namespace CommandLine.Tests.Fakes
{
    class SimpleOptionsForAutoBuid : SimpleOptions
    {
        [Option('m', "mock", Required=true, HelpText="Force required.")]
        public bool MockOption { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}

