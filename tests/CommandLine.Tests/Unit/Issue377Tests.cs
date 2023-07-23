using CommandLine.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommandLine.Tests.Unit.Issue389Tests;
using Xunit;
using FluentAssertions;

namespace CommandLine.Tests.Unit
{
    //Reference: PR# 377
    public class Issue377Tests
    {

        // Test the normal behavior of the library
        [Fact]
        public void Test_Read_File_List_With_IEnumerable_And_Type()
        {
            ParserResult<Options_IEnumerable_With_Type> parsedOptions = Parser.Default.ParseArguments<Options_IEnumerable_With_Type>(new string[] { "--read", "file1", "file2" });
            parsedOptions.Tag.Should().Be(ParserResultType.Parsed);
            parsedOptions.Value.Should().NotBeNull();
            parsedOptions.Value.InputFiles.Should().HaveCount(2);
            parsedOptions.Value.InputFiles.First().Should().Be("file1");
            parsedOptions.Value.InputFiles.Last().Should().Be("file2");
        }


        // Test the normal behavior of the library
        [Fact]
        public void Test_Read_File_List_With_IEnumerable_And_Without_Type()
        {
            Action parseUnsupportedType = () => Parser.Default.ParseArguments<Options_IEnumerable_Without_Type>(new string[] { "--read", "file1", "file2" });
            Assert.Throws<InvalidOperationException>(parseUnsupportedType);
        }

        // Tests the behaviour of the library with an unsupported type (i.e. a dictionary)
        [Fact]
        public void Test_Read_File_List_With_IDictionary()
        {
            Action parseUnsupportedType = () => Parser.Default.ParseArguments<Options_Dictionary>(new string[] { "--read", "file1", "file2" });
            Assert.Throws<InvalidOperationException>(parseUnsupportedType);
        }

        // Options with IEnumerable
        internal class Options_IEnumerable_With_Type
        {

            [Option('r', "read", Required = true, HelpText = "Input files to be processed.")]
            public IEnumerable<string> InputFiles { get; set; }

        }

        // Options with IEnumerable
        internal class Options_IEnumerable_Without_Type
        {

            [Option('r', "read", Required = true, HelpText = "Input files to be processed.")]
            public IEnumerable InputFiles { get; set; }

        }

        // Options with unsupported IDictionary type
        internal class Options_Dictionary
        {

            [Option('r', "read", Required = true, HelpText = "Input files to be processed.")]
            public IDictionary<string, string> InputFiles { get; set; }

        }
    }
}
