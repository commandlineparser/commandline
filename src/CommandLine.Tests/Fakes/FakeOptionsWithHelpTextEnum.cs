// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    enum Shapes
    {
        Circle,
        Square,
        Triangle
    }

    class FakeOptionsWithHelpTextEnum
    {
        [Option(HelpText = "Define a string value here.")]
        public string StringValue { get; set; }

        [Option(HelpText="Define a enum value here.")]
        public Shapes Shape { get; set; }
    }
}
