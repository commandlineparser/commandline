// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace CommandLine.Text
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class UsageAttribute : Attribute
    {
        public string ApplicationAlias { get; set; }
    }
}
