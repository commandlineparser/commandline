using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Text
{
    public sealed class Example<T>
    {
        private readonly string group;
        private readonly string helpText;
        private readonly IEnumerable<UnParserSettings> settings;
        private readonly T sample;

        public Example(string group, string helpText, IEnumerable<UnParserSettings> settings, T sample)
        {
            this.group = group;
            this.helpText = helpText;
            this.settings = settings;
            this.sample = sample;
        }
    }
}
