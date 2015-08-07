// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    public class Options_With_Required_Set_To_True_Within_Same_Set {
        [Option("ftpurl", SetName = "SetA", Required = true)]
        public string FtpUrl { get; set; }

        [Option("weburl", SetName = "SetA",  Required = true)]
        public string WebUrl { get; set; }
    }
}