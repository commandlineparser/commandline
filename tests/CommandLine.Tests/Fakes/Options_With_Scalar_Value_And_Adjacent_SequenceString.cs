// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    public class Options_With_Scalar_Value_And_Adjacent_SequenceString
    {
        [Value(0)]
        public string StringValueWithIndexZero { get; set; }

        [Option('s', "strseq")]
        public IEnumerable<string> StringOptionSequence { get; set; }
    }
}
