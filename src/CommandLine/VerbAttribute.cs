// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine
{
    /// <summary>
    /// Models a verb command specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    //public sealed class VerbAttribute : Attribute
    public  class VerbAttribute : Attribute
    {
        private readonly string name;
        private readonly bool isDefault;
        private Infrastructure.LocalizableAttributeProperty helpText;
        private Type resourceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.VerbAttribute"/> class.
        /// </summary>
        /// <param name="name">The long name of the verb command.</param>
        /// <param name="isDefault">Whether the verb is the default verb.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="name"/> is null, empty or whitespace and <paramref name="isDefault"/> is false.</exception>
        public VerbAttribute(string name, bool isDefault = false)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");

            this.name = name ;
            this.isDefault = isDefault;
            helpText = new Infrastructure.LocalizableAttributeProperty(nameof(HelpText));
            resourceType = null;
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
            get => helpText.Value??string.Empty;
            set => helpText.Value = value ?? throw new ArgumentNullException("value");
        }
        /// <summary>
        /// Gets or sets the <see cref="System.Type"/> that contains the resources for <see cref="HelpText"/>.
        /// </summary>
        public Type ResourceType
        {
            get => resourceType;
            set => resourceType =helpText.ResourceType = value;
        }

        /// <summary>
        /// Gets whether this verb is the default verb.
        /// </summary>
        public bool IsDefault
        {
            get => isDefault;
        }
    }
}
