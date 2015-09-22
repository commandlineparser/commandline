// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine.Tests.Fakes
{
    public class Options_With_Guid
    {
        [Option('t', "txid")]
        public Guid TransactionId { get; set; }
    }
}
