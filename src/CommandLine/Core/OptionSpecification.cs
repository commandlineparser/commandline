// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Collections.Generic;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal sealed class OptionSpecification : Specification
    {
        private readonly string shortName;
        private readonly string longName;
        private readonly char separator;
        private readonly string setName;
        private readonly string helpText;
        private readonly string metaValue;
        private readonly IEnumerable<string> enumValues;

        public OptionSpecification(string shortName, string longName, bool required, string setName, int min, int max, char separator, Maybe<object> defaultValue, System.Type conversionType, string helpText, string metaValue, IEnumerable<string> enumValues)
            : base(SpecificationType.Option, required, min, max, defaultValue, conversionType)
        {
            this.shortName = shortName;
            this.longName = longName;
            this.separator = separator;
            this.setName = setName;
            this.helpText = helpText;
            this.metaValue = metaValue;
            this.enumValues = enumValues;
        }

        public static OptionSpecification FromAttribute(OptionAttribute attribute, System.Type conversionType, IEnumerable<string> enumValues)
        {
            return new OptionSpecification(
                attribute.ShortName,
                attribute.LongName,
                attribute.Required,
                attribute.SetName,
                attribute.Min,
                attribute.Max,
                attribute.Separator,
                attribute.DefaultValue.ToMaybe(),
                conversionType,
                attribute.HelpText,
                attribute.MetaValue,
                enumValues);
        }

        public string ShortName
        {
            get { return shortName; }
        }

        public string LongName
        {
            get { return longName; }
        }

        public char Separator
        {
            get { return separator; }
        }

        public string SetName
        {
            get { return setName; }
        }

        public string HelpText
        {
            get { return helpText; }
        }

        public string MetaValue
        {
            get { return metaValue; }
        }

        public IEnumerable<string> EnumValues
        {
            get { return enumValues; }
        }
    }
}