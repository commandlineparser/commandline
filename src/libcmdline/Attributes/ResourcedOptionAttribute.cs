using System;
using System.Resources;

namespace CommandLine
{
    /// <summary>
    /// Models an option specification where the HelpText is in a resource file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ResourcedOptionAttribute : OptionAttribute
    {
        /// <summary>
        /// The resource manager used to retrieve the resourced strings.
        /// </summary>
        public static ResourceManager ResourceManager;

        private string helpTextKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.ResourcedOptionAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option or null if not used.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public ResourcedOptionAttribute(char shortName, string longName)
            : base(shortName, longName)
        {
        }

        /// <summary>
        /// This is the name of the string resource in the ResourceManager that should be used as HelpText.
        /// </summary>
        public string HelpTextKey
        {
            get { return helpTextKey; }
            set
            {
                if (ResourceManager == null)
                {
                    throw new InvalidOperationException("ResourcedOption: You need to assign a ResourceManager.");
                }
                helpTextKey = value;
                base.HelpText = ResourceManager.GetString(value);
            }
        }

        /// <summary>
        /// A short description of this command line option. Usually a sentence summary.
        /// You are not allowed to set this on <see cref="CommandLine.ResourcedOptionAttribute"/>.
        /// Instead, use HelpTextKey and ResourceManager.
        /// </summary>
        public new string HelpText
        {
            get { return base.HelpText; }
            set { throw new InvalidOperationException("ResourcedOption: Specify HelpTextKey instead."); }
        }
    }
}
