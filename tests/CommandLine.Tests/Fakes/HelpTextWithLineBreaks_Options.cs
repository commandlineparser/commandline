namespace CommandLine.Tests.Fakes
{
    public class HelpTextWithLineBreaks_Options
    {
        [Option(HelpText = 
            @"This is a help text description.
It has multiple lines.
We also want to ensure that indentation is correct.")]
        public string StringValue { get; set; }


        [Option(HelpText = @"This is a help text description where we want
   the left pad after a linebreak to be honoured so that
   we can sub-indent within a description.")]
        public string StringValu2 { get; set; }


        [Option(HelpText = @"This is a help text description where we want
    The left pad after a linebreak to be honoured and the indentation to be preserved across to the next line in a way that looks pleasing")]
        public string StringValu3 { get; set; }

    }
}
