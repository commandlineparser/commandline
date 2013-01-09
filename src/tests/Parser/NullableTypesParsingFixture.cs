#region License
//
// Command Line Library: NullableTypesParsingFixture.cs
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
using System.IO;
using NUnit.Framework;
using Should.Fluent;
using CommandLine.Tests.Mocks;
#endregion

namespace CommandLine.Tests
{
    public sealed class NullableTypesParsingFixture : CommandLineParserBaseFixture
    {
        public NullableTypesParsingFixture() : base()
        {
        }

        [Test]
        public void ParseNullableIntegerOption()
        {
            var options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { "-i", "99" }, options);

            ResultShouldBeTrue();
            options.IntegerValue.Should().Equal(99);

            options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { }, options);

            ResultShouldBeTrue();
            options.IntegerValue.Should().Be.Null();
        }

        [Test]
        public void PassingBadValueToANullableIntegerOptionFails()
        {
            var options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { "-i", "string-value" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingNoValueToANullableIntegerOptionFails()
        {
            var options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { "-int" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void ParseNullableEnumerationOption()
        {
            var options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { "--enum=ReadWrite" }, options);

            ResultShouldBeTrue();
            options.EnumValue.Should().Equal(FileAccess.ReadWrite);

            options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { }, options);

            ResultShouldBeTrue();
            options.EnumValue.Should().Be.Null();
        }

        [Test]
        public void PassingBadValueToANullableEnumerationOptionFails()
        {
            var options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { "-e", "Overwrite" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingNoValueToANullableEnumerationOptionFails()
        {
            var options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { "--enum" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void ParseNullableDoubleOption()
        {
            var options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { "-d9.999" }, options);

            ResultShouldBeTrue();
            options.DoubleValue.Should().Equal(9.999);

            options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { }, options);

            ResultShouldBeTrue();
            options.DoubleValue.Should().Be.Null();
        }

        [Test]
        public void PassingBadValueToANullableDoubleOptionFails()
        {
            var options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { "--double", "9,999" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingNoValueToANullableDoubleOptionFails()
        {
            var options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { "-d" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void ParseStringOptionAndNullableValueTypes()
        {
            var options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(new string[] { "--string", "alone" }, options);

            ResultShouldBeTrue();
            options.StringValue.Should().Equal("alone");

            options = new NullableTypesOptions();
            Result = base.Parser.ParseArguments(
                new string[] { "-d1.789", "--int", "10099", "-stogether", "--enum", "Read" }, options);

            ResultShouldBeTrue();
            options.DoubleValue.Should().Equal(1.789D);
            options.IntegerValue.Should().Equal(10099);
            options.StringValue.Should().Equal("together");
            options.EnumValue.Should().Equal(FileAccess.Read);
        }

    }
}