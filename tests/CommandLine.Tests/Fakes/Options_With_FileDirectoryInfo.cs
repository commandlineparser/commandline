// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.IO;

namespace CommandLine.Tests.Fakes
{
    public class Options_With_FileDirectoryInfo
    {
        [Option('s', "stringPath")]
        public string StringPath { get; set; }

        [Option('f', "filePath")]
        public FileInfo FilePath { get; set; }

        [Option('d', "directoryPath")]
        public DirectoryInfo DirectoryPath { get; set; }
    }
}
