// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsForHelp
    {
        [Option('v', "verbose")]
        public bool Verbose { get; set; }

        [Option("input-file")]
        public string FileName { get; set; }
    }

    class FakeOptionsWithDescription
    {
        [Option('v', "verbose", HelpText = "Comment extensively every operation.")]
        public bool Verbose { get; set; }

        [Option('i', "input-file", Required = true, HelpText = "Specify input file to be processed.")]
        public string FileName { get; set; }
    }

    class FakeOptionsWithLongDescription
    {
        [Option('v', "verbose", HelpText = "This is the description of the verbosity to test out the wrapping capabilities of the Help Text.")]
        public bool Verbose { get; set; }

        [Option("input-file", HelpText = "This is a very long description of the Input File argument that gets passed in.  It should  be passed in as a string.")]
        public string FileName { get; set; }
    }

    class FakeOptionsWithLongDescriptionAndNoSpaces
    {
        [Option('v', "verbose", HelpText = "Before 012345678901234567890123 After")]
        public bool Verbose { get; set; }

        [Option("input-file", HelpText = "Before 012345678901234567890123456789 After")]
        public string FileName { get; set; }
    }
}
