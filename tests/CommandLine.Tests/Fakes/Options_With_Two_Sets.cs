// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    class Options_With_Two_Sets
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
