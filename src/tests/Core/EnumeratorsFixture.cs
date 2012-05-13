#region License
//
// Command Line Library: EnumeratorsFixture.cs
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
using NUnit.Framework;
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

            Assert.AreEqual(valueOne, e.Current);
            Assert.AreEqual(valueTwo, e.Next);
            Assert.IsFalse(e.IsLast);
            
            e.MoveNext();
            
            Assert.AreEqual(valueTwo, e.Current);
            Assert.AreEqual(valueThree, e.Next);
            Assert.IsFalse(e.IsLast);
            
            e.MoveNext();
            
            Assert.AreEqual(valueThree, e.Current);
            Assert.IsNull(e.Next);
            Assert.IsTrue(e.IsLast);
        }

        [Test]
        public void CharIteration()
        {
            IArgumentEnumerator e = new OneCharStringEnumerator("abcd");
            e.MoveNext();

            Assert.AreEqual("a", e.Current);
            Assert.AreEqual("b", e.Next);
            Assert.AreEqual("bcd", e.GetRemainingFromNext());
            Assert.IsFalse(e.IsLast);
            
            e.MoveNext();
            
            Assert.AreEqual("b", e.Current);
            Assert.AreEqual("c", e.Next);
            Assert.AreEqual("cd", e.GetRemainingFromNext());
            Assert.IsFalse(e.IsLast);
            
            e.MoveNext();
            
            Assert.AreEqual("c", e.Current);
            Assert.AreEqual("d", e.Next);
            Assert.AreEqual("d", e.GetRemainingFromNext());
            Assert.IsFalse(e.IsLast);
            
            e.MoveNext();
            
            Assert.AreEqual("d", e.Current);
            Assert.IsTrue(e.IsLast);
        }
    }
}