namespace CommandLine.Tests.Fakes
{
    class Options_With_Nullable_Bool
    {
        [Option('b', "bool")]
        public bool Bool { get; set; }

        [Option('n', "nullable-bool")]
        public bool? NullableBool { get; set; }
    }
}
