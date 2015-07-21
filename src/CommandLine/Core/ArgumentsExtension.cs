// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace CommandLine.Core
{
    static class ArgumentsExtension
    {
        public static IEnumerable<Error> Preprocess(
            this IEnumerable<string> arguments,
            IEnumerable<
                    Func<IEnumerable<string>, IEnumerable<Error>>
                > preprocessorLookup)
        {
            return preprocessorLookup.TryHead().Return(
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
