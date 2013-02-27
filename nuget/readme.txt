Command Line Parser Library 1.9.71.2 stable
------------------------------------------
Giacomo Stelluti Scala
(gsscoder@gmail.com)

GitHub (Latest Sources, Updated Docs): https://github.com/gsscoder/commandline
Codeplex (Binary Downloads): http://commandline.codeplex.com/

Remarks:
 - IParser and IParserSettings interface were removed.

Upgrading from < 1.9.6.1 rc1:
-----------------------------
Now CommandLine.Parser is defiend as:
interface CommandLine.Parser {
  bool ParseArguments(string[] args, object options);
  bool ParseArguments(string[] args, object options, Action<string, object> onVerbCommand);
  bool ParseArgumentsStrict(string[] args, object options, Action onFail = null);
  bool ParseArgumentsStrict(string[] args, object options, Action<string, object> onVerbCommand, Action onFail = null);
}
Please refer to wiki (https://github.com/gsscoder/commandline/wiki).
For help screen in verb command scenario use new HelpText::AutoBuild(object,string).

Upgrading from < 1.9.4.91 versions:
-----------------------------------
- Use System.Char for short name:
  [Option('o', "my-option", DefaultValue=10, HelpText="This is an option!")]
  public int MyOption { get; set; }
- Receive parsing errors without CommandLineOptionsBase (removed):
  public class Options {
    [ParserState]
    public IParserState LastParserState { get; set; }
  }
- Types rename:
  MultiLineTextAttribute -> MultilineTextAttribute (first 'L' -> lowercase)
  CommandLineParser -> Parser (suggestion: qualify with namespace -> CommandLine.Parser).
  ICommandLineParser -> IParser
  CommandLineParserSettings -> ParserSettings
  CommandLineParserException -> ParserException

Upgrading from 1.8.* versions:
------------------------------
The major API change is that all attributes that inherits from BaseOptionAttribute now
apply only to properties. Fields are no longer supported.

Old Code:
---------
class Options {
  [Option("o", "my-option", HelpText="This is an option!")]
  public int MyOption = 10;
}

New Code:
---------
class Options {
  [Option("o", "my-option", DefaultValue=10, HelpText="This is an option!")]
  public int MyOption { get; set; }
}

As you can see I've added the new DefaultValue property to help you initialize properties.

Shortcut for Help Screen
------------------------
[HelpOption]
public string GetUsage()
{
  return HelpText.AutoBuild(this,
    (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
}

Note:
-----
If you don't use mutually exclusive options, now there's a singleton built for common uses:

if (CommandLineParser.Default.ParseArguments(args, options)) {
  // consume values here
}

Have fun!
