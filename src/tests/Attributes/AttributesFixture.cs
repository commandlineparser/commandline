#region License
//
// Command Line Library: AttributesFixture.cs
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
using System;
using NUnit.Framework;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public sealed class AttributesFixture
    {
        class CustomOptionAttribute : BaseOptionAttribute
        {
            public CustomOptionAttribute(string shortName, string longName)
            {
                base.ShortName = shortName;
                base.LongName = longName;
            }
        }

        #region #DGN0002
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void OptionShortNameCanNotExceedOneCharacter()
        {
            new OptionAttribute("more-than-one-character", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void OptionListShortNameCanNotExceedOneCharacter()
        {
            new OptionListAttribute("same-as-above", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void HelpOptionShortNameCanNotExceedOneCharacter()
        {
            new HelpOptionAttribute("same-as-above-again", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShortNameOfBaseOptionDerivedTypeCanNotExceedOneCharacter()
        {
            new CustomOptionAttribute("not-allowed", null);
        }
        #endregion

        [Test]
        public void AllOptionsAllowOneCharacterInShortName()
        {
            new OptionAttribute("o", null);
            new OptionListAttribute("l", null);
            new HelpOptionAttribute("?", null);
            new CustomOptionAttribute("c", null);
        }

        [Test]
        public void AllOptionsAllowNullValueInShortName()
        {
            new OptionAttribute(null, "option-attr");
            new OptionListAttribute(null, "option-list-attr");
            new HelpOptionAttribute(null, "help-attr");
            new CustomOptionAttribute(null, "custom-attr");
        }
    }
}