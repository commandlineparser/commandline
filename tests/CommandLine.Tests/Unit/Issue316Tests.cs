using Xunit;
using CommandLine.Tests.Fakes;
using System.Collections.Generic;
using System;

namespace CommandLine.Tests.Unit
{
    //issue#316, Set to true nullable bool arguments without the value
    public class Issue316Tests
    {
        [Theory]
        [InlineData("--bool --nullable-bool", true, true)]
        [InlineData("--nullable-bool --bool", true, true)]
        [InlineData("--nullable-bool true --bool", true, true)]
        [InlineData("--bool --nullable-bool true", true, true)]
        [InlineData("--nullable-bool false --bool", true, false)]
        [InlineData("--bool --nullable-bool false", true, false)]
        [InlineData("--bool", true, null)]
        [InlineData("--nullable-bool", false, true)]
        [InlineData("--nullable-bool true", false, true)]
        [InlineData("--nullable-bool false", false, false)]
        [InlineData("", false, null)]
        public void Check_if_nullable_boolean_works_correctly(string args, bool expectedBoolean, bool? expectedNullable)
        {
            string[] arguments = args.Split(' ');

            Options_With_Nullable_Bool options = null;
            IEnumerable<Error> errors = null;
            Parser.Default.ParseArguments<Options_With_Nullable_Bool>(arguments)
                .WithParsed(o => options = o)
                .WithNotParsed(o => errors = o);

            if (errors != null)
                foreach (Error e in errors)
                    Console.WriteLine(e);

            Assert.NotNull(options);
            Assert.Equal(options.Bool, expectedBoolean);
            Assert.Equal(options.NullableBool, expectedNullable);
        }
    }
}
