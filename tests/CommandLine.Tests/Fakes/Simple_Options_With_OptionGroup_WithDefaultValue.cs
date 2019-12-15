namespace CommandLine.Tests.Fakes
{
    public class Simple_Options_With_OptionGroup_WithDefaultValue
    {
        [Option(HelpText = "Define a string value here.", Required = true, Group = "")]
        public string StringValue { get; set; }

        [Option('s', "shortandlong", HelpText = "Example with both short and long name.", Required = true, Group = "")]
        public string ShortAndLong { get; set; }

        [Option('x', HelpText = "Define a boolean or switch value here.")]
        public bool BoolValue { get; set; }
    }
}
