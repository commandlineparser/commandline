#region License
// <copyright file="Program.cs" company="Your name here">
//   Copyright 2013 Your name here
// </copyright>
//
// [License Body Here]
#endregion

namespace CSharpTemplate
{
    #region Using Directives
    using System;
    using CommandLine;
    using CommandLine.Text;
    #endregion

    internal class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();

            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            Console.WriteLine("t|ext: " + options.TextValue);
            Console.WriteLine("n|umeric: " + options.NumericValue);
            Console.WriteLine("b|ool: " + options.BooleanValue.ToString().ToLowerInvariant());
        }
    }
}
