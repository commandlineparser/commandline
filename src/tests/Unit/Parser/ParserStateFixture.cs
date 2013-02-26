using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace CommandLine.Tests.Unit.Parser
{
    class FakeParserState : IParserState
    {
        public FakeParserState()
        {
            Errors = new List<ParsingError>();
        }

        public IList<ParsingError> Errors { get; private set; }
    }

    class FakeOptionsWithPreBuiltParserState
    {
        public FakeOptionsWithPreBuiltParserState()
        {
            BadParserState = new FakeParserState();
        }

        public string GetUsage()
        {
            return "FakeOptionsWithPreBuiltParserState::GetUsage()";
        }

        [Option(Required = true)]
        public string Foo { get; set; }

        [ParserState]
        public IParserState BadParserState { get; set; }
    }

    class FakeOptionsWithParserStateAttributeAppliedInWrongWay
    {
        public FakeOptionsWithParserStateAttributeAppliedInWrongWay()
        {
            EvenWorseParserState = string.Empty;
        }

        [Option(Required = true)]
        public string Bar { get; set; }

        [ParserState]
        public string EvenWorseParserState { get; set; }
    }

    class FakeOptionsWithParserStateAttributeAppliedInWrongWayAndNotInitialized
    {
        [Option(Required = true)]
        public string Bar { get; set; }

        [ParserState]
        public StringBuilder EvenWorseParserState { get; set; }
    }

    public class ParserStateFixture
    {
        [Fact]
        public void Parser_state_instance_should_not_pre_built()
        {
            var options = new FakeOptionsWithPreBuiltParserState();

            Assert.ThrowsDelegate act = () => new CommandLine.Parser(
                with => with.HelpWriter = new StringWriter()).ParseArguments(new[] { "--bar" }, options);

            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void Parser_state_attribute_should_be_applied_to_a_property_of_the_correct_type()
        {
            var options = new FakeOptionsWithParserStateAttributeAppliedInWrongWay();

            Assert.ThrowsDelegate act = () => new CommandLine.Parser(
                with => with.HelpWriter = new StringWriter()).ParseArguments(new[] { "--foo" }, options);

            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void Parser_state_attribute_should_be_applied_to_a_property_of_the_correct_type_also_if_not_initialized()
        {
            var options = new FakeOptionsWithParserStateAttributeAppliedInWrongWayAndNotInitialized();

            Assert.ThrowsDelegate act = () => new CommandLine.Parser(
                with => with.HelpWriter = new StringWriter()).ParseArguments(new[] { "--foo" }, options);

            Assert.Throws<InvalidOperationException>(act);
        }
    }
}
