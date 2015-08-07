// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Core
{
    sealed class Verb
    {
        private readonly string name;
        private readonly string helpText;

        public Verb(string name, string helpText)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (helpText == null) throw new ArgumentNullException("helpText");

            this.name = name;
            this.helpText = helpText;
        }

        public string Name
        {
            get { return name; }
        }

        public string HelpText
        {
            get { return helpText; }
        }

        public static Verb FromAttribute(VerbAttribute attribute)
        {
            return new Verb(
                attribute.Name,
                attribute.HelpText
                );
        }

        public static IEnumerable<Tuple<Verb, Type>> SelectFromTypes(IEnumerable<Type> types)
        {
            return from type in types
                   let attrs = type.GetCustomAttributes(typeof(VerbAttribute), true)
                   where attrs.Length == 1
                   select Tuple.Create(
                       FromAttribute((VerbAttribute)attrs.Single()),
                       type);
        }
    }
}