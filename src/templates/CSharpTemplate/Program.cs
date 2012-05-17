#region License
//
// Your Project Name Here: Program.cs
//
// Author:
//   Your Name Here (insert-your@email.here)
//
// Copyright (C) 2012 Your Name Here
//
// [License Body Here]
#endregion
#region Using Directives
using System;
using CommandLine;
using CommandLine.Text;
#endregion

namespace CSharpTemplate
{
	sealed class Program
	{
		sealed class Options : CommandLineOptionsBase
		{
			[Option("t", "text", Required = true, HelpText = "text value here")]
			public string TextValue { get; set; }
			
			[Option("n", "numeric", HelpText = "numeric value here")]
			public double NumericValue { get; set; }
			
			[Option("b", "bool", HelpText = "on|off switch here")]
			public bool BooleanValue { get; set; }
			
			[HelpOption]
			public string GetUsage()
			{
				var help = new HelpText {
					Heading = new HeadingInfo(ThisAssembly.Title, ThisAssembly.InformationalVersion),
					Copyright = new CopyrightInfo(ThisAssembly.Author, 2012),
					AdditionalNewLineAfterOption = true,
					AddDashesToOption = true
				};
				this.HandleParsingErrorsInHelp(help);
				help.AddPreOptionsLine("<<license details here.>>");
				help.AddPreOptionsLine("Usage: CSharpTemplate -tSomeText --numeric 2012 -b");
				help.AddOptions(this);
				
				return help;
			}
			
			void HandleParsingErrorsInHelp(HelpText help)
			{
				if (this.LastPostParsingState.Errors.Count > 0)
				{
					var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
					if (!string.IsNullOrEmpty(errors))
					{
						help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
						help.AddPreOptionsLine(errors);
					}
				}
			}
		}
			
		static void Main(string[] args)
		{
			var options = new Options();
			if (CommandLineParser.Default.ParseArguments(args, options))
			{
				Console.WriteLine("t|ext: " + options.TextValue);
				Console.WriteLine("n|umeric: " + options.NumericValue.ToString());
				Console.WriteLine("b|ool: " + options.BooleanValue.ToString().ToLower());
			}
		}
	}
}