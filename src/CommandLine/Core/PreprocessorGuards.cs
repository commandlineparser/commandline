// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Core
{
    static class PreprocessorGuards
    {
        public static IEnumerable<Func<IEnumerable<Token>, IEnumerable<Error>>>
            Lookup(StringComparer nameComparer, bool autoHelp, bool autoHelpShortName, bool autoVersion, bool autoVersionShortName)
        {
            var list = new List<Func<IEnumerable<Token>, IEnumerable<Error>>>();
            if (autoHelp)
                list.Add(HelpCommand(nameComparer));
            if (autoHelp && autoHelpShortName)
                list.Add(ShortHelpCommand(nameComparer));
            if (autoVersion)
                list.Add(VersionCommand(nameComparer));
            if (autoVersion && autoVersionShortName)
                list.Add(ShortVersionCommand(nameComparer));
            return list;
        }

        public static Func<IEnumerable<Token>, IEnumerable<Error>> HelpCommand(StringComparer nameComparer)
        {
            return
                arguments =>
                    arguments.OfType<Name>().Any(arg => nameComparer.Equals("help", arg.Text))
                        ? new Error[] { new HelpRequestedError() }
                        : Enumerable.Empty<Error>();
        }

        public static Func<IEnumerable<Token>, IEnumerable<Error>> ShortHelpCommand(StringComparer nameComparer)
        {
            return
                arguments =>
                    arguments.OfType<Name>().Any(arg => nameComparer.Equals("h", arg.Text))
                        ? new Error[] { new HelpRequestedError() }
                        : Enumerable.Empty<Error>();
        }

        public static Func<IEnumerable<Token>, IEnumerable<Error>> VersionCommand(StringComparer nameComparer)
        {
            return
                arguments =>
                    arguments.OfType<Name>().Any(arg => nameComparer.Equals("version", arg.Text))
                        ? new Error[] { new VersionRequestedError() }
                        : Enumerable.Empty<Error>();
        }

        public static Func<IEnumerable<Token>, IEnumerable<Error>> ShortVersionCommand(StringComparer nameComparer)
        {
            return
                arguments =>
                    arguments.OfType<Name>().Any(arg => nameComparer.Equals("V", arg.Text))  // Uppercase V
                        ? new Error[] { new VersionRequestedError() }
                        : Enumerable.Empty<Error>();
        }
    }
}
