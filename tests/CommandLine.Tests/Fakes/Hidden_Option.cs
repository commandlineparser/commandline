using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine.Tests.Fakes
{
    public class Hidden_Option
    {
        [Option('h', "hiddenOption",  Hidden = true)]
        public string HiddenOption { get; set; }
    }
}
