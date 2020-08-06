using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    public class Options_With_Option_Sequence_And_Value_Sequence
    {
        [Option('o', "option-seq")]
        public IEnumerable<string> OptionSequence { get; set; }

        [Value(0)]
        public IEnumerable<string> ValueSequence { get; set; }
    }
}
