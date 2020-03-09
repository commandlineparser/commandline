// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine.Tests.Fakes
{
    public class CustomStructOptions
    {
        [Option('c', "custom", HelpText = "Custom Type")]
        public CustomStruct Custom { get; set; }
    }

    public struct CustomStruct
    {
        public string Input { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public CustomStruct(string url)
        {
            Input = url;
            Server = "";
            Port = 80;
            var data = url.Split(':');
            if (data.Length == 2)
            {
                Server = data[0];
                Port = Convert.ToInt32(data[1]);
            }
        }
    }

    public class CustomClassOptions
    {
        [Option('c', "custom", HelpText = "Custom Type")]
        public CustomClass Custom { get; set; }
    }

    public class CustomClass
    {
        public string Input { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public CustomClass(string url)
        {
            Input = url;
            Server = "";
            Port = 80;
            var data = url.Split(':');
            if (data.Length == 2)
            {
                Server = data[0];
                Port = Convert.ToInt32(data[1]);
            }
        }
    }
}
