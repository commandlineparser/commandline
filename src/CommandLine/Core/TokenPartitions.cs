// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Core
{
    sealed class TokenPartitions
    {
        private readonly IEnumerable<KeyValuePair<string, IEnumerable<string>>> options;
        private readonly IEnumerable<string> values;
        private readonly IEnumerable<Token> errors;

        private TokenPartitions(
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
            get { return options; }
        }

        public IEnumerable<string> Values
        {
            get { return values; }
        }

        public IEnumerable<Token> Errors
        {
            get { return errors; }
        }

        public static TokenPartitions Create(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> options,
            IEnumerable<string> values,
            IEnumerable<Token> errors)
        {
            return new TokenPartitions(options, values, errors);
        }
    }
}
