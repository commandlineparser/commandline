Command Line Parser Library 1.9.3.34 Stable
-------------------------------------------
Giacomo Stelluti Scala
(gsscoder@gmail.com)

Codeplex: http://commandline.codeplex.com/
GitHub (Latest Sources, Updated Docs): https://github.com/gsscoder/commandline

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
