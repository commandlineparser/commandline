using CommandLine;

namespace ReadText.Demo
{
    interface IOptions
    {
        [Option('n', "lines",
            Default = 5U,
            SetName = "bylines",
            HelpText = "Lines to be printed from the beginning or end of the file.")]
        uint? Lines { get; set; }

        [Option('c', "bytes",
            SetName = "bybytes",
            HelpText = "Bytes to be printed from the beginning or end of the file.")]
        uint? Bytes { get; set; }

        [Option('q', "quiet",
            HelpText = "Supresses summary messages.")]
        bool Quiet { get; set; }

        [Value(0, MetaName = "input file",
            HelpText = "Input file to be processed.",
            Required = true)]
        string FileName { get; set; }
    }

    [Verb("head", HelpText = "Displays first lines of a file.")]
    class HeadOptions : IOptions
    {
        public uint? Lines { get; set; }

        public uint? Bytes { get; set; }

        public bool Quiet { get; set; }

        public string FileName { get; set; }
    }

    [Verb("tail", HelpText = "Displays last lines of a file.")]
    class TailOptions : IOptions
    {
        public uint? Lines { get; set; }

        public uint? Bytes { get; set; }

        public bool Quiet { get; set; }

        public string FileName { get; set; }
    }
}