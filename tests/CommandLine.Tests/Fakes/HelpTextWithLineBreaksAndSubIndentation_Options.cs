namespace CommandLine.Tests.Fakes
{
    public class HelpTextWithLineBreaksAndSubIndentation_Options
    {

        [Option(HelpText = @"This is a help text description where we want:
    * The left pad after a linebreak to be honoured and the indentation to be preserved across to the next line
    * The ability to return to no indent.
Like this.")]
        public string StringValue { get; set; }

    }
}