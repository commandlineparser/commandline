// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    public class Options_For_Issue_91
    {
        [Value(0, Required = true)]
        public string InputFileName { get; set; }

        [Option('o', "output")]
        public string OutputFileName { get; set; }

        [Option('i', "include", Separator = ',')]
        public IEnumerable<string> Included { get; set; }

        [Option('e', "exclude", Separator = ',')]
        public IEnumerable<string> Excluded { get; set; }
    }

    public class Options_For_Issue_454
    {
        [Option('c', "channels", Required = true, Separator = ':', HelpText = "Channel names")]
        public IEnumerable<string> Channels { get; set; }

        [Value(0, Required = true, MetaName = "file_path", HelpText = "Path of archive to be processed")]
        public string ArchivePath { get; set; }
    }

    public class Options_For_Issue_510
    {
        [Option('a', "aa", Required = false, Separator = ',')]
        public IEnumerable<string> A { get; set; }

        [Option('b', "bb", Required = false)]
        public string B { get; set; }
        
        [Value(0, Required = true)]
        public string C { get; set; }
    }

    public enum FMode { C, D, S };

    public class Options_For_Issue_617
    {
        [Option("fm",  Separator=',', Default = new[] { FMode.S })]  
        public IEnumerable<FMode> Mode { get; set; }
            
        [Option('q')]
        public bool q { get;set; }
            
        [Value(0)]
        public IList<string> Files { get; set; }
    }

    public class Options_For_Issue_619
    {
        [Option("verbose", Required = false, Default = false, HelpText = "Generate process tracing information")]
        public bool Verbose { get; set; }

        [Option("outdir", Required = false, Default = ".", HelpText = "Directory to look for object file")]
        public string OutDir { get; set; }

        [Option("modules", Required = true, Separator = ',', HelpText = "Directories to look for module file")]
        public IEnumerable<string> ModuleDirs { get; set; }

        [Option("ignore", Required = false, Separator = ' ', HelpText = "List of additional module name references to ignore")]
        public IEnumerable<string> Ignores { get; set; }

        [Value(0, Required = true, HelpText = "List of source files to process")]
        public IEnumerable<string> Srcs { get; set; }
    }
}
