// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace CommandLine.Core
{
    sealed class OptionSpecification : Specification
    {
        private readonly string shortName;
        private readonly string longName;
        private readonly char separator;
        private readonly string setName;
        private readonly string group;
        private readonly bool flagCounter;

        public OptionSpecification(string shortName, string longName, bool required, string setName, Maybe<int> min, Maybe<int> max,
            char separator, Maybe<object> defaultValue, string helpText, string metaValue, IEnumerable<string> enumValues,
            Type conversionType, TargetType targetType, string group, bool flagCounter, bool hidden)
            : base(SpecificationType.Option,
                 required, min, max, defaultValue, helpText, metaValue, enumValues, conversionType, targetType, hidden)
        {
            this.shortName = shortName;
            this.longName = longName;
            this.separator = separator;
            this.setName = setName;
            this.group = group;
            this.flagCounter = flagCounter;
        }

        public static OptionSpecification FromAttribute(OptionAttribute attribute, Type conversionType, IEnumerable<string> enumValues)
        {
            return new OptionSpecification(
                attribute.ShortName,
                attribute.LongName,
                attribute.Required,
                attribute.SetName,
                attribute.Min == -1 ? Maybe.Nothing<int>() : Maybe.Just(attribute.Min),
                attribute.Max == -1 ? Maybe.Nothing<int>() : Maybe.Just(attribute.Max),
                attribute.Separator,
                attribute.Default.ToMaybe(),
                attribute.HelpText,
                attribute.MetaValue,
                enumValues,
                conversionType,
                conversionType.ToTargetType(),
                attribute.Group,
                attribute.FlagCounter,
                attribute.Hidden);
        }

        public static OptionSpecification NewSwitch(string shortName, string longName, bool required, string helpText, string metaValue, bool flagCounter, bool hidden)
        {
            return new OptionSpecification(shortName, longName, required, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(),
                '\0', Maybe.Nothing<object>(), helpText, metaValue, Enumerable.Empty<string>(), typeof(bool), TargetType.Switch, string.Empty, flagCounter, hidden);
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

        public string Group
        {
            get { return group; }
        }

        public bool FlagCounter
        {
            get { return flagCounter; }
        }
    }
}
