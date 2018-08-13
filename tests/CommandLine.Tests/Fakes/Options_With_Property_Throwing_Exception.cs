using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine.Tests.Fakes
{
    class Options_With_Property_Throwing_Exception
    {
        private string optValue;

        [Option('e')]
        public string OptValue
        {
            get
            {
                return optValue;
            }
            set
            {
                if (value != "good")
                    throw new ArgumentException("Invalid value, only accept 'good' value");

                optValue = value;
            }
        }
    }
}
