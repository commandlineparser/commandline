#region License
//
// Command Line Library: AttributesFixture.cs
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
using Xunit;
#endregion

namespace CommandLine.Tests.Unit.Attributes
{
    public class AttributesFixture
    {
        class CustomOptionAttribute : BaseOptionAttribute
        {
            public CustomOptionAttribute(string longName)
                : base(null, longName)
            {
            }

            public CustomOptionAttribute(char shortName, string longName)
                : base(shortName, longName)
            {
            }
        }

        #region #DGN0002
        // Impossible now! (see API change 00)
        //[Fact]
        //[ExpectedException(typeof(ArgumentException))]
        //public void OptionShortNameCanNotExceedOneCharacter()
        //{
        //    new OptionAttribute("more-than-one-character", null);
        //}
        //[Fact]
        //[ExpectedException(typeof(ArgumentException))]
        //public void OptionListShortNameCanNotExceedOneCharacter()
        //{
        //    new OptionListAttribute("same-as-above", null);
        //}
        //[Fact]
        //[ExpectedException(typeof(ArgumentException))]
        //public void HelpOptionShortNameCanNotExceedOneCharacter()
        //{
        //    new HelpOptionAttribute("same-as-above-again", null);
        //}
        //[Fact]
        //[ExpectedException(typeof(ArgumentException))]
        //public void ShortNameOfBaseOptionDerivedTypeCanNotExceedOneCharacter()
        //{
        //    new CustomOptionAttribute("not-allowed", null);
        //}
        #endregion

        #region API change 01
        [Fact]
        public void Short_name_with_line_terminator_throws_exception()
        {
            Assert.Throws<ArgumentException>(() =>
                new OptionAttribute('\n'));
        }

        [Fact]
        public void Short_name_with_line_terminator_throws_exception_2()
        {
            Assert.Throws<ArgumentException>(() =>
                new OptionAttribute('\r'));
        }

        [Fact]
        public void Short_name_with_white_space_throws_exception()
        {
            Assert.Throws<ArgumentException>(() =>
                new OptionAttribute(' '));
        }

        [Fact]
        public void Short_name_with_white_space_throws_exception_2()
        {
            Assert.Throws<ArgumentException>(() =>
                new OptionAttribute('\t'));
        }
        #endregion

        [Fact]
        public void All_options_allow_one_character_in_short_name()
        {
            new OptionAttribute('o', null);
            new OptionListAttribute('l', null);
            new HelpOptionAttribute('?', null);
            new CustomOptionAttribute('c', null);
        }

        [Fact]
        public void All_options_allow_null_value_in_short_name()
        {
            new OptionAttribute("option-attr");
            new OptionListAttribute("option-list-attr");
            new HelpOptionAttribute("help-attr");
            new CustomOptionAttribute("custom-attr");
        }
    }
}
