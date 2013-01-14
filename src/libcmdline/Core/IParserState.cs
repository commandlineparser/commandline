using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine
{
    /// <summary>
    /// Represents the parser state.
    /// </summary>
    public interface IParserState
    {
        /// <summary>
        /// Errors occurred during parsing.
        /// </summary>
        IList<ParsingError> Errors { get; }
    }
}