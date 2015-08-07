// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    class Options_With_Two_Option_Required_Set_To_True_And_Two_Sets
    {
        [Option(SetName = "web", Required = true)]
        public string WebUrl { get; set; }

        [Option(SetName = "ftp", Required = true)]
        public string FtpUrl { get; set; }

        [Option('a')]
        public bool FetchAsync { get; set; }
    }
}
