#region License
// <copyright file="CopyrightInfo.cs" company="Giacomo Stelluti Scala">
//   Copyright 2015-2013 Giacomo Stelluti Scala
// </copyright>
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
#endregion
#region Using Directives
using System;
using System.Globalization;
using System.Reflection;
using System.Text;

using CommandLine.Infrastructure;
#endregion

namespace CommandLine.Text
{
    /// <summary>
    /// Models the copyright part of an help text.
    /// You can assign it where you assign any <see cref="System.String"/> instance.
    /// </summary>
    public class CopyrightInfo
    {
        private const string DefaultCopyrightWord = "Copyright";
        private const string SymbolLower = "(c)";
        private const string SymbolUpper = "(C)";
        private readonly AssemblyCopyrightAttribute _attribute;
        private readonly bool _isSymbolUpper;
        private readonly int[] _copyrightYears;
        private readonly string _author;
        private readonly int _builderSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.CopyrightInfo"/> class
        /// specifying author and year.
        /// </summary>
        /// <param name="author">The company or person holding the copyright.</param>
        /// <param name="year">The year of coverage of copyright.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="author"/> is null or empty string.</exception>
        public CopyrightInfo(string author, int year)
            : this(true, author, new[] { year })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.CopyrightInfo"/> class
        /// specifying author and copyrightYears.
        /// </summary>
        /// <param name="author">The company or person holding the copyright.</param>
        /// <param name="years">The copyrightYears of coverage of copyright.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="author"/> is null or empty string.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when parameter <paramref name="years"/> is not supplied.</exception>
        public CopyrightInfo(string author, params int[] years)
            : this(true, author, years)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.CopyrightInfo"/> class
        /// specifying symbol case, author and copyrightYears.
        /// </summary>
        /// <param name="isSymbolUpper">The case of the copyright symbol.</param>
        /// <param name="author">The company or person holding the copyright.</param>
        /// <param name="copyrightYears">The copyrightYears of coverage of copyright.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="author"/> is null or empty string.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when parameter <paramref name="copyrightYears"/> is not supplied.</exception>
        public CopyrightInfo(bool isSymbolUpper, string author, params int[] copyrightYears)
        {
            Assumes.NotNullOrEmpty(author, "author");
            Assumes.NotZeroLength(copyrightYears, "copyrightYears");

            const int ExtraLength = 10;
            _isSymbolUpper = isSymbolUpper;
            _author = author;
            _copyrightYears = copyrightYears;
            _builderSize = 12 + author.Length + (4 * copyrightYears.Length) + ExtraLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.CopyrightInfo"/> class.
        /// </summary>
        protected CopyrightInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.CopyrightInfo"/> class
        /// with an assembly attribute, this overrides all formatting.
        /// </summary>
        /// <param name="attribute">The attribute which text to use.</param>
        private CopyrightInfo(AssemblyCopyrightAttribute attribute)
        {
            _attribute = attribute;
        }

        /// <summary>
        /// Gets the default copyright information.
        /// Retrieved from <see cref="AssemblyCopyrightAttribute"/>, if it exists,
        /// otherwise it uses <see cref="AssemblyCompanyAttribute"/> as copyright holder with the current year.
        /// If neither exists it throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        public static CopyrightInfo Default
        {
            get
            {
                // if an exact copyright string has been specified, it takes precedence
                var copyright = ReflectionHelper.GetAttribute<AssemblyCopyrightAttribute>();
                if (copyright != null)
                {
                    return new CopyrightInfo(copyright);
                }

                // if no copyright attribute exist but a company attribute does, use it as copyright holder
                var company = ReflectionHelper.GetAttribute<AssemblyCompanyAttribute>();
                if (company != null)
                {
                    return new CopyrightInfo(company.Company, DateTime.Now.Year);
                }

                throw new InvalidOperationException(SR.InvalidOperationException_CopyrightInfoRequiresAssemblyCopyrightAttributeOrAssemblyCompanyAttribute);
            }
        }

        /// <summary>
        /// Gets a different copyright word when overridden in a derived class.
        /// </summary>
        protected virtual string CopyrightWord
        {
            get { return DefaultCopyrightWord; }
        }

        /// <summary>
        /// Converts the copyright instance to a <see cref="System.String"/>.
        /// </summary>
        /// <param name="info">This <see cref="CommandLine.Text.CopyrightInfo"/> instance.</param>
        /// <returns>The <see cref="System.String"/> that contains the copyright.</returns>
        public static implicit operator string(CopyrightInfo info)
        {
            return info.ToString();
        }

        /// <summary>
        /// Returns the copyright as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>The <see cref="System.String"/> that contains the copyright.</returns>
        public override string ToString()
        {
            if (_attribute != null)
            {
                return _attribute.Copyright;
            }

            var builder = new StringBuilder(_builderSize);
            builder.Append(CopyrightWord);
            builder.Append(' ');
            builder.Append(_isSymbolUpper ? SymbolUpper : SymbolLower);
            builder.Append(' ');
            builder.Append(FormatYears(_copyrightYears));
            builder.Append(' ');
            builder.Append(_author);
            return builder.ToString();
        }

        /// <summary>
        /// When overridden in a derived class, allows to specify a new algorithm to render copyright copyrightYears
        /// as a <see cref="System.String"/> instance.
        /// </summary>
        /// <param name="years">A <see cref="System.Int32"/> array of copyrightYears.</param>
        /// <returns>A <see cref="System.String"/> instance with copyright copyrightYears.</returns>
        protected virtual string FormatYears(int[] years)
        {
            if (years.Length == 1)
            {
                return years[0].ToString(CultureInfo.InvariantCulture);
            }

            var yearsPart = new StringBuilder(years.Length * 6);
            for (int i = 0; i < years.Length; i++)
            {
                yearsPart.Append(years[i].ToString(CultureInfo.InvariantCulture));
                int next = i + 1;
                if (next < years.Length)
                {
                    yearsPart.Append(years[next] - years[i] > 1 ? " - " : ", ");
                }
            }

            return yearsPart.ToString();
        }
    }
}