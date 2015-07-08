using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace ReadText.Demo
{
	class Program
	{
		public static void Main(string[] args)
		{
            var result = Parser.Default.ParseArguments<HeadOptions, TailOptions>(args);
            if (result.Errors.Count() > 0)
            {
                Environment.Exit(1);
            }

            if (result.Value.GetType () == typeof(HeadOptions))
            {
            }

            // TODO: complete...
		}
	}
}
