// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal sealed class OptionSpecification : Specification
    {
        private readonly string shortName;
        private readonly string longName;
        private readonly string setName;
        private readonly string helpText;
        private readonly string metaValue;
        private readonly System.Collections.Generic.IEnumerable<string> enumValues;

        public OptionSpecification(string shortName, string longName, bool required, string setName, int min, int max, Maybe<object> defaultValue, System.Type conversionType, string helpText, string metaValue, System.Collections.Generic.IEnumerable<string> enumValues)
            : base(SpecificationType.Option, required, min, max, defaultValue, conversionType)
        {
            this.shortName = shortName;
            this.longName = longName;
            this.setName = setName;
            this.helpText = helpText;
            this.metaValue = metaValue;
            this.enumValues = enumValues;
        }

        public static OptionSpecification FromAttribute(OptionAttribute attribute, System.Type conversionType, System.Collections.Generic.IEnumerable<string> enumValues)
        {
            return new OptionSpecification(
                attribute.ShortName,
                attribute.LongName,
                attribute.Required,
                attribute.SetName,
                attribute.Min,
                attribute.Max,
                attribute.DefaultValue.ToMaybe(),
                conversionType,
                attribute.HelpText,
                attribute.MetaValue,
                enumValues);
        }

        public string ShortName
        {
            get { return this.shortName; }
        }

        public string LongName
        {
            get { return this.longName; }
        }

        public string SetName
        {
            get { return this.setName; }
        }

        public string HelpText
        {
            get { return this.helpText; }
        }

        public string MetaValue
        {
            get { return this.metaValue; }
        }

        public System.Collections.Generic.IEnumerable<string> EnumValues
        {
            get { return this.enumValues; }
        }
    }
}