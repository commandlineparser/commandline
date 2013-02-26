#region License
//
// Command Line Library: EnumeratorsFixture.cs
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

using CommandLine.Parsing;

using Xunit;
using FluentAssertions;
using CommandLine.Infrastructure;
#endregion

namespace CommandLine.Tests.Unit.Infrastructure
{
    public class EnumeratorsFixture
    {
        [Fact]
        public void String_iteration()
        {
            const string valueOne = "one";
            const string valueTwo = "two";
            const string valueThree = "three";

            string[] values = { valueOne, valueTwo, valueThree };
            IArgumentEnumerator e = new StringArrayEnumerator(values);
            e.MoveNext();

            e.Current.Should().Be(valueOne);
            e.Next.Should().Be(valueTwo);
            e.IsLast.Should().BeFalse();
            
            e.MoveNext();
            
            e.Current.Should().Be(valueTwo);
            e.Next.Should().Be(valueThree);
            e.IsLast.Should().BeFalse();
            
            e.MoveNext();
            
            e.Current.Should().Be(valueThree);
            e.Next.Should().BeNull();
            e.IsLast.Should().BeTrue();
        }

        [Fact]
        public void Char_iteration()
        {
            IArgumentEnumerator e = new OneCharStringEnumerator("abcd");
            e.MoveNext();

            e.Current.Should().Be("a");
            e.Next.Should().Be("b");
            e.GetRemainingFromNext().Should().Be("bcd");
            e.IsLast.Should().BeFalse();
            
            e.MoveNext();
            
            e.Current.Should().Be("b");
            e.Next.Should().Be("c");
            e.GetRemainingFromNext().Should().Be("cd");
            e.IsLast.Should().BeFalse();
            
            e.MoveNext();
            
            e.Current.Should().Be("c");
            e.Next.Should().Be("d");
            e.GetRemainingFromNext().Should().Be("d");
            e.IsLast.Should().BeFalse();
            
            e.MoveNext();
            
            e.Current.Should().Be("d");
            e.IsLast.Should().BeTrue();
        }
    }
}
