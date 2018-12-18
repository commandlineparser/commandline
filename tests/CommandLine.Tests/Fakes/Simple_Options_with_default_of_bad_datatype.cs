using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    public class Simple_Options_with_default_of_bad_datatype
    {
        [Option(HelpText = "Define a string value here.")]
        public string StringValue { get; set; }

       
        [Option('s', "shortandlong",  HelpText = "Example with both short and long name.")]
        public long ShortAndLong { get; set; }

        //Default is boolean that mismatch datatype of the option 'IEnumerable<int>', to test exception handling
        [Option('i', Min = 3, Max = 4, Default = false, HelpText = "Define a int sequence here.")]
        public IEnumerable<int> IntSequence { get; set; }
       
    }
}