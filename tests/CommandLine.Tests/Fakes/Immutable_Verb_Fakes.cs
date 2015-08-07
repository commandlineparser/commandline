// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    [Verb("add", HelpText = "Add file contents to the index.")]
    class Immutable_Add_Verb
    {
        private readonly bool patch;
        private readonly bool force;
        private readonly string fileName;

        public Immutable_Add_Verb(bool patch, bool force, string fileName)
        {
            this.patch = patch;
            this.force = force;
            this.fileName = fileName;
        }

        [Option('p', "patch", SetName = "mode",
            HelpText = "Interactively choose hunks of patch between the index and the work tree and add them to the index.")]
        public bool Patch { get { return patch; } }

        [Option('f', "force", SetName = "mode",
            HelpText = "Allow adding otherwise ignored files.")]
        public bool Force { get { return force; } }

        [Value(0)]
        public string FileName { get { return fileName; } }
    }

    [Verb("commit", HelpText = "Record changes to the repository.")]
    class Immutable_Commit_Verb
    {
        private readonly bool patch;
        private readonly bool amend;

        public Immutable_Commit_Verb(bool patch, bool amend)
        {
            this.patch = patch;
            this.amend = amend;
        }

        [Option('p', "patch",
            HelpText = "Use the interactive patch selection interface to chose which changes to commit.")]
        public bool Patch { get { return patch; } }

        [Option("amend", HelpText = "Used to amend the tip of the current branch.")]
        public bool Amend { get { return amend; } }
    }

    [Verb("clone", HelpText = "Clone a repository into a new directory.")]
    class Immutable_Clone_Verb
    {
        private readonly bool noHardLinks;
        private readonly bool quiet;
        private readonly IEnumerable<string> urls;

        public Immutable_Clone_Verb(bool noHardLinks, bool quiet, IEnumerable<string> urls)
        {
            this.noHardLinks = noHardLinks;
            this.quiet = quiet;
            this.urls = urls;
        }

        [Option("no-hardlinks",
            HelpText = "Optimize the cloning process from a repository on a local filesystem by copying files.")]
        public bool NoHardLinks { get { return noHardLinks; } }

        [Option('q', "quiet",
            HelpText = "Suppress summary message.")]
        public bool Quiet { get { return quiet; } }

        [Value(0)]
        public IEnumerable<string> Urls { get { return urls; } }
    }
}