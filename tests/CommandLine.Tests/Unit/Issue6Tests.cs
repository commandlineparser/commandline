using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Microsoft.FSharp.Core;
using Xunit;
using Xunit.Abstractions;

//Issue #6
//Support Aliases on verbs (i.e. "move" and "mv" are the same verb).

namespace CommandLine.Tests.Unit
{
    public class Issue6Tests
    {
        /// <summary>
        /// Test Verb aliases when one verb is set as a default
        /// </summary>
        /// <param name="args"></param>
        /// <param name="expectedArgType"></param>
        [Theory]
        [InlineData("move -a bob", typeof(AliasedVerbOption1))]
        [InlineData("mv -a bob", typeof(AliasedVerbOption1))]
        [InlineData("copy -a bob", typeof(AliasedVerbOption2))]
        [InlineData("cp -a bob", typeof(AliasedVerbOption2))]
        [InlineData("-a bob", typeof(AliasedVerbOption2))]
        public void Parse_option_with_aliased_verbs(string args, Type expectedArgType)
        {
            var arguments = args.Split(' ');
            object options = null;
            IEnumerable<Error> errors = null;
            var result = Parser.Default.ParseArguments<AliasedVerbOption1, AliasedVerbOption2>(arguments)
                .WithParsed(o => options = o)
                .WithNotParsed(o => errors = o)
               ;
            if (errors != null && errors.Any())
            {
                foreach (Error e in errors)
                {
                    System.Console.WriteLine(e.ToString());
                }
            }

            Assert.NotNull(options);
            Assert.Equal(expectedArgType, options.GetType());
        }

        /// <summary>
        /// Test verb aliases with no default verb and 1 verb with no aliases
        /// </summary>
        /// <param name="args"></param>
        /// <param name="expectedArgType"></param>
        [Theory]
        [InlineData("move -a bob", typeof(AliasedVerbOption1))]
        [InlineData("mv -a bob", typeof(AliasedVerbOption1))]
        [InlineData("delete -b fred", typeof(VerbNoAlias))]
        public void Parse_option_with_aliased_verb(string args, Type expectedArgType)
        {
            var arguments = args.Split(' ');
            object options = null;
            IEnumerable<Error> errors = null;
            var result = Parser.Default.ParseArguments<AliasedVerbOption1, VerbNoAlias>(arguments)
                .WithParsed(o => options = o)
                .WithNotParsed(o => errors = o)
               ;
            if (errors != null && errors.Any())
            {
                foreach (Error e in errors)
                {
                    System.Console.WriteLine(e.ToString());
                }
            }

            Assert.NotNull(options);
            Assert.Equal(expectedArgType, options.GetType());
        }

        /// <summary>
        /// Verify auto-help generation.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="verbsIndex"></param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData("--help", true, new string[]
            {
                "copy, cp, cpy    (Default Verb) Copy some stuff",
                "move, mv",
                "delete           Delete stuff",
                "help             Display more information on a specific command.",
                "version          Display version information.",
            })]
        [InlineData("help", true, new string[]
            {
                "copy, cp, cpy    (Default Verb) Copy some stuff",
                "move, mv",
                "delete           Delete stuff",
                "help             Display more information on a specific command.",
                "version          Display version information.",
            })]
        [InlineData("move --help", false, new string[]
            {
                "-a, --alpha    Required.",
                "--help         Display this help screen.",
                "--version      Display version information.",
            })]
        [InlineData("mv --help", false, new string[]
            {
                "-a, --alpha    Required.",
                "--help         Display this help screen.",
                "--version      Display version information.",
            })]
        [InlineData("delete --help", false, new string[]
            {
                "-b, --beta    Required.",
                "--help        Display this help screen.",
                "--version     Display version information.",
            })]
        public void Parse_help_option_for_aliased_verbs(string args, bool verbsIndex, string[] expected)
        {
            var arguments = args.Split(' ');
            object options = null;
            IEnumerable<Error> errors = null;
            // the order of the arguments here drives the order of the commands shown
            // in the help message
            var result = Parser.Default.ParseArguments<
                                            AliasedVerbOption2,
                                            AliasedVerbOption1,
                                            VerbNoAlias
                                        >(arguments)
                .WithParsed(o => options = o)
                .WithNotParsed(o => errors = o)
               ;

            var message = HelpText.AutoBuild(result,
                error => error,
                ex => ex,
                verbsIndex: verbsIndex
            );

            string helpMessage = message.ToString();
            var helps = helpMessage.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(2).ToList<string>();

            expected.Length.Should().Be(helps.Count);
            int i = 0;
            foreach (var expect in expected)
            {
                helps[i].Trim().Should().Be(expect);
                i++;
            }
        }

        /// <summary>
        /// Verify auto-help generation with no default verb.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="verbsIndex"></param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData("--help", true, new string[]
            {
                "move, mv",
                "delete      Delete stuff",
                "help        Display more information on a specific command.",
                "version     Display version information.",
            })]
        [InlineData("help", true, new string[]
            {
                "move, mv",
                "delete      Delete stuff",
                "help        Display more information on a specific command.",
                "version     Display version information.",
            })]
        [InlineData("move --help", false, new string[]
            {
                "-a, --alpha    Required.",
                "--help         Display this help screen.",
                "--version      Display version information.",
            })]
        [InlineData("mv --help", false, new string[]
            {
                "-a, --alpha    Required.",
                "--help         Display this help screen.",
                "--version      Display version information.",
            })]
        [InlineData("delete --help", false, new string[]
            {
                "-b, --beta    Required.",
                "--help        Display this help screen.",
                "--version     Display version information.",
            })]
        public void Parse_help_option_for_aliased_verbs_no_default(string args, bool verbsIndex, string[] expected)
        {
            var arguments = args.Split(' ');
            object options = null;
            IEnumerable<Error> errors = null;
            // the order of the arguments here drives the order of the commands shown
            // in the help message
            var result = Parser.Default.ParseArguments<
                                            AliasedVerbOption1,
                                            VerbNoAlias
                                        >(arguments)
                .WithParsed(o => options = o)
                .WithNotParsed(o => errors = o)
               ;

            var message = HelpText.AutoBuild(result,
                error => error,
                ex => ex,
                verbsIndex: verbsIndex
            );

            string helpMessage = message.ToString();
            var helps = helpMessage.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(2).ToList<string>();

            expected.Length.Should().Be(helps.Count);
            int i = 0;
            foreach (var expect in expected)
            {
                helps[i].Trim().Should().Be(expect);
                i++;
            }
        }

        [Verb("move",
            aliases: new string[] { "mv" }
        )]
        public class AliasedVerbOption1
        {
            [Option('a', "alpha", Required = true)]
            public string Option { get; set; }
        }

        [Verb("copy",
            isDefault: true,
            aliases: new string[] { "cp", "cpy" },
            HelpText = "Copy some stuff"
        )]
        public class AliasedVerbOption2
        {
            [Option('a', "alpha", Required = true)]
            public string Option { get; set; }
        }

        [Verb("delete", HelpText = "Delete stuff")]
        public class VerbNoAlias
        {
            [Option('b', "beta", Required = true)]
            public string Option { get; set; }
        }
    }
}
