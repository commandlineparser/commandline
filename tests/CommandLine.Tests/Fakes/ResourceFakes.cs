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
        internal static string InternalText { get { return "Internal Text"; } }
        private static string PrivateText { get { return "Private Text"; } }
    }

    public class NonStaticResource
    {
        public static string HelpText { get { return "Localized HelpText"; } }
        public static string WriteOnlyText { set { } }
        public string InstanceText { get { return "Instance Text"; } }
    }

    internal class InternalResource
    {
        public static string Text { get { return "Localized Text"; } }
    }
}
