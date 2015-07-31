using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine.Tests.Fakes
{
    class MySimpleType
    {
        private readonly string value;

        public MySimpleType(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }

    class FakeOptionsWithSimpleType
    {
        [Option]
        public Uri EndPoint { get; set; }

        [Value(0)]
        public MySimpleType MyValue { get; set; }
    }
}
