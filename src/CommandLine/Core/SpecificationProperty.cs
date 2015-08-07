// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Reflection;
using CSharpx;

namespace CommandLine.Core
{
    class SpecificationProperty
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
            if (value == null) throw new ArgumentNullException("value");

            return new SpecificationProperty(specification, property, value);
        }

        public Specification Specification
        {
            get { return specification; }
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public Maybe<object> Value
        {
            get { return value; }
        }
    }
}
