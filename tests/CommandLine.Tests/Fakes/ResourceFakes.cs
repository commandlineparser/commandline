using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine.Tests.Fakes
{
    public static class StaticResource
    {
        public static string HelpText { get { return "Localized HelpText"; } }
    }

    public class NonStaticResource
    {
        public static string HelpText { get { return "Localized HelpText"; } }
        public static string WriteOnlyText { set { value?.ToString(); } }
        private static string PrivateHelpText { get { return "Localized HelpText"; } }
    }

    public class NonStaticResource_WithNonStaticProperty
    {
        public string HelpText { get { return "Localized HelpText"; } }
    }

    internal class InternalResource
    {
        public static string HelpText { get { return "Localized HelpText"; } }
    }

}
