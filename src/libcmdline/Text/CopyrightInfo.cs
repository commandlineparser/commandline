#region License
//
// Command Line Library: CopyrightInfo.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2012 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System.Globalization;
using System.Text;
#endregion

namespace CommandLine.Text
{
    /// <summary>
    /// Models the copyright informations part of an help text.
    /// You can assign it where you assign any <see cref="System.String"/> instance.
    /// </summary>
    public class CopyrightInfo
    {
        private readonly bool _isSymbolUpper;
        private readonly int[] _years;
        private readonly string _author;
        private static readonly string _defaultCopyrightWord = "Copyright";
        private static readonly string _symbolLower = "(c)";
        private static readonly string _symbolUpper = "(C)";
        private StringBuilder _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.CopyrightInfo"/> class.
        /// </summary>
        protected CopyrightInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.CopyrightInfo"/> class
        /// specifying author and year.
        /// </summary>
        /// <param name="author">The company or person holding the copyright.</param>
        /// <param name="year">The year of coverage of copyright.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="author"/> is null or empty string.</exception>
        public CopyrightInfo(string author, int year)
            : this(true, author, new int[] { year })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.CopyrightInfo"/> class
        /// specifying author and years.
        /// </summary>
        /// <param name="author">The company or person holding the copyright.</param>
        /// <param name="years">The years of coverage of copyright.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="author"/> is null or empty string.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when parameter <paramref name="years"/> is not supplied.</exception>
        public CopyrightInfo(string author, params int[] years)
            : this(true, author, years)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.CopyrightInfo"/> class
        /// specifying symbol case, author and years.
        /// </summary>
        /// <param name="isSymbolUpper">The case of the copyright symbol.</param>
        /// <param name="author">The company or person holding the copyright.</param>
        /// <param name="years">The years of coverage of copyright.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="author"/> is null or empty string.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when parameter <paramref name="years"/> is not supplied.</exception>
        public CopyrightInfo(bool isSymbolUpper, string author, params int[] years)
        {
            Assumes.NotNullOrEmpty(author, "author");
            Assumes.NotZeroLength(years, "years");

            const int extraLength = 10;
            _isSymbolUpper = isSymbolUpper;
            _author = author;
            _years = years;
            _builder = new StringBuilder
                    (CopyrightWord.Length + author.Length + (4 * years.Length) + extraLength);
        }

        /// <summary>
        /// Returns the copyright informations as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>The <see cref="System.String"/> that contains the copyright informations.</returns>
        public override string ToString()
        {
            _builder.Append(CopyrightWord);
            _builder.Append(' ');
            if (_isSymbolUpper)
                _builder.Append(_symbolUpper);
            else
                _builder.Append(_symbolLower);

            _builder.Append(' ');
            _builder.Append(FormatYears(_years));
            _builder.Append(' ');
            _builder.Append(_author);

            return _builder.ToString();
        }

        /// <summary>
        /// Converts the copyright informations to a <see cref="System.String"/>.
        /// </summary>
        /// <param name="info">This <see cref="CommandLine.Text.CopyrightInfo"/> instance.</param>
        /// <returns>The <see cref="System.String"/> that contains the copyright informations.</returns>
        public static implicit operator string(CopyrightInfo info)
        {
            return info.ToString();
        }

        /// <summary>
        /// When overridden in a derived class, allows to specify a different copyright word.
        /// </summary>
        protected virtual string CopyrightWord
        {
            get { return _defaultCopyrightWord; }
        }

        /// <summary>
        /// When overridden in a derived class, allows to specify a new algorithm to render copyright years
        /// as a <see cref="System.String"/> instance.
        /// </summary>
        /// <param name="years">A <see cref="System.Int32"/> array of years.</param>
        /// <returns>A <see cref="System.String"/> instance with copyright years.</returns>
        protected virtual string FormatYears(int[] years)
        {
            if (years.Length == 1)
                return years[0].ToString(CultureInfo.InvariantCulture);

            var yearsPart = new StringBuilder(years.Length * 6);
            for (int i = 0; i < years.Length; i++)
            {
                yearsPart.Append(years[i].ToString(CultureInfo.InvariantCulture));
                int next = i + 1;
                if (next < years.Length)
                {
                    if (years[next] - years[i] > 1)
                        yearsPart.Append(" - ");
                    else
                        yearsPart.Append(", ");
                }
            }

            return yearsPart.ToString();
        }
    }
}