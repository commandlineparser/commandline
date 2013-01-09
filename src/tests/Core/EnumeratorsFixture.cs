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
using NUnit.Framework;
using Should.Fluent;
using CommandLine.Internal;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public sealed class EnumeratorsFixture
    {
        [Test]
        public void StringIteration()
        {
            const string valueOne = "one";
            const string valueTwo = "two";
            const string valueThree = "three";

            string[] values = { valueOne, valueTwo, valueThree };
            IArgumentEnumerator e = new StringArrayEnumerator(values);
            e.MoveNext();

            e.Current.Should().Equal(valueOne);
            e.Next.Should().Equal(valueTwo);
            e.IsLast.Should().Be.False();
            
            e.MoveNext();
            
            e.Current.Should().Equal(valueTwo);
            e.Next.Should().Equal(valueThree);
            e.IsLast.Should().Be.False();
            
            e.MoveNext();
            
            e.Current.Should().Equal(valueThree);
            e.Next.Should().Be.Null();
            e.IsLast.Should().Be.True();
        }

        [Test]
        public void CharIteration()
        {
            IArgumentEnumerator e = new OneCharStringEnumerator("abcd");
            e.MoveNext();

            e.Current.Should().Equal("a");
            e.Next.Should().Equal("b");
            e.GetRemainingFromNext().Should().Equal("bcd");
            e.IsLast.Should().Be.False();
            
            e.MoveNext();
            
            e.Current.Should().Equal("b");
            e.Next.Should().Equal("c");
            e.GetRemainingFromNext().Should().Equal("cd");
            e.IsLast.Should().Be.False();
            
            e.MoveNext();
            
            e.Current.Should().Equal("c");
            e.Next.Should().Equal("d");
            e.GetRemainingFromNext().Should().Equal("d");
            e.IsLast.Should().Be.False();
            
            e.MoveNext();
            
            e.Current.Should().Equal("d");
            e.IsLast.Should().Be.True();
        }
    }
}