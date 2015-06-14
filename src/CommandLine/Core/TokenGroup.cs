// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Core
{
    internal class TokenGroup
    {
        private readonly IEnumerable<KeyValuePair<string, IEnumerable<string>>> options;
        private readonly IEnumerable<string> values;
        private readonly IEnumerable<Token> errors;

        private TokenGroup(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> options,
            IEnumerable<string> values,
            IEnumerable<Token> errors)
        {
            this.options = options;
            this.values = values;
            this.errors = errors;
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Options
        {
            get { return this.options; }
        }

        public IEnumerable<string> Values
        {
            get { return this.values; }
        }

        public IEnumerable<Token> Errors
        {
            get { return this.errors; }
        }

        public static TokenGroup Create(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> options,
            IEnumerable<string> values,
            IEnumerable<Token> errors)
        {
            return new TokenGroup(options, values, errors);
        }
    }
}
