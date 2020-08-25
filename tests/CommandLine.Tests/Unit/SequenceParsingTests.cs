using System.Collections.Generic;
using System.Linq;
using System;
using Xunit;
using CommandLine.Text;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using CommandLine.Core;
using System.Reflection;
using CSharpx;
using RailwaySharp.ErrorHandling;

namespace CommandLine.Tests.Unit
{
	// Reference: PR #684
    public class SequenceParsingTests
    {
        // Issue #91
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void Enumerable_with_separator_before_values_does_not_try_to_parse_too_much(bool useGetoptMode)
        {
            var args = "--exclude=a,b InputFile.txt".Split();
            var expected = new Options_For_Issue_91 {
                Excluded = new[] { "a", "b" },
                Included = Enumerable.Empty<string>(),
                InputFileName = "InputFile.txt",
            };
            var sut = new Parser(parserSettings => { parserSettings.GetoptMode = useGetoptMode; });
            var result = sut.ParseArguments<Options_For_Issue_91>(args);
            result.Should().BeOfType<Parsed<Options_For_Issue_91>>();
            result.As<Parsed<Options_For_Issue_91>>().Value.Should().BeEquivalentTo(expected);
        }

        // Issue #396
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void Options_with_similar_names_are_not_ambiguous(bool useGetoptMode)
        {
            var args = new[] { "--configure-profile", "deploy", "--profile", "local" };
            var expected = new Options_With_Similar_Names { ConfigureProfile = "deploy", Profile = "local", Deploys = Enumerable.Empty<string>() };
            var sut = new Parser(parserSettings => { parserSettings.GetoptMode = useGetoptMode; });
            var result = sut.ParseArguments<Options_With_Similar_Names>(args);
            result.Should().BeOfType<Parsed<Options_With_Similar_Names>>();
            result.As<Parsed<Options_With_Similar_Names>>().Value.Should().BeEquivalentTo(expected);
        }

        // Issue #420
        [Fact]

        public static void Values_with_same_name_as_sequence_option_do_not_cause_later_values_to_split_on_separators()
        {
            var args = new[] { "c", "x,y" };
            var tokensExpected = new[] { Token.Value("c"), Token.Value("x,y") };
            var typeInfo = typeof(Options_With_Similar_Names_And_Separator);

            var specProps = typeInfo.GetSpecifications(pi => SpecificationProperty.Create(
                    Specification.FromProperty(pi), pi, Maybe.Nothing<object>()))
                .Select(sp => sp.Specification)
                .OfType<OptionSpecification>();

            var tokenizerResult = Tokenizer.ConfigureTokenizer(StringComparer.InvariantCulture, false, false)(args, specProps);
            var tokens = tokenizerResult.SucceededWith();
            tokens.Should().BeEquivalentTo(tokensExpected);
        }

        // Issue #454
        [Theory]
        [InlineData(false)]
        [InlineData(true)]

        public static void Enumerable_with_colon_separator_before_values_does_not_try_to_parse_too_much(bool useGetoptMode)
        {
            var args = "-c chanA:chanB file.hdf5".Split();
            var expected = new Options_For_Issue_454 {
                Channels = new[] { "chanA", "chanB" },
                ArchivePath = "file.hdf5",
            };
            var sut = new Parser(parserSettings => { parserSettings.GetoptMode = useGetoptMode; });
            var result = sut.ParseArguments<Options_For_Issue_454>(args);
            result.Should().BeOfType<Parsed<Options_For_Issue_454>>();
            result.As<Parsed<Options_For_Issue_454>>().Value.Should().BeEquivalentTo(expected);
        }

        // Issue #510
        [Theory]
        [InlineData(false)]
        [InlineData(true)]

        public static void Enumerable_before_values_does_not_try_to_parse_too_much(bool useGetoptMode)
        {
            var args = new[] { "-a", "1,2", "c" };
            var expected = new Options_For_Issue_510 { A = new[] { "1", "2" }, C = "c" };
            var sut = new Parser(parserSettings => { parserSettings.GetoptMode = useGetoptMode; });
            var result = sut.ParseArguments<Options_For_Issue_510>(args);
            result.Should().BeOfType<Parsed<Options_For_Issue_510>>();
            result.As<Parsed<Options_For_Issue_510>>().Value.Should().BeEquivalentTo(expected);
        }

        // Issue #617
        [Theory]
        [InlineData(false)]
        [InlineData(true)]

        public static void Enumerable_with_enum_before_values_does_not_try_to_parse_too_much(bool useGetoptMode)
        {
            var args = "--fm D,C a.txt".Split();
            var expected = new Options_For_Issue_617 {
                Mode = new[] { FMode.D, FMode.C },
                Files = new[] { "a.txt" },
            };
            var sut = new Parser(parserSettings => { parserSettings.GetoptMode = useGetoptMode; });
            var result = sut.ParseArguments<Options_For_Issue_617>(args);
            result.Should().BeOfType<Parsed<Options_For_Issue_617>>();
            result.As<Parsed<Options_For_Issue_617>>().Value.Should().BeEquivalentTo(expected);
        }

        // Issue #619
        [Theory]
        [InlineData(false)]
        [InlineData(true)]

        public static void Separator_just_before_values_does_not_try_to_parse_values(bool useGetoptMode)
        {
            var args = "--outdir ./x64/Debug --modules ../utilities/x64/Debug,../auxtool/x64/Debug m_xfunit.f03 m_xfunit_assertion.f03".Split();
            var expected = new Options_For_Issue_619 {
                OutDir = "./x64/Debug",
                ModuleDirs = new[] { "../utilities/x64/Debug", "../auxtool/x64/Debug" },
                Ignores = Enumerable.Empty<string>(),
                Srcs = new[] { "m_xfunit.f03", "m_xfunit_assertion.f03" },
            };
            var sut = new Parser(parserSettings => { parserSettings.GetoptMode = useGetoptMode; });
            var result = sut.ParseArguments<Options_For_Issue_619>(args);
            result.Should().BeOfType<Parsed<Options_For_Issue_619>>();
            result.As<Parsed<Options_For_Issue_619>>().Value.Should().BeEquivalentTo(expected);
        }
    }
}
