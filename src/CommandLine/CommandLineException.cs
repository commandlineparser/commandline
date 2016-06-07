using System;
#if !NETSTANDARD1_5
using System.Runtime.Serialization;
#endif

namespace CommandLine
{
#if !NETSTANDARD1_5
    [Serializable]
#endif
    public class CommandLineException : Exception
    {
        public CommandLineException()
        {
        }

        public CommandLineException(string message)
            : base(message)
        {
        }

        public CommandLineException(string message, Exception inner) 
            : base(message, inner)
        {
        }

#if !NETSTANDARD1_5
        protected CommandLineException(SerializationInfo info, StreamingContext context)
            :
            base(info, context)
        {
        }
#endif
    }
}
