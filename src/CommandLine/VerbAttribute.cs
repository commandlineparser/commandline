// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine
{
    /// <summary>
    /// Models a verb command specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public sealed class VerbAttribute : Attribute
    {
        private readonly string name;
        private string helpText;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.VerbAttribute"/> class.
        /// </summary>
        /// <param name="name">The long name of the verb command.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="name"/> is null, empty or whitespace.</exception>
        public VerbAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");

            this.name = name;
            this.helpText = string.Empty;
        }

        /// <summary>
        /// Gets the verb name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a command line verb is visible in the help text.
        /// </summary>
        public bool Hidden
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a short description of this command line option. Usually a sentence summary. 
        /// </summary>
        public string HelpText
        {
            get { return helpText; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                helpText = value;
            }
        }
    }
}