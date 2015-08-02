// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    [Verb("add", HelpText = "Add file contents to the index.")]
    public class AddOptions
    {
        [Option('p', "patch", SetName = "mode-p",
            HelpText = "Interactively choose hunks of patch between the index and the work tree and add them to the index.")]
        public bool Patch { get; set; }

        [Option('f', "force", SetName = "mode-f",
            HelpText = "Allow adding otherwise ignored files.")]
        public bool Force { get; set; }

        [Value(0)]
        public string FileName { get; set; }
    }

    [Verb("commit", HelpText = "Record changes to the repository.")]
    public class CommitOptions
    {
        [Option('p', "patch",
            HelpText = "Use the interactive patch selection interface to chose which changes to commit.")]
        public bool Patch { get; set; }

        [Option("amend", HelpText = "Used to amend the tip of the current branch.")]
        public bool Amend { get; set; }
    }

    [Verb("clone", HelpText = "Clone a repository into a new directory.")]
    public class CloneOptions
    {
        [Option("no-hardlinks",
            HelpText = "Optimize the cloning process from a repository on a local filesystem by copying files.")]
        public bool NoHardLinks { get; set; }

        [Option('q', "quiet",
            HelpText = "Suppress summary message.")]
        public bool Quiet { get; set; }

        [Value(0)]
        public IEnumerable<string> Urls { get; set; }
    }

    [Verb("sequence", HelpText = "Sequence options test.")]
    public class SequenceOptions
    {
        [Option("long-seq", Separator = ';')]
        public IEnumerable<long> LongSequence { get; set; }

        [Option('s', Min = 1, Max = 100, Separator = ',')]
        public IEnumerable<string> StringSequence { get; set; }
    }

    abstract class BaseFileOptions
    {
        [Option('p', "patch", SetName = "mode",
           HelpText = "Interactively choose hunks of patch between the index and the work tree and add them to the index.")]
        public bool Patch { get; set; }

        [Value(0)]
        public string FileName { get; set; }
    }

    [Verb("derivedadd", HelpText = "Add file contents to the index.")]
    class DerivedAddOptions : BaseFileOptions
    {
        [Option('f', "force", SetName = "mode",
            HelpText = "Allow adding otherwise ignored files.")]
        public bool Force { get; set; }
    }
}