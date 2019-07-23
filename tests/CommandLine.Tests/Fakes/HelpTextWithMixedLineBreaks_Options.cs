namespace CommandLine.Tests.Fakes
{
    public class HelpTextWithMixedLineBreaks_Options
    {
        [Option(HelpText = 
            "This is a help text description\n  It has multiple lines.\r\n  Third line")]
        public string StringValue { get; set; }
    }
}
