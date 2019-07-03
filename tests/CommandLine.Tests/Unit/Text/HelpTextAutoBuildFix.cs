using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Text
{
    public class HelpTextTests2
    {
        [Fact]
        public static void error_ishelp()
        {
            // Fixture setup
            // Exercize system
            var parser = new Parser(x => x.HelpWriter = null);
            var result = parser.ParseArguments<Simple_Options>(new[]{"--help"});

            result .WithNotParsed(errs =>
            {
                errs.IsHelp().Should().BeTrue();
                errs.IsVersion().Should().BeFalse();
            });
        }
        [Fact]
        public static void error_isVersion()
        {
            // Fixture setup
            // Exercize system
            var parser = new Parser(x => x.HelpWriter = null);
            var result = parser.ParseArguments<Simple_Options>(new[]{"--version"});

            result .WithNotParsed(errs =>
            {
                errs.IsHelp().Should().BeFalse();
                errs.IsVersion().Should().BeTrue();
            });
        }
        
        [Fact]
        public static void custom_helptext_with_AdditionalNewLineAfterOption_false()
        {
            // Fixture setup
            // Exercize system
            var parser = new Parser(x => x.HelpWriter = null);
            var result = parser.ParseArguments<Simple_Options>(new[]{"--help"});

            result .WithNotParsed(errs =>
            {
               
                var sut = HelpText.AutoBuild(result,
                    h =>
                    {
                        h.AdditionalNewLineAfterOption = false;
                        return h;
                    }
                    , e => e);
                //Assert
                var expected = new[]
                {
                    "  --help                Display this help screen.",
                    "  --version             Display version information."
                };
                var lines = sut.ToString().ToLines();
                lines.Should().ContainInOrder(expected);
            });
        }

        [Fact]
        public static void custom_helptext_with_AdditionalNewLineAfterOption_true()
        {
            // Fixture setup
            // Exercize system
            var parser = new Parser(x => x.HelpWriter = null);
            var result = parser.ParseArguments<Simple_Options>(new[]{"--help"});

            result .WithNotParsed(errs =>
            {
               
                var sut = HelpText.AutoBuild(result,
                    h =>h //AdditionalNewLineAfterOption =true by default
                    , e => e);
               
                //Assert
                var expected = new[]
                {
                    string.Empty,
                    "  --help                Display this help screen.",
                    string.Empty, 
                    "  --version             Display version information."
                };
                var lines = sut.ToString().ToLines();
                lines.Should().ContainInOrder(expected);
            });
        }

       
        [Fact]
        public static void custom_helptext_with_parser_autohelp_false_and_AdditionalNewLineAfterOption_false()
        {
            // Fixture setup
            // Exercize system
            var parser = new Parser(x =>
            {
                x.HelpWriter = null;
                x.AutoHelp=false;
                //x.AutoVersion=false;
            });
            var result = parser.ParseArguments<Simple_Options>(new[]{"--help"});
            //you could generate help even parser.AutoHelp is disabled
            result .WithNotParsed(errs =>
            {
                errs.IsHelp().Should().BeTrue();
                var sut = HelpText.AutoBuild(result,
                    h =>
                    {
                        h.AdditionalNewLineAfterOption = false;
                        return h;
                    }
                    , e => e);
              
                //Assert
                var expected = new[]
                {
                    "  --help                Display this help screen.",
                    "  --version             Display version information."
                };
                var lines = sut.ToString().ToLines();
                lines.Should().ContainInOrder(expected);
            });
        }

        [Fact]
        public static void custom_helptext_with_autohelp_false()
        {
            // Fixture setup
            // Exercize system
            var parser = new Parser(x =>
            {
                x.HelpWriter = null;
                x.AutoHelp=false;
                //x.AutoVersion=false;
            });
            var result = parser.ParseArguments<Simple_Options>(new[]{"--help"});

            result .WithNotParsed(errs =>
            {
                errs.IsHelp().Should().BeTrue();
                var sut = HelpText.AutoBuild(result,
                    h =>h,e => e);
             
                //Assert
                var expected = new[]
                {
                    string.Empty,
                    "  --help                Display this help screen.",
                    string.Empty, 
                    "  --version             Display version information."
                };
                var lines = sut.ToString().ToLines();
                lines.Should().ContainInOrder(expected);
            });
        }
    }
}
