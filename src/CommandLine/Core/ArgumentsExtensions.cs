// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace CommandLine.Core
{
    static class ArgumentsExtensions
    {
        public static IEnumerable<Error> Preprocess(
            this IEnumerable<string> arguments,
            IEnumerable<
                    Func<IEnumerable<string>, IEnumerable<Error>>
                > preprocessorLookup)
        {
            return preprocessorLookup.TryHead().MapValueOrDefault(
                func =>
                    {
                        var errors = func(arguments);
                        return errors.Any()
                            ? errors
                            : arguments.Preprocess(preprocessorLookup.TailNoFail());
                    },
                Enumerable.Empty<Error>());
        }
    }
}
