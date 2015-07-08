using System;
using CommandLine;

namespace ReadText.Demo
{
    abstract class Options
    {
        [Option('q', "quiet",
                HelpText = "Supresses summary messages.")]
        public bool Quiet { get; set; }

        [Value(0)]
        public string FileName { get; set; }
    }

    [Verb("head", HelpText = "Displays first lines of a file.")]
    class HeadOptions : Options
    {
        [Option('n', "lines",
                DefaultValue = 10,
                SetName = "amount",
                HelpText = "Lines to be printed from the beginning of the file (default 10).")]
        public uint Lines { get; set; }

        [Option('c', "bytes",
                SetName = "amount",
                HelpText = "Bytes to be printed from the beginning of the file.")]
        public uint Bytes { get; set; }
    }

    [Verb("tail", HelpText = "Displays last lines of a file.")]
    class TailOptions : Options
    {
        [Option('n', "lines",
                DefaultValue = 10,
                SetName = "amount",
                HelpText = "Lines to be printed from the end of the file (default 10).")]
        public uint Lines { get; set; }

        [Option('c', "bytes",
                SetName = "amount",
                HelpText = "Bytes to be printed from the end of the file.")]
        public uint Bytes { get; set; }
    }
}