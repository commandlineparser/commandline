namespace CommandLine.Tests.Fakes
{
    public class Simple_Options_With_OptionGroup_MutuallyExclusiveSet
    {
        [Option(HelpText = "Define a string value here.", Group = "test", SetName = "setname", Default = "qwerty123")]
        public string StringValue { get; set; }

        [Option('s', "shortandlong", HelpText = "Example with both short and long name.", Group = "test", SetName = "setname")]
        public string ShortAndLong { get; set; }

        [Option('x', HelpText = "Define a boolean or switch value here.")]
        public bool BoolValue { get; set; }
    }
}
