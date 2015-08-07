// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    enum Shapes
    {
        Circle,
        Square,
        Triangle
    }

    class Options_With_Enum_Having_HelpText
    {
        [Option(HelpText = "Define a string value here.")]
        public string StringValue { get; set; }

        [Option(HelpText="Define a enum value here.")]
        public Shapes Shape { get; set; }
    }
}
