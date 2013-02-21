#region License
// <copyright file="Options.cs" company="Your name here">
//   Copyright 2013 Your name here
// </copyright>
//
// [License Body Here]
#endregion

namespace CSharpTemplate
{
    #region Using Directives
    using CommandLine;
    using CommandLine.Text;
    #endregion

    internal class Options
    {
        [Option('t', "text", Required = true, HelpText = "text value here")]
        public string TextValue { get; set; }

        [Option('n', "numeric", HelpText = "numeric value here")]
        public double NumericValue { get; set; }

        [Option('b', "bool", HelpText = "on|off switch here")]
        public bool BooleanValue { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
