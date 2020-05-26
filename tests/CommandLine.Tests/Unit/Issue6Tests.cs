using System;
using System.Linq;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

//Issue #6
//Support Aliases on verbs (i.e. "move" and "mv" are the same verb).

namespace CommandLine.Tests.Unit
{
    public class Issue6Tests
    {
        [Theory]
        [InlineData("move -a bob", typeof(AliasedVerbOption1))]
        [InlineData("mv -a bob", typeof(AliasedVerbOption1))]
        [InlineData("copy -a bob", typeof(AliasedVerbOption2))]
        [InlineData("cp -a bob", typeof(AliasedVerbOption2))]
        public void Parse_option_with_aliased_verbs(string args, Type expectedArgType)
        {
            var arguments = args.Split(' ');
            object options = null;
            var result = Parser.Default.ParseArguments<AliasedVerbOption1, AliasedVerbOption2>(arguments)
                .WithParsed((o) => options = o)
               ;

            Assert.NotNull(options);
            Assert.Equal(expectedArgType, options.GetType());
        }

        [Verb("move", aliases:new string[] { "mv" })]
        public class AliasedVerbOption1
        {
            [Option('a', "alpha", Required = true)]
            public string Option { get; set; }
        }

        [Verb("copy", aliases: new string[] { "cp" })]
        public class AliasedVerbOption2
        {
            [Option('a', "alpha", Required = true)]
            public string Option { get; set; }
        }
    }
}
