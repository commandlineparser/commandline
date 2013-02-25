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
using Xunit;
using FluentAssertions;
using CommandLine.Text;
#endregion

namespace CommandLine.Tests.Unit.Text
{
    public class CopyrightInfoFixture
    {
        #region Mock Objects
        private sealed class CopyleftInfo : CopyrightInfo
        {
            public CopyleftInfo(bool isSymbolUpper, string author, params int[] copyrightYears)
                : base(isSymbolUpper, author, copyrightYears)
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

        [Fact]
        public void Lower_symbol_one_year()
        {
            var copyright = new CopyrightInfo(false, "Authors, Inc.", 2007);

            copyright.ToString().Should().Be("Copyright (c) 2007 Authors, Inc.");
        }

        [Fact]
        public void Upper_symbol_two_consecutive_years()
        {
            var copyright = new CopyrightInfo(true, "X & Y Group", 2006, 2007);

            copyright.ToString().Should().Be("Copyright (C) 2006, 2007 X & Y Group");
        }

        [Fact]
        public void Default_symbol_two_non_consecutive_years()
        {
            var copyright = new CopyrightInfo("W & Z, Inc.", 2005, 2007);

            copyright.ToString().Should().Be("Copyright (C) 2005 - 2007 W & Z, Inc.");
        }

        [Fact]
        public void Default_symbol_several_years()
        {
            var copyright = new CopyrightInfo("CommandLine, Ltd", 1999, 2003, 2004, 2007);

            copyright.ToString().Should().Be("Copyright (C) 1999 - 2003, 2004 - 2007 CommandLine, Ltd");
        }

        [Fact]
        public void Will_throw_exception_if_author_is_null()
        {
            Assert.Throws<ArgumentException>(
                () => new CopyrightInfo(null, 2000));
        }

        [Fact]
        public void Will_throw_exception_if_no_years_are_supplied()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new CopyrightInfo("Authors, Inc."));
        }

        [Fact]
        public void Derived_class()
        {
            var info = new CopyleftInfo(true, "Free Company, Inc.", 96, 97, 98, 2005);

            info.ToString().Should().Be("Copyleft (C) '96, '97, '98, 2005 Free Company, Inc.");
        }

        #region #BUG0006
        [Fact]
        public void Should_not_grow_when_converted_to_string()
        {
            var info = new CopyrightInfo ("ManOnTheMoon, Inc.", 2019);

            for (int i=0; i<10; i++)
            {
                info.ToString().Length.Should().Be(37);
            }
        }
        #endregion
    }
}

