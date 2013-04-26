// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSets
    {
        [Option(SetName = "web")]
        public string WebUrl { get; set; }

        [Option(SetName = "web")]
        public int MaxLinks { get; set; }

        [Option(SetName = "ftp")]
        public string FtpUrl { get; set; }

        [Option(SetName = "ftp")]
        public int MaxFiles { get; set; }

    }
}
