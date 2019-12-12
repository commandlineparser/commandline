using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    public class Simple_Options_With_Multiple_OptionGroups
    {
        [Option(HelpText = "Define a string value here.", Group = "string-group")]
        public string StringValue { get; set; }

        [Option('s', "shortandlong", HelpText = "Example with both short and long name.", Group = "string-group")]
        public string ShortAndLong { get; set; }

        [Option('x', HelpText = "Define a boolean or switch value here.", Group = "second-group")]
        public bool BoolValue { get; set; }

        [Option('i', Min = 3, Max = 4, HelpText = "Define a int sequence here.", Group = "second-group")]
        public IEnumerable<int> IntSequence { get; set; }
    }
}
