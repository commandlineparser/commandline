// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

#if !SKIP_FSHARP
using Microsoft.FSharp.Core;

namespace CommandLine.Tests.Fakes
{
    public class Options_With_FSharpOption
    {
        [Option]
        public FSharpOption<string> FileName { get; set; }

        [Value(0)]
        public FSharpOption<int> Offset { get; set; }
    }
}
#endif
