// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    [Verb("add", HelpText = "Add file contents to the index.")]
    class AddOptions
    {
        [Option('p', "patch", SetName = "mode",
            HelpText = "Interactively choose hunks of patch between the index and the work tree and add them to the index.")]
        public bool Patch { get; set; }

        [Option('f', "force", SetName = "mode",
            HelpText = "Allow adding otherwise ignored files.")]
        public bool Force { get; set; }

        [Value(0)]
        public string FileName { get; set; }
    }

    [Verb("commit", HelpText = "Record changes to the repository.")]
    class CommitOptions
    {
        [Option('p', "patch",
            HelpText = "Use the interactive patch selection interface to chose which changes to commit.")]
        public bool Patch { get; set; }

        [Option("amend", HelpText = "Used to amend the tip of the current branch.")]
        public bool Amend { get; set; }
    }

    [Verb("clone", HelpText = "Clone a repository into a new directory.")]
    class CloneOptions
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

}