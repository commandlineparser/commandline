// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class KeyValuePairHelper
    {
        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ForSwitch(
            IEnumerable<Token> tokens)
        {
            return tokens.Select(t => Create(t.Text, "true"));
        }

        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ForScalar(
            IEnumerable<Token> tokens)
        {
            return tokens.Pairwise((f, s) => Create(f.Text, s.Text));
        }

        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ForSequence(
            IEnumerable<Token> tokens)
        {
            return from t in tokens.Pairwise(
                (f, s) =>
                        f.IsName()
                            ? Create(f.Text, tokens.SkipWhile(t => t.Equals(f)).TakeWhile(v => v.IsValue()).Select(x => x.Text).ToArray())
                            : Create(string.Empty))
                   where t.Key.Length > 0 && t.Value.Any()
                   select t;
        }

        private static KeyValuePair<string, IEnumerable<string>> Create(string value, params string[] values)
        {
            return new KeyValuePair<string, IEnumerable<string>>(value, values);
        }
    }
}
