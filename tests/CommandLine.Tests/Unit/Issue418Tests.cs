using CommandLine.Text;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Tests.Fakes;
using Xunit;
using FluentAssertions;

namespace CommandLine.Tests.Unit
{
	 //issue#418, --version does not print a new line at the end cause trouble in Linux
    public class Issue418Tests
    {	
		
        [Fact]
        public void Explicit_version_request_generates_version_info_screen_with_eol()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<Simple_Options>(new[] { "--version" });
            var result = help.ToString();           
            // Verify outcome
            var lines = result.ToNotEmptyLines();  
            result.Length.Should().BeGreaterThan(0);			
            result.Should().EndWith(Environment.NewLine);
            result.ToNotEmptyLines().Length.Should().Be(1);
           
            // Teardown
        }
	}
}
