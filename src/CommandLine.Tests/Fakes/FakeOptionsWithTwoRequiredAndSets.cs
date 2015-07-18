// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithTwoRequiredAndSets
    {
        [Option(SetName = "web", Required = true)]
        public string WebUrl { get; set; }

        [Option(SetName = "ftp", Required = true)]
        public string FtpUrl { get; set; }

        [Option('a')]
        public bool FetchAsync { get; set; }
    }
}
