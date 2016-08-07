// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;
using CommandLine.Text;

namespace CommandLine.Tests.Fakes
{
    class Simple_Options_Without_HelpText
    {
        [Option('v', "verbose")]
        public bool Verbose { get; set; }

        [Option("input-file")]
        public string FileName { get; set; }

        [Option("secert-option", Hidden = true, HelpText = "This is a description for a secert hidden option that should never be visibile to the user via help text.")]
        public string SecertOption { get; set; }
    }

    class Simple_Options_With_HelpText_Set
    {
        [Option('v', "verbose", HelpText = "Comment extensively every operation.")]
        public bool Verbose { get; set; }

        [Option('i', "input-file", Required = true, HelpText = "Specify input file to be processed.")]
        public string FileName { get; set; }

        [Option("secert-option", Hidden = true, HelpText = "This is a description for a secert hidden option that should never be visibile to the user via help text.")]
        public string SecertOption { get; set; }
    }

    class Simple_Options_With_HelpText_Set_To_Long_Description
    {
        [Option('v', "verbose", HelpText = "This is the description of the verbosity to test out the wrapping capabilities of the Help Text.")]
        public bool Verbose { get; set; }

        [Option("input-file", HelpText = "This is a very long description of the Input File argument that gets passed in.  It should  be passed in as a string.")]
        public string FileName { get; set; }

        [Option("secert-option", Hidden =  true, HelpText = "This is a description for a secert hidden option that should never be visibile to the user via help text.")]
        public string SecertOption { get; set; }
    }

    class Simple_Options_With_HelpText_Set_To_Long_Description_Without_Spaces
    {
        [Option('v', "verbose", HelpText = "Before 012345678901234567890123 After")]
        public bool Verbose { get; set; }

        [Option("input-file", HelpText = "Before 012345678901234567890123456789 After")]
        public string FileName { get; set; }

        [Option("secert-option", Hidden = true, HelpText = "This is a description for a secert hidden option that should never be visibile to the user via help text.")]
        public string SecertOption { get; set; }
    }

    class Options_With_Usage_Attribute
    {
        [Option('i',"input", HelpText = "Set input file.")]
        public string InputFile { get; set; }

        [Option('i', "output", HelpText = "Set output file.")]
        public string OutputFile { get; set; }

        [Option(HelpText = "Set verbosity level.")]
        public bool Verbose { get; set; }

        [Option('w', "warns", HelpText = "Log warnings.")]
        public bool LogWarning { get; set; }

        [Option('e', "errs", HelpText = "Log errors.")]
        public bool LogError { get; set; }

        [Option('l', Separator = ',', HelpText = "List.")]
        public IEnumerable<int> Sequence { get; set; }

        [Value(0, HelpText = "Value.")]
        public string Value { get; set; }

        [Option("secert-option", Hidden = true, HelpText = "This is a description for a secert hidden option that should never be visibile to the user via help text.")]
        public string SecertOption { get; set; }

        [Usage(ApplicationAlias = "mono testapp.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Normal scenario", new Options_With_Usage_Attribute { InputFile = "file.bin", OutputFile = "out.bin" });
                yield return new Example("Logging warnings", UnParserSettings.WithGroupSwitchesOnly() , new Options_With_Usage_Attribute { InputFile = "file.bin", LogWarning = true });
                yield return new Example("Logging errors", new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly() }, new Options_With_Usage_Attribute { InputFile = "file.bin", LogError = true });
                yield return new Example("List", new Options_With_Usage_Attribute { Sequence = new[] { 1, 2 } });
                yield return new Example("Value", new Options_With_Usage_Attribute { Value = "value" });
            }
        }
    }

    [Verb("secert", Hidden = true, HelpText = "This is a secert hidden verb that should never be visible to the user via help text.")]
    public class Secert_Verb
    {
        [Option('f', "force", SetName = "mode-f", HelpText = "Allow adding otherwise ignored files.")]
        public bool Force { get; set; }

        [Option("secert-option", Hidden = true, HelpText = "This is a description for a secert hidden option that should never be visibile to the user via help text.")]
        public string SecertOption { get; set; }
    }

    [Verb("add", HelpText = "Add file contents to the index.")]
    public class Add_Verb_With_Usage_Attribute
    {
        [Option('p', "patch", SetName = "mode-p",
            HelpText = "Interactively choose hunks of patch between the index and the work tree and add them to the index.")]
        public bool Patch { get; set; }

        [Option('f', "force", SetName = "mode-f",
            HelpText = "Allow adding otherwise ignored files.")]
        public bool Force { get; set; }

        [Value(0)]
        public string FileName { get; set; }

        [Option("secert-option", Hidden = true, HelpText = "This is a description for a secert hidden option that should never be visibile to the user via help text.")]
        public string SecertOption { get; set; }

        [Usage(ApplicationAlias = "git")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Forcing file", new Add_Verb { FileName = "README.md", Force = true });
            }
        }
    }

    [Verb("commit", HelpText = "Record changes to the repository.")]
    public class Commit_Verb_With_Usage_Attribute
    {
        [Option('p', "patch",
            HelpText = "Use the interactive patch selection interface to chose which changes to commit.")]
        public bool Patch { get; set; }

        [Option("amend", HelpText = "Used to amend the tip of the current branch.")]
        public bool Amend { get; set; }

        [Option("secert-option", Hidden = true, HelpText = "This is a description for a secert hidden option that should never be visibile to the user via help text.")]
        public string SecertOption { get; set; }

        [Usage(ApplicationAlias = "git")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Committing work", new Commit_Verb_With_Usage_Attribute { Patch = true });
            }
        }
    }

    [Verb("clone", HelpText = "Clone a repository into a new directory.")]
    public class Clone_Verb_With_Usage_Attribute
    {
        [Option("no-hardlinks",
            HelpText = "Optimize the cloning process from a repository on a local filesystem by copying files.")]
        public bool NoHardLinks { get; set; }

        [Option('q', "quiet",
            HelpText = "Suppress summary message.")]
        public bool Quiet { get; set; }

        [Option("secert-option", Hidden = true, HelpText = "This is a description for a secert hidden option that should never be visibile to the user via help text.")]
        public string SecertOption { get; set; }

        [Value(0, MetaName = "URLS",
            HelpText = "A list of url(s) to clone.")]
        public IEnumerable<string> Urls { get; set; }

        [Usage(ApplicationAlias = "git")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Basic cloning", new Clone_Verb_With_Usage_Attribute { Urls = new[] { "https://github.com/gsscoder/csharpx" } });
                yield return new Example("Cloning quietly", new Clone_Verb_With_Usage_Attribute { Quiet = true, Urls = new[] { "https://github.com/gsscoder/railwaysharp" } });
                yield return new Example("Cloning without hard links", new Clone_Verb_With_Usage_Attribute { NoHardLinks = true, Urls = new[] { "https://github.com/gsscoder/csharpx" } });
            }
        }
    }
}
