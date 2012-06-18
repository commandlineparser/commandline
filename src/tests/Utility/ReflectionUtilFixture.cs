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
#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CommandLine.Text;
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
            public MockObject()
            {
                IntField = 0;
            }

            [Mock]
            public string StringField {get;set;}

            [Mock]
            public bool BooleanField {get;set;}

            [AnotherMock]
            public int IntField { get; set; }

            [Mock]
            public void DoNothing()
            {
            }
        }

        private class AnotherMockObject
        {
            public AnotherMockObject()
            {
                X = 0;
                Y = 0;
                Z = 0;
            }

            [MockWithValue(StringValue="applied to X")]
            public long X { get; set; }

            [MockWithValue(StringValue="applied to Y")]
            public long Y { get; set; }

            [MockWithValue(StringValue="applied to Z")]
            public long Z { get; set; }
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
            var list = ReflectionUtil.RetrievePropertyList<MockAttribute>(_target);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("StringField", list[0].Left.Name);
            Assert.AreEqual("BooleanField", list[1].Left.Name);

            PrintFieldList<MockAttribute>(list);

            var anotherList = ReflectionUtil.RetrievePropertyList<AnotherMockAttribute>(_target);

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
            var list = ReflectionUtil.RetrievePropertyAttributeList<MockWithValueAttribute>(new AnotherMockObject());

            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("applied to X", list[0].StringValue);
            Assert.AreEqual("applied to Y", list[1].StringValue);
            Assert.AreEqual("applied to Z", list[2].StringValue);
        }

        /*[Test]
        public void GetAssemblyTitleAttribute()
        {
            var attr = ReflectionUtil.GetAttribute<AssemblyTitleAttribute>();

            Assert.IsNotNull(attr);
            Assert.IsInstanceOf(typeof(AssemblyTitleAttribute), attr);
        }*/

        /*
        [Test]
        public void GetAssemblyVersionAttribute()
        {
            var attr = ReflectionUtil.GetAttribute<AssemblyVersionAttribute>();

            Assert.IsNotNull(attr);
            Assert.IsInstanceOf(typeof(AssemblyVersionAttribute), attr);
        }
        */

        /*[Test]
        public void GetAssemblyInformationalVersionAttribute()
        {
            var attr = ReflectionUtil.GetAttribute<AssemblyInformationalVersionAttribute>();

            Assert.IsNotNull(attr);
            Assert.IsInstanceOf(typeof(AssemblyInformationalVersionAttribute), attr);
        }*/

        private static void PrintFieldList<TAttribute>(IList<Pair<PropertyInfo, TAttribute>> list)
                where TAttribute : Attribute
        {
            Console.WriteLine("Attribute: {0}", list[0].Right.GetType());
            foreach (Pair<PropertyInfo, TAttribute> pair in list)
            {
                Console.WriteLine("\tField: {0}", pair.Left.Name);
            }
        }
    }
}