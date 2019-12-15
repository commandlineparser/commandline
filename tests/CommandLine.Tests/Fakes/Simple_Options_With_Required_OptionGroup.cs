namespace CommandLine.Tests.Fakes
{
    public class Simple_Options_With_Required_OptionGroup
    {
        [Option(HelpText = "Define a string value here.", Required = true, Group = "string-group")]
        public string StringValue { get; set; }

        [Option('s', "shortandlong", HelpText = "Example with both short and long name.", Required = true, Group = "string-group")]
        public string ShortAndLong { get; set; }

        [Option('x', HelpText = "Define a boolean or switch value here.")]
        public bool BoolValue { get; set; }
    }
}
