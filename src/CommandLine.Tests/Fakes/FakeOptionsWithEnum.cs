// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    public enum Colors
    {
        Red,
        Green,
        Blue
    }

    class FakeOptionsWithEnum
    {
        [Option]
        public Colors Colors { get; set; }
    }
}
