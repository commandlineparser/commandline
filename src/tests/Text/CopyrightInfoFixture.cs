#region License
//
// Command Line Library: CopyrightInfoFixture.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2013 Giacomo Stelluti Scala
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
using System;
using System.Globalization;
using System.Text;
using NUnit.Framework;
using Should.Fluent;
#endregion

namespace CommandLine.Text.Tests
{
    [TestFixture]
    public sealed class CopyrightInfoFixture
    {
        #region Mock Objects
        private sealed class CopyleftInfo : CopyrightInfo
        {
            public CopyleftInfo(bool isSymbolUpper, string author, params int[] years)
                : base(isSymbolUpper, author, years)
            {
            }

            protected override string CopyrightWord
            {
                get { return "Copyleft"; }
            }

            protected override string FormatYears(int[] years)
            {
                var yearsPart = new StringBuilder(years.Length * 4);

                foreach (int year in years)
                {
                    string y = year.ToString(CultureInfo.InvariantCulture);
                    if (y.Length == 2)
                        yearsPart.Append(string.Concat("'", y));
                    else
                        yearsPart.Append(y);
                    yearsPart.Append(", ");
                }
                yearsPart.Remove(yearsPart.Length - 2, 2);

                return yearsPart.ToString();
            }
        }
        #endregion

        [Test]
        public void LowerSymbolOneYear()
        {
            var copyright = new CopyrightInfo(false, "Authors, Inc.", 2007);

            copyright.ToString().Should().Equal("Copyright (c) 2007 Authors, Inc.");
        }

        [Test]
        public void UpperSymbolTwoConsecutiveYears()
        {
            var copyright = new CopyrightInfo(true, "X & Y Group", 2006, 2007);

            copyright.ToString().Should().Equal("Copyright (C) 2006, 2007 X & Y Group");
        }

        [Test]
        public void DefaultSymbolTwoNonConsecutiveYears()
        {
            var copyright = new CopyrightInfo("W & Z, Inc.", 2005, 2007);

            copyright.ToString().Should().Equal("Copyright (C) 2005 - 2007 W & Z, Inc.");
        }

        [Test]
        public void DefaultSymbolSeveralYears()
        {
            var copyright = new CopyrightInfo("CommandLine, Ltd", 1999, 2003, 2004, 2007);

            copyright.ToString().Should().Equal("Copyright (C) 1999 - 2003, 2004 - 2007 CommandLine, Ltd");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WillThrowExceptionIfAuthorIsNull()
        {
            new CopyrightInfo(null, 2000);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WillThrowExceptionIfNoYearsAreSupplied()
        {
            new CopyrightInfo("Authors, Inc.");
        }

        [Test]
        public void DerivedClass()
        {
            var info = new CopyleftInfo(true, "Free Company, Inc.", 96, 97, 98, 2005);

            info.ToString().Should().Equal("Copyleft (C) '96, '97, '98, 2005 Free Company, Inc.");
        }

        #region #BUG0006
        [Test]
        public void ShouldNotGrowWhenConvertedToString()
        {
            var info = new CopyrightInfo ("ManOnTheMoon, Inc.", 2019);

            for (int i=0; i<10; i++)
            {
                info.ToString().Length.Should().Equal(37);
            }
        }
        #endregion
    }
}