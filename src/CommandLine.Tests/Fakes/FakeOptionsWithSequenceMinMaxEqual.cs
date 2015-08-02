// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequenceMinMaxEqual
    {
        [Value(0, Min=2, Max=2)]
        public IEnumerable<string> StringSequence { get; set; }
    }
}
