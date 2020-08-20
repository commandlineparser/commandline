// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    public class Options_With_Similar_Names
    {
        [Option("deploy", Separator = ',', HelpText= "Projects to deploy")]
        public IEnumerable<string> Deploys { get; set; }

        [Option("profile", Required = true, HelpText = "Profile to use when restoring and publishing")]
        public string Profile { get; set; }

        [Option("configure-profile", Required = true, HelpText = "Profile to use for Configure")]
        public string ConfigureProfile { get; set; }
    }

    public class Options_With_Similar_Names_And_Separator
    {
        [Option('f', "flag", HelpText = "Flag")]
        public bool Flag { get; set; }

        [Option('c', "categories", Required = false, Separator = ',', HelpText = "Categories")]
        public IEnumerable<string> Categories { get; set; }

        [Option('j', "jobId", Required = true, HelpText = "Texts.ExplainJob")]
        public int JobId { get; set; }
    }

}
