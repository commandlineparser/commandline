using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace ReadText.LocalizedDemo
{
    interface IOptions
    {
        [Option('n', "lines",
            Default = 5U,
            SetName = "bylines",
            HelpText = "HelpTextLines",
            ResourceType = typeof(Properties.Resources))]
        uint? Lines { get; set; }

        [Option('c', "bytes",
            SetName = "bybytes",
            HelpText = "HelpTextBytes",
            ResourceType = typeof(Properties.Resources))]
        uint? Bytes { get; set; }

        [Option('q', "quiet",
            HelpText = "HelpTextQuiet",
            ResourceType = typeof(Properties.Resources))]
        bool Quiet { get; set; }

        [Value(0, MetaName = "input file",
            HelpText = "HelpTextFileName",
            Required = true,
            ResourceType = typeof(Properties.Resources))]
        string FileName { get; set; }
    }

    [Verb("head", HelpText = "HelpTextVerbHead", ResourceType = typeof(Properties.Resources))]
    class HeadOptions : IOptions
    {
        public uint? Lines { get; set; }

        public uint? Bytes { get; set; }

        public bool Quiet { get; set; }

        public string FileName { get; set; }

        [Usage(ApplicationAlias = "ReadText.LocalizedDemo.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(Properties.Resources.ExamplesNormalScenario, new HeadOptions { FileName = "file.bin"});
                yield return new Example(Properties.Resources.ExamplesSpecifyBytes, new HeadOptions { FileName = "file.bin", Bytes=100 });
                yield return new Example(Properties.Resources.ExamplesSuppressSummary, UnParserSettings.WithGroupSwitchesOnly(), new HeadOptions { FileName = "file.bin", Quiet = true });
                yield return new Example(Properties.Resources.ExamplesReadMoreLines, new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly() }, new HeadOptions { FileName = "file.bin", Lines = 10 });
            }
        }
    }

    [Verb("tail", HelpText = "HelpTextVerbTail", ResourceType = typeof(Properties.Resources))]
    class TailOptions : IOptions
    {
        public uint? Lines { get; set; }

        public uint? Bytes { get; set; }

        public bool Quiet { get; set; }

        public string FileName { get; set; }
    }
}
