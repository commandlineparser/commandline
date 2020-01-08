namespace CommandLine.Tests.Fakes
{
    public class Hidden_Option
    {
        [Option('h', "hiddenOption",  Hidden = true)]
        public string HiddenOption { get; set; }
    }
}
