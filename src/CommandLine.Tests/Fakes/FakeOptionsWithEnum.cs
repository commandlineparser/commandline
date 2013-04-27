using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    enum Colors
    {
        Red,
        Green,
        Blue
    }

    class FakeOptionsWithEnum
    {
        [Option]
        public Colors Colors { get; set; }
    }
}
