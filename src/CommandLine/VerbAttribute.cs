// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CommandLine
{
    /// <summary>
    /// Models a verb command specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    //public sealed class VerbAttribute : Attribute
    public class VerbAttribute : Attribute
    {
        private readonly Infrastructure.LocalizableAttributeProperty helpText;

#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
            DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
        private Type resourceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.VerbAttribute"/> class.
        /// </summary>
        /// <param name="name">The long name of the verb command.</param>
        /// <param name="isDefault">Whether the verb is the default verb.</param>
        /// <param name="aliases">aliases for this verb. i.e. "move" and "mv"</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="name"/> is null, empty or whitespace and <paramref name="isDefault"/> is false.</exception>
        public VerbAttribute(string name, bool isDefault = false, string[] aliases = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");

            Name = name;
            IsDefault = isDefault;
            helpText = new Infrastructure.LocalizableAttributeProperty(nameof(HelpText));
            resourceType = null;
            Aliases = aliases ?? new string[0];
        }

        /// <summary>
        /// Gets the verb name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether a command line verb is visible in the help text.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets a short description of this command line option. Usually a sentence summary.
        /// </summary>
        public string HelpText
        {
            get => helpText.Value ?? string.Empty;
            set => helpText.Value = value ?? throw new ArgumentNullException("value");
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Type"/> that contains the resources for <see cref="HelpText"/>.
        /// </summary>
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
            DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
        public Type ResourceType
        {
            get => resourceType;
            set => resourceType = helpText.ResourceType = value;
        }

        /// <summary>
        /// Gets whether this verb is the default verb.
        /// </summary>
        public bool IsDefault { get; private set; }

        /// <summary>
        /// Gets or sets the aliases
        /// </summary>
        public string[] Aliases { get; private set; }
    }
}
