// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Text;
using System.Linq;

namespace CommandLine.Text
{
    /// <summary>
    /// Provides base properties for creating an attribute, used to define multiple lines of text.
    /// </summary>
    public abstract class MultilineTextAttribute : Attribute
    {
        private readonly string line1;
        private readonly string line2;
        private readonly string line3;
        private readonly string line4;
        private readonly string line5;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultilineTextAttribute"/> class. Used in derived type
        /// using one line of text.
        /// </summary>
        /// <param name="line1">The first line of text.</param>
        protected MultilineTextAttribute(string line1)
            : this(line1, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultilineTextAttribute"/> class. Used in  type
        /// using two lines of text.
        /// </summary>
        /// <param name="line1">The first line of text.</param>
        /// <param name="line2">The second line of text.</param>
        protected MultilineTextAttribute(string line1, string line2)
            : this(line1, line2, string.Empty, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultilineTextAttribute"/> class. Used in  type
        /// using three lines of text.
        /// </summary>
        /// <param name="line1">The first line of text.</param>
        /// <param name="line2">The second line of text.</param>
        /// <param name="line3">The third line of text.</param>
        protected MultilineTextAttribute(string line1, string line2, string line3)
            : this(line1, line2, line3, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultilineTextAttribute"/> class. Used in type
        /// using four lines of text.
        /// </summary>
        /// <param name="line1">The first line of text.</param>
        /// <param name="line2">The second line of text.</param>
        /// <param name="line3">The third line of text.</param>
        /// <param name="line4">The fourth line of text.</param>
        protected MultilineTextAttribute(string line1, string line2, string line3, string line4)
            : this(line1, line2, line3, line4, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultilineTextAttribute"/> class. Used in type
        /// using five lines of text.
        /// </summary>
        /// <param name="line1">The first line of text.</param>
        /// <param name="line2">The second line of text.</param>
        /// <param name="line3">The third line of text.</param>
        /// <param name="line4">The fourth line of text.</param>
        /// <param name="line5">The fifth line of text.</param>
        protected MultilineTextAttribute(string line1, string line2, string line3, string line4, string line5)
        {
            if (line1 == null) throw new ArgumentException("line1");
            if (line2 == null) throw new ArgumentException("line2");
            if (line3 == null) throw new ArgumentException("line3");
            if (line4 == null) throw new ArgumentException("line4");
            if (line5 == null) throw new ArgumentException("line5");
            
            this.line1 = line1;
            this.line2 = line2;
            this.line3 = line3;
            this.line4 = line4;
            this.line5 = line5;
        }

        /// <summary>
        /// Gets the all non-blank lines as string.
        /// </summary>
        /// <value>A string of all non-blank lines.</value>
        public virtual string Value
        {
            get
            {
                var value = new StringBuilder(string.Empty);
                var strArray = new[] { line1, line2, line3, line4, line5 };

                for (var i = 0; i < GetLastLineWithText(strArray); i++)
                {
                    value.AppendLine(strArray[i]);
                }

                return value.ToString();
            }
        }

        /// <summary>
        /// Gets the first line of text.
        /// </summary>
        public string Line1
        {
            get { return line1; }
        }

        /// <summary>
        /// Gets the second line of text.
        /// </summary>
        public string Line2
        {
            get { return line2; }
        }

        /// <summary>
        /// Gets third line of text.
        /// </summary>
        public string Line3
        {
            get { return line3; }
        }

        /// <summary>
        /// Gets the fourth line of text.
        /// </summary>
        public string Line4
        {
            get { return line4; }
        }

        /// <summary>
        /// Gets the fifth line of text.
        /// </summary>
        public string Line5
        {
            get { return line5; }
        }

        internal HelpText AddToHelpText(HelpText helpText, Func<string, HelpText> func)
        {
            var strArray = new[] { line1, line2, line3, line4, line5 };
            return strArray.Take(GetLastLineWithText(strArray)).Aggregate(helpText, (current, line) => func(line));
        }

        internal HelpText AddToHelpText(HelpText helpText, bool before)
        {
            // before flag only distinguishes which action is called, 
            // so refactor common code and call with appropriate func
            return before
                ? AddToHelpText(helpText, helpText.AddPreOptionsLine)
                : AddToHelpText(helpText, helpText.AddPostOptionsLine);
        }

        /// <summary>
        /// Returns the last line with text. Preserves blank lines if user intended by skipping a line.
        /// </summary>
        /// <returns>The last index of line of the non-blank line.
        /// </returns>
        /// <param name='value'>The string array to process.</param>
        protected virtual int GetLastLineWithText(string[] value)
        {
            var index = Array.FindLastIndex(value, str => !string.IsNullOrEmpty(str));

            // remember FindLastIndex returns zero-based index
            return index + 1;
        }
    }
}