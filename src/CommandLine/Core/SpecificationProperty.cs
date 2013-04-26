// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Reflection;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal class SpecificationProperty
    {
        private readonly Specification specification;
        private readonly PropertyInfo property;
        private readonly Maybe<object> value;

        private SpecificationProperty(Specification specification, PropertyInfo property, Maybe<object> value)
        {
            this.property = property;
            this.specification = specification;
            this.value = value;
        }

        public static SpecificationProperty Create(Specification specification, PropertyInfo property, Maybe<object> value)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            if (property == null) throw new ArgumentNullException("property");
            if (value == null) throw new ArgumentNullException("value");

            return new SpecificationProperty(specification, property, value);
        }

        public Specification Specification
        {
            get { return this.specification; }
        }

        public PropertyInfo Property
        {
            get { return this.property; }
        }

        public Maybe<object> Value
        {
            get { return this.value; }
        }
    }
}
