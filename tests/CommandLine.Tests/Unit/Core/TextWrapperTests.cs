using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using CSharpx;
using CommandLine.Text;

namespace CommandLine.Tests.Unit.Core
{
    public class TextWrapperTests
    {
        private string NormalizeLineBreaks(string str)
        {
            return str.Replace("\r", "");
        }

        private void EnsureEquivalent(string a, string b)
        {
            //workaround build system line-end inconsistencies
            NormalizeLineBreaks(a).Should().Be(NormalizeLineBreaks(b));
        }


        [Fact]
        public void ExtraSpacesAreTreatedAsNonBreaking()
        {
            var input =
                "here is some text                                                          with some extra spacing";
            var expected = @"here is some text
with some extra
spacing";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(20).ToText(), expected);
        }


        [Fact]
        public void IndentWorksCorrectly()
        {
            var input =
                @"line1
line2";
            var expected = @"  line1
  line2";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.Indent(2).ToText(), expected);
        }

        [Fact]
        public void LongWordsAreBroken()
        {
            var input =
                "here is some text that contains a veryLongWordThatWontFitOnASingleLine";
            var expected = @"here is some text
that contains a
veryLongWordThatWont
FitOnASingleLine";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(20).ToText(), expected);
        }

        [Fact]
        public void NegativeColumnWidthStillProducesOutput()
        {
            var input = @"test";
            var expected = string.Join(Environment.NewLine, input.Select(c => c.ToString()));
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(-1).ToText(), expected);
        }

        [Fact]
        public void SimpleWrappingIsAsExpected()
        {
            var input =
                @"here is some text that needs wrapping";
            var expected = @"here is
some text
that needs
wrapping";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(10).ToText(), expected);
        }

        [Fact]
        public void SingleColumnStillProducesOutputForSubIndentation()
        {
            var input = @"test
    ind";

            var expected = @"t
e
s
t
i
n
d";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(-1).ToText(), expected);
        }

        [Fact]
        public void SpacesWithinStringAreRespected()
        {
            var input =
                "here     is some text with some extra spacing";
            var expected = @"here     is some
text with some extra
spacing";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(20).ToText(), expected);
        }

        [Fact]
        public void SubIndentationCorrectlyWrapsWhenColumnWidthRequiresIt()
        {
            var input = @"test
    indented";
            var expected = @"test
    in
    de
    nt
    ed";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(6).ToText(), expected);
        }

        [Fact]
        public void SubIndentationIsPreservedWhenBreakingWords()
        {
            var input =
                "here is some text that contains \n  a veryLongWordThatWontFitOnASingleLine";
            var expected = @"here is some text
that contains
  a
  veryLongWordThatWo
  ntFitOnASingleLine";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(20).ToText(), expected);
        }

        [Fact]
        public void WrappingAvoidsBreakingWords()
        {
            var input =
                @"here hippopotamus is some text that needs wrapping";
            var expected = @"here
hippopotamus is
some text that
needs wrapping";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(15).ToText(), expected);
        }


        [Fact]
        public void WrappingExtraSpacesObeySubIndent()
        {
            var input =
                "here is some\n   text                                                          with some extra spacing";
            var expected = @"here is some
   text
   with some extra
   spacing";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(20).ToText(), expected);
        }

        [Fact]
        public void WrappingObeysLineBreaksOfAllStyles()
        {
            var input =
                "here is some text\nthat needs\r\nwrapping";
            var expected = @"here is some text
that needs
wrapping";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(20).ToText(), expected);
        }


        [Fact]
        public void WrappingPreservesSubIndentation()
        {
            var input =
                "here is some text\n   that needs wrapping where we want the wrapped part to preserve indentation\nand this part to not be indented";
            var expected = @"here is some text
   that needs
   wrapping where we
   want the wrapped
   part to preserve
   indentation
and this part to not
be indented";
            var wrapper = new TextWrapper(input);
            EnsureEquivalent(wrapper.WordWrap(20).ToText(), expected);
        }
    }
}
