// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLine.Core
{
    sealed class Verb
    {
        public Verb(string name, string helpText, bool hidden, bool isDefault, string[] aliases)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;

            HelpText = helpText ?? throw new ArgumentNullException(nameof(helpText));
            Hidden = hidden;
            IsDefault = isDefault;
            Aliases = aliases ?? new string[0];
        }

        public string Name { get; private set; }

        public string HelpText { get; private set; }

        public bool Hidden { get; private set; }

        public bool IsDefault { get; private set; }

        public string[] Aliases { get; private set; }

        public static Verb FromAttribute(VerbAttribute attribute)
        {
            return new Verb(
                attribute.Name,
                attribute.HelpText,
                attribute.Hidden,
                attribute.IsDefault,
                attribute.Aliases
                );
        }

        public static IEnumerable<Tuple<Verb, Type>> SelectFromTypes(IEnumerable<Type> types)
        {
            return from type in types
                   let attrs = type.GetTypeInfo().GetCustomAttributes(typeof(VerbAttribute), true).ToArray()
                   where attrs.Length == 1
                   select Tuple.Create(
                       FromAttribute((VerbAttribute)attrs.Single()),
                       type);
        }
    }
}
