using System;
using CommandLine.Core;

namespace CommandLine
{

    /// <summary>
    /// The exception thrown when an error occurs during Parser operations.
    /// </summary>

    public class CommandLineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineException"/> class.
        /// </summary>
        public CommandLineException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public CommandLineException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
        public CommandLineException(string message, Exception innerException)
                : base(message, innerException)
        {
        }



        /// <summary>
        /// Create  a new instance of the <see cref="CommandLineException"/> class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specProp">The Source that raise the Exception</param>
        /// <param name="ex">The exception that is the cause of the current exception</param>
        /// <returns></returns>
        internal static CommandLineException Create<T>( SpecificationProperty specProp, Exception ex)
        {
            var message =
                $"Fail to SetProperties: Class= '{typeof(T)}', PropertyName='{specProp.Property.Name}', NameText= '{specProp.Specification.FromSpecification().NameText}'. Show InnerException for more information.";

            return new CommandLineException(message, ex);
        }
       
    }
}

