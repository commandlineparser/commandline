// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequenceAndOnlyMaxConstraint
    {
        [Option('s', "string-seq", Max=3)]
        public IEnumerable<string> StringSequence { get; set; }
    }
}
