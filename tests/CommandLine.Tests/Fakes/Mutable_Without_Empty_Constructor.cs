// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes 
{
    class Mutable_Without_Empty_Constructor 
    {
        [Option("amend", HelpText = "Used to amend the tip of the current branch.")]
        public bool Amend { get; set; }

        private Mutable_Without_Empty_Constructor() 
        {
        }

        public static Mutable_Without_Empty_Constructor Create() 
        {
            return new Mutable_Without_Empty_Constructor();
        }
    }
}
