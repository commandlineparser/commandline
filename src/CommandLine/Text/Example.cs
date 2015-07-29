// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Text
{
    public sealed class Example<T>
    {
        private readonly string group;
        private readonly string helpText;
        private readonly IEnumerable<UnParserSettings> formatStyles;
        private readonly T sample;

        public Example(string group, string helpText, IEnumerable<UnParserSettings> formatStyles, T sample)
        {
            this.group = group;
            this.helpText = helpText;
            this.formatStyles = formatStyles;
            this.sample = sample;
        }

        public Example(string helpText, IEnumerable<UnParserSettings> formatStyles, T sample)
            : this(string.Empty, helpText, formatStyles, sample)
        {  
        }

        public Example(string helpText, UnParserSettings formatStyle, T sample)
            : this(string.Empty, helpText, new[] { formatStyle }, sample)
        {
        }

        public Example(string helpText, T sample)
            : this(string.Empty, helpText, Enumerable.Empty<UnParserSettings>(), sample)
        {
        }

        public string Group
        {
            get { return group }
        }

        public string HelpText
        {
            get { return helpText }
        }

        public IEnumerable<UnParserSettings> FormatStyles
        {
            get { return this.formatStyles }
        }

        public T Sample
        {
            get { return sample }
        }
    }
}
