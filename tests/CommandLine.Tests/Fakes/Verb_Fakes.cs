// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    [Verb("add", HelpText = "Add file contents to the index.")]
    public class Add_Verb
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
    [Verb("add", isDefault:true,HelpText = "Add file contents to the index.")]
    public class Add_Verb_As_Default
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
    public class Commit_Verb
    {
        [Option('p', "patch",
            HelpText = "Use the interactive patch selection interface to chose which changes to commit.")]
        public bool Patch { get; set; }

        [Option("amend", HelpText = "Used to amend the tip of the current branch.")]
        public bool Amend { get; set; }

        [Option('m', "message", HelpText = "Use the given message as the commit message.")]
        public string Message { get; set; }
    }

    [Verb("clone", HelpText = "Clone a repository into a new directory.")]
    public class Clone_Verb
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

    abstract class Base_Class_For_Verb
    {
        [Option('p', "patch", SetName = "mode",
           HelpText = "Interactively choose hunks of patch between the index and the work tree and add them to the index.")]
        public bool Patch { get; set; }

        [Value(0)]
        public string FileName { get; set; }
    }

    [Verb("derivedadd", HelpText = "Add file contents to the index.")]
    class Derived_Verb : Base_Class_For_Verb
    {
        [Option('f', "force", SetName = "mode",
            HelpText = "Allow adding otherwise ignored files.")]
        public bool Force { get; set; }
    }

    [Verb("test")]
    class Verb_With_Option_And_Value_Of_String_Type
    {
        [Option('o', "opt")]
        public string OptValue { get; set; }

        [Value(0)]
        public string PosValue { get; set; }
    }

    [Verb("default1", true)]
    class Default_Verb_One
    {
        [Option('t', "test-one")]
        public bool TestValueOne { get; set; }
    }

    [Verb("default2", true)]
    class Default_Verb_Two
    {
        [Option('t', "test-two")]
        public bool TestValueTwo { get; set; }
    }

    [Verb(null, true)]
    class Default_Verb_With_Empty_Name
    {
        [Option('t', "test")]
        public bool TestValue { get; set; }
    }
    
    [Verb("Verb1")] public class Option1 { }
    [Verb("Verb2")] public class Option2 { }
    [Verb("Verb3")] public class Option3 { }
    [Verb("Verb4")] public class Option4 { }
    [Verb("Verb5")] public class Option5 { }
    [Verb("Verb6")] public class Option6 { }
    [Verb("Verb7")] public class Option7 { }
    [Verb("Verb8")] public class Option8 { }
    [Verb("Verb9")] public class Option9 { }
    [Verb("Verb10")] public class Option10 { }
    [Verb("Verb11")] public class Option11 { }
    [Verb("Verb12")] public class Option12 { }
    [Verb("Verb13")] public class Option13 { }
    [Verb("Verb14")] public class Option14 { }
    [Verb("Verb15")] public class Option15 { }
    [Verb("Verb16")] public class Option16 { }
    [Verb("Verb17")] public class Option17 { }
}
