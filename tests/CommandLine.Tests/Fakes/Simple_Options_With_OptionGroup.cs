namespace CommandLine.Tests.Fakes
{
    public class Simple_Options_With_OptionGroup
    {
        [Option(HelpText = "Define a string value here.", Group = "string-group")]
        public string StringValue { get; set; }

        [Option('s', "shortandlong", HelpText = "Example with both short and long name.", Group = "string-group")]
        public string ShortAndLong { get; set; }

        [Option('x', HelpText = "Define a boolean or switch value here.")]
        public bool BoolValue { get; set; }
    }
}
