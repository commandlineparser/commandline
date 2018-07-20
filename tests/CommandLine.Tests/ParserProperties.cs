// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using CommandLine.Tests.Properties.Fakes;
using FluentAssertions;
using FsCheck;
using Xunit;

namespace CommandLine.Tests.Properties
{
    public class ParserProperties
    {
        private static readonly Parser Sut = new Parser();

        //[Fact]
        public void Parsing_a_string_returns_original_string()
        {
            Prop.ForAll<NonNull<string>>(
                x =>
                {
                    var value = x.Get;
                    var result = Sut.ParseArguments<Scalar_String_Mutable>(new[] { "--stringvalue", value });
                    ((Parsed<Scalar_String_Mutable>)result).Value.StringValue.Should().BeEquivalentTo(value);
                }).QuickCheckThrowOnFailure();
        }
    }
}
