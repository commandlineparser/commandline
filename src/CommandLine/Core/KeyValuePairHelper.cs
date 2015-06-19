// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class KeyValuePairHelper
    {
        public static KeyValuePair<string, IEnumerable<string>> Create(string value, params string[] values)
        {
            return new KeyValuePair<string, IEnumerable<string>>(value, values);
        }

        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> CreateSequence(
            IEnumerable<Token> tokens)
        {
            return from t in tokens.Pairwise(
                (f, s) =>
                        f.IsName()
                            ? KeyValuePairHelper.Create(f.Text, tokens.SkipWhile(t => t.Equals(f)).TakeWhile(v => v.IsValue()).Select(x => x.Text).ToArray())
                            : KeyValuePairHelper.Create(string.Empty))
                   where t.Key.Length > 0 && t.Value.Any()
                   select t;
        }
    }
}
