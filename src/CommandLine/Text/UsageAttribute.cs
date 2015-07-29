// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace CommandLine.Text
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class UsageAttribute : Attribute
    {
        private readonly IDictionary<string, string> groups;

        public UsageAttribute(IDictionary<string, string> groups)
        {
            this.groups = groups;
        }

        public UsageAttribute()
            : this(new Dictionary<string, string>())
        {
        }

        public IDictionary<string, string> Groups
        {
            get { return groups; }
        }
    }
}
