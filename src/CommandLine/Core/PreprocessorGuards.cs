// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    static class PreprocessorGuards
    {
        public static IEnumerable<Func<IEnumerable<string>, IEnumerable<Error>>>
            Lookup(StringComparer nameComparer)
        {
            return new List<Func<IEnumerable<string>, IEnumerable<Error>>>
                {
                    HelpCommand(nameComparer)
                };
        }

        public static Func<IEnumerable<string>, IEnumerable<Error>> HelpCommand(StringComparer nameComparer)
        {
            return
                arguments =>
                    nameComparer.Equals("--help", arguments.First())
                        ? new Error[] { new HelpRequestedError() }
                        : Enumerable.Empty<Error>();
        }
    }
}
