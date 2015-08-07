// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace CommandLine.Core
{
    static class KeyValuePairHelper
    {
        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ForSwitch(
            IEnumerable<Token> tokens)
        {
            return tokens.Select(t => t.Text.ToKeyValuePair("true"));
        }

        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ForScalar(
            IEnumerable<Token> tokens)
        {
            return tokens.Pairwise((f, s) => f.Text.ToKeyValuePair(s.Text));
        }

        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ForSequence(
            IEnumerable<Token> tokens)
        {
            return from t in tokens.Pairwise(
                (f, s) =>
                        f.IsName()
                            ? f.Text.ToKeyValuePair(tokens.SkipWhile(t => !t.Equals(f)).SkipWhile(t => t.Equals(f)).TakeWhile(v => v.IsValue()).Select(x => x.Text).ToArray())
                            : string.Empty.ToKeyValuePair())
                   where t.Key.Length > 0 && t.Value.Any()
                   select t;
        }

        private static KeyValuePair<string, IEnumerable<string>> ToKeyValuePair(this string value, params string[] values)
        {
            return new KeyValuePair<string, IEnumerable<string>>(value, values);
        }
    }
}
