namespace CommandLine.Tests.Fakes
{
    class Options_With_Custom_Help_Option : Simple_Options
    {
        [Option('h', "help")]
        public bool Help { get; set; }
    }
}
