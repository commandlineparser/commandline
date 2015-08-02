// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine.Text
{
    /// <summary>
    /// Applied to a static property that yields a sequence of <see cref="CommandLine.Text.Example"/>,
    /// provides data to render usage section of help screen.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class UsageAttribute : Attribute
    {
        /// <summary>
        /// Application name, script or any means that starts current program.
        /// </summary>
        public string ApplicationAlias { get; set; }
    }
}
