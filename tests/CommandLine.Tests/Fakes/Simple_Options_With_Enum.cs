// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    public enum Colors
    {
        Red,
        Green,
        Blue
    }

    class Simple_Options_With_Enum
    {
        [Option]
        public Colors Colors { get; set; }
    }
}
