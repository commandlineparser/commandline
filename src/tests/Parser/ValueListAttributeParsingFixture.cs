#region License
//
// Command Line Library: ValueListParsingFixture.cs
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
using NUnit.Framework;
using Should.Fluent;
using CommandLine.Tests.Mocks;
#endregion

namespace CommandLine.Tests
{
    public sealed class ValueListAttributeParsingFixture : CommandLineParserBaseFixture
    {
        public ValueListAttributeParsingFixture() : base()
        {
        }

        [Test]
        public void ValueListAttributeIsolatesNonOptionValues()
        {
            var options = new SimpleOptionsWithValueList();
            Result = base.Parser.ParseArguments(
                new string[] { "--switch", "file1.ext", "file2.ext", "file3.ext", "-s", "out.ext" }, options);

            ResultShouldBeTrue();
            options.Items[0].Should().Equal("file1.ext");
            options.Items[1].Should().Equal("file2.ext");
            options.Items[2].Should().Equal("file3.ext");
            options.StringValue.Should().Equal("out.ext");
            options.BooleanValue.Should().Be.True();
            Console.WriteLine(options);
        }

        [Test]
        public void ValueListWithMaxElemInsideBounds()
        {
            var options = new OptionsWithValueListMaximumThree();
            Result = base.Parser.ParseArguments(new string[] { "file.a", "file.b", "file.c" }, options);

            ResultShouldBeTrue();
            options.InputFilenames[0].Should().Equal("file.a");
            options.InputFilenames[1].Should().Equal("file.b");
            options.InputFilenames[2].Should().Equal("file.c");
            options.OutputFile.Should().Be.Null();
            options.Overwrite.Should().Be.False();
            Console.WriteLine(options);
        }

        [Test]
        public void ValueListWithMaxElemOutsideBounds()
        {
            var options = new OptionsWithValueListMaximumThree();
            Result = base.Parser.ParseArguments(
                    new string[] { "file.a", "file.b", "file.c", "file.d" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void ValueListWithMaxElemSetToZeroSucceeds()
        {
            var options = new OptionsWithValueListMaximumZero();
            Result = base.Parser.ParseArguments(new string[] { }, options);

            ResultShouldBeTrue();
            options.Junk.Should().Count.Zero();
            Console.WriteLine(options);
        }

        [Test]
        public void ValueListWithMaxElemSetToZeroFailes()
        {
            var options = new OptionsWithValueListMaximumZero();

            Result = base.Parser.ParseArguments(new string[] { "some", "value" }, options);
            ResultShouldBeFalse();
        }
    }
}