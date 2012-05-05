#region License
//
// Command Line Library: ReflectionUtilFixture.cs
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
#if UNIT_TESTS
#region Using Directives
using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public class ReflectionUtilFixture
    {
        #region Mock Objects
        private class MockAttribute : Attribute
        {
        }

        private class AnotherMockAttribute : Attribute
        {
        }

        private class MockWithValueAttribute : Attribute
        {
            public string StringValue = String.Empty;
        }

        private class MockObject
        {
            [Mock]
            public string StringField = String.Empty;

            [Mock]
            public bool BooleanField = false;

            [AnotherMock]
            public int IntField = 0;

            [Mock]
            public void DoNothing()
            {
            }
        }

        private class AnotherMockObject
        {
            [MockWithValue(StringValue="applied to x")]
            public long x = 0;

            [MockWithValue(StringValue="applied to y")]
            public long y = 0;

            [MockWithValue(StringValue="applied to z")]
            public long z = 0;
        }
        #endregion

        private static object _target;

        [SetUp]
        public void CreateInstance()
        {
            _target = new MockObject();
        }

        [TearDown]
        public void ShutdownInstance()
        {
            _target = null;
        }

        [Test]
        public void GetFieldsByAttribute()
        {
            var list = ReflectionUtil.RetrieveFieldList<MockAttribute>(_target);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("StringField", list[0].Left.Name);
            Assert.AreEqual("BooleanField", list[1].Left.Name);

            PrintFieldList<MockAttribute>(list);

            var anotherList = ReflectionUtil.RetrieveFieldList<AnotherMockAttribute>(_target);

            Assert.AreEqual(1, anotherList.Count);
            Assert.AreEqual("IntField", anotherList[0].Left.Name);

            PrintFieldList<AnotherMockAttribute>(anotherList);
        }

        [Test]
        public void GetMethodByAttribute()
        {
            var pair = ReflectionUtil.RetrieveMethod<MockAttribute>(_target);

            Assert.IsNotNull(pair);
            Assert.AreEqual("DoNothing", pair.Left.Name);
        }

        [Test]
        public void GetFieldsAttributeList()
        {
            var list = ReflectionUtil.RetrieveFieldAttributeList<MockWithValueAttribute>(new AnotherMockObject());

            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("applied to x", list[0].StringValue);
            Assert.AreEqual("applied to y", list[1].StringValue);
            Assert.AreEqual("applied to z", list[2].StringValue);
        }

        private static void PrintFieldList<TAttribute>(IList<Pair<FieldInfo, TAttribute>> list)
                where TAttribute : Attribute
        {
            Console.WriteLine("Attribute: {0}", list[0].Right.GetType());
            foreach (Pair<FieldInfo, TAttribute> pair in list)
            {
                Console.WriteLine("\tField: {0}", pair.Left.Name);
            }
        }
    }
}
#endif