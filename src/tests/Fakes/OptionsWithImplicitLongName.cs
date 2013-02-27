using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class OptionsWithImplicitLongName
    {
        [Option]
        public string Download { get; set; }

        [Option("up-load")]
        public string Upload { get; set; }

        [Option('b')]
        public int Bytes { get; set; }

        [OptionArray]
        public int[] Offsets { get; set; }

        [OptionList]
        public IList<string> Segments { get; set; }
    }
}
