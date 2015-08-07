// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    public class Options_With_Required_Set_To_True_For_Values
    {
        [Value(0, Required = true)]
        public string StringValue { get; set; }

        [Value(1)]
        public int IntValue { get; set; }
    }
}
