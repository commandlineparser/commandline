// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLine.Core
{
    sealed class Verb
    {
        private readonly string name;
        private readonly string helpText;
        private readonly bool hidden;
        private readonly bool isDefault;

        public Verb(string name, string helpText, bool hidden = false, bool isDefault = false)
        {
            if ( string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            this.name = name;

            this.helpText = helpText ?? throw new ArgumentNullException(nameof(helpText));
            this.hidden = hidden;
            this.isDefault = isDefault;
        }

        public string Name
        {
            get { return name; }
        }

        public string HelpText
        {
            get { return helpText; }
        }

        public bool Hidden
        {
            get { return hidden; }
        }

        public bool IsDefault
        {
            get => isDefault;
        }

        public static Verb FromAttribute(VerbAttribute attribute)
        {
            return new Verb(
                attribute.Name,
                attribute.HelpText,
                attribute.Hidden,
                attribute.IsDefault
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
