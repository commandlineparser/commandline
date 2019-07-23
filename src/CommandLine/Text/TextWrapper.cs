using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Infrastructure;

namespace CommandLine.Text
{
    /// <summary>
    /// A utility class to word-wrap and indent blocks of text
    /// </summary>
    public class TextWrapper
    {
        private string[] lines;
        public TextWrapper(string input)
        {
            //start by splitting at newlines and then reinserting the newline as a separate word
            //Note that on the input side, we can't assume the line-break style at run time so we have to
            //be able to handle both.  We can't use Environment.NewLine because that changes at
            //_runtime_ and may not match the line-break style that was compiled in
            lines = input
                .Replace("\r","")
                .Split(new[] {'\n'}, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits a string into a words and performs wrapping while also preserving line-breaks and sub-indentation
        /// </summary>
        /// <param name="columnWidth">The number of characters we can use for text</param>
        /// <remarks>
        /// This method attempts to wrap text without breaking words 
        /// For example, if columnWidth is 10 , the input
        /// "a string for wrapping 01234567890123"
        /// would return
        /// "a string
        /// "for 
        /// "wrapping
        /// "0123456789
        /// "0123"          
        /// </remarks>
        /// <returns>this</returns>
        public TextWrapper WordWrap(int columnWidth)
        {
            //ensure we always use at least 1 column even if the client has told us there's no space available
            columnWidth = Math.Max(1, columnWidth);
            lines= lines
                .SelectMany(line => WordWrapLine(line, columnWidth))
                .ToArray();
            return this;
        }

        /// <summary>
        /// Indent all lines in the TextWrapper by the desired number of spaces
        /// </summary>
        /// <param name="numberOfSpaces">The number of spaces to indent by</param>
        /// <returns>this</returns>
        public TextWrapper Indent(int numberOfSpaces)
        {
            lines = lines
                .Select(line => numberOfSpaces.Spaces() + line)
                .ToArray();
            return this;
        }

        /// <summary>
        /// Returns the current state of the TextWrapper as a string
        /// </summary>
        /// <returns></returns>
        public string ToText()
        {
            //return the whole thing as a single string
            return string.Join(Environment.NewLine,lines);
        }

        /// <summary>
        /// Convenience method to wraps and indent a string in a single operation
        /// </summary>
        /// <param name="input">The string to operate on</param>
        /// <param name="indentLevel">The number of spaces to indent by</param>
        /// <param name="columnWidth">The width of the column used for wrapping</param>
        /// <remarks>
        /// The string is wrapped _then_ indented so the columnWidth is the width of the
        /// usable text block, and does NOT include the indentLevel.
        /// </remarks>
        /// <returns>the processed string</returns>
        public static string WrapAndIndentText(string input, int indentLevel,int columnWidth)
        {
            return new TextWrapper(input)
                .WordWrap(columnWidth)
                .Indent(indentLevel)
                .ToText();
        }


        private string [] WordWrapLine(string line,int columnWidth)
        {
            //create a list of individual lines generated from the supplied line

            //When handling sub-indentation we must always reserve at least one column for text!
            var unindentedLine = line.TrimStart();
            var currentIndentLevel = Math.Min(line.Length - unindentedLine.Length,columnWidth-1) ;
            columnWidth -= currentIndentLevel;

            return unindentedLine.Split(' ')
                .Aggregate(
                    new List<StringBuilder>(),
                    (lineList, word) => AddWordToLastLineOrCreateNewLineIfNecessary(lineList, word, columnWidth)
                )
                .Select(builder => currentIndentLevel.Spaces()+builder.ToString().TrimEnd())
                .ToArray();
        }

        /// <summary>
        /// When presented with a word, either append to the last line in the list or start a new line
        /// </summary>
        /// <param name="lines">A list of StringBuilders containing results so far</param>
        /// <param name="word">The individual word to append</param>
        /// <param name="columnWidth">The usable text space</param>
        /// <remarks>
        /// The 'word' can actually be an empty string.  It's important to keep these -
        /// empty strings allow us to preserve indentation and extra spaces within a line.
        /// </remarks>
        /// <returns>The same list as is passed in</returns>
        private static List<StringBuilder> AddWordToLastLineOrCreateNewLineIfNecessary(List<StringBuilder> lines, string word,int columnWidth)
        {
            //The current indentation level is based on the previous line but we need to be careful
            var previousLine = lines.LastOrDefault()?.ToString() ??string.Empty;
            
            var wouldWrap = !lines.Any() || (word.Length>0 && previousLine.Length + word.Length > columnWidth);
          
            if (!wouldWrap)
            {
                //The usual case is we just append the 'word' and a space to the current line
                //Note that trailing spaces will get removed later when we turn the line list 
                //into a single string
                lines.Last().Append(word + ' ');
            }
            else
            {
                //The 'while' here is to take account of the possibility of someone providing a word
                //which just can't fit in the current column.  In that case we just split it at the 
                //column end.
                //That's a rare case though - most of the time we'll succeed in a single pass without
                //having to split
                //Note that we always do at least one pass even if the 'word' is empty in order to 
                //honour sub-indentation and extra spaces within strings
                do
                {
                    var availableCharacters = Math.Min(columnWidth, word.Length);
                    var segmentToAdd = LeftString(word,availableCharacters) + ' ';
                    lines.Add(new StringBuilder(segmentToAdd));
                    word = RightString(word,availableCharacters);
                } while (word.Length > 0);
            }
            return lines;
        }

       
        /// <summary>
        /// Return the right part of a string in a way that compensates for Substring's deficiencies
        /// </summary>
        private static string RightString(string str,int n)
        {
            return (n >= str.Length || str.Length==0) 
                ? string.Empty 
                : str.Substring(n);
        }
        /// <summary>
        /// Return the left part of a string in a way that compensates for Substring's deficiencies
        /// </summary>
        private static string LeftString(string str,int n)
        {
            
            return  (n >= str.Length || str.Length==0)
                ? str 
                : str.Substring(0,n);
        }
    }
}
