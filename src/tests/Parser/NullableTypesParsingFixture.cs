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
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Mocks;
#endregion

namespace CommandLine.Tests
{
    public class NullableTypesParsingFixture : CommandLineParserBaseFixture
    {
        public NullableTypesParsingFixture() : base()
        {
        }

        [Fact]
        public void ParseNullableIntegerOption()
        {
            var options = new NullableTypesOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-i", "99" }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(99);

            options = new NullableTypesOptions();
            parser = new CommandLineParser();
            result = parser.ParseArguments(new string[] { }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().NotHaveValue();
        }

        [Fact]
        public void PassingBadValueToANullableIntegerOptionFails()
        {
            var options = new NullableTypesOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-i", "string-value" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingNoValueToANullableIntegerOptionFails()
        {
            var options = new NullableTypesOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-int" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void ParseNullableEnumerationOption()
        {
            var options = new NullableTypesOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--enum=ReadWrite" }, options);

            result.Should().BeTrue();
            options.EnumValue.Should().Be(FileAccess.ReadWrite);

            options = new NullableTypesOptions();
            parser = new CommandLineParser();
            result = parser.ParseArguments(new string[] { }, options);

            result.Should().BeTrue();
            options.EnumValue.Should().BeNull();
        }

        [Fact]
        public void PassingBadValueToANullableEnumerationOptionFails()
        {
            var options = new NullableTypesOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-e", "Overwrite" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingNoValueToANullableEnumerationOptionFails()
        {
            var options = new NullableTypesOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--enum" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void ParseNullableDoubleOption()
        {
            var options = new NullableTypesOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-d9.999" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(9.999);

            options = new NullableTypesOptions();
            parser = new CommandLineParser();
            result = parser.ParseArguments(new string[] { }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().NotHaveValue();
        }

        [Fact]
        public void PassingBadValueToANullableDoubleOptionFails()
        {
            var options = new NullableTypesOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--double", "9,999" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingNoValueToANullableDoubleOptionFails()
        {
            var options = new NullableTypesOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-d" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void ParseStringOptionAndNullableValueTypes()
        {
            var options = new NullableTypesOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--string", "alone" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("alone");

            options = new NullableTypesOptions();
            parser = new CommandLineParser();
            result = parser.ParseArguments(
                new string[] { "-d1.789", "--int", "10099", "-stogether", "--enum", "Read" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(1.789D);
            options.IntegerValue.Should().Be(10099);
            options.StringValue.Should().Be("together");
            options.EnumValue.Should().Be(FileAccess.Read);
        }

    }
}