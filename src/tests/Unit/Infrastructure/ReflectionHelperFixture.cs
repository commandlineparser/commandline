#region License
//
// Command Line Library: ReflectionUtilFixture.cs
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
using System.Collections.Generic;
using System.Reflection;
using CommandLine.Infrastructure;
using Xunit;
using FluentAssertions;
#endregion

namespace CommandLine.Tests.Unit.Infrastructure
{
    public class ReflectionHelperFixture
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

        [Fact]
        public void Get_fields_by_attribute()
        {
            var target = new MockObject();
            var list = ReflectionHelper.RetrievePropertyList<MockAttribute>(target);

            list.Should().HaveCount(n => n == 2);
            list[0].Left.Name.Should().Be("StringField");
            list[1].Left.Name.Should().Be("BooleanField");

            PrintFieldList<MockAttribute>(list);

            var anotherList = ReflectionHelper.RetrievePropertyList<AnotherMockAttribute>(target);

            anotherList.Should().HaveCount(n => n == 1);
            anotherList[0].Left.Name.Should().Be("IntField");

            PrintFieldList<AnotherMockAttribute>(anotherList);
        }

        [Fact]
        public void Get_method_by_attribute()
        {
            var target = new MockObject();
            var pair = ReflectionHelper.RetrieveMethod<MockAttribute>(target);

            pair.Should().NotBeNull();
            pair.Left.Name.Should().Be("DoNothing");
        }

        [Fact]
        public void Get_fields_attribute_list()
        {
            var list = ReflectionHelper.RetrievePropertyAttributeList<MockWithValueAttribute>(new AnotherMockObject());

            list.Should().NotBeNull();
            list.Should().HaveCount(n => n == 3);
            list[0].StringValue.Should().Be("applied to X");
            list[1].StringValue.Should().Be("applied to Y");
            list[2].StringValue.Should().Be("applied to Z");
        }

        private static void PrintFieldList<TAttribute>(IList<Pair<PropertyInfo, TAttribute>> list)
                where TAttribute : Attribute
        {
            Console.WriteLine("Attribute: {0}", list[0].Right.GetType());
            foreach (var pair in list)
            {
                Console.WriteLine("\tField: {0}", pair.Left.Name);
            }
        }
    }
}

