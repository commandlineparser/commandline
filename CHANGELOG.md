# Changelog
All notable changes to this project will be documented in this file.

CommandLineParser project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.8.0] - 2020-5-1
## [2.8.0-preview4] - 2020-4-30
## [2.8.0-preview1] - 2020-3-14

### Added
- Added support for async programming for `WithParsed and WithNotParsed` by [@joseangelmt, PR# 390 ](https://github.com/commandlineparser/commandline/pull/390).
- Publish a new symbol packages with source link support for c# and F# (.snupkg) to improved package debugging experience by [@moh-hassan, PR#554](https://github.com/commandlineparser/commandline/pull/554)
- Add default verb support by [@Artentus, PR# 556](https://github.com/commandlineparser/commandline/pull/556).
- Add more details for localized attribute properties  by [@EdmondShtogu, PR# 558](https://github.com/commandlineparser/commandline/pull/558)
- Support Default in Group Options and raise error if both SetName and Group are applied on option by [@hadzhiyski, PR# 575](https://github.com/commandlineparser/commandline/pull/575).
- Support mutable types without empty constructor that only does explicit implementation of interfaces by [@pergardebrink, PR#590](https://github.com/commandlineparser/commandline/pull/590).


### Changed
- Tests cleanup by [@gsscoder, PR# 560](https://github.com/commandlineparser/commandline/pull/560).
- Upgraded parts of CSharpx from Version 1.6.2-alpha by [@gsscoder, PR# 561](https://github.com/commandlineparser/commandline/pull/561).
- Upgraded RailwaySharp from Version 1.1.0 by [@gsscoder, PR# 562](https://github.com/commandlineparser/commandline/pull/562).
- SkipDefault is being respected by [Usage] Examples by [@kendfrey, PR# 565](https://github.com/commandlineparser/commandline/pull/565).
- Remove useless testing code by [@gsscoder, PR# 568](https://github.com/commandlineparser/commandline/pull/568).
- Remove constraint on T for ParseArguments with factory (required by issue #70) by [@pergardebrink](https://github.com/commandlineparser/commandline/pull/590).
- Update nuget api key by [@ericnewton76](https://github.com/commandlineparser/commandline/commit/2218294550e94bcbc2b76783970541385eaf9c07)

### Fixed
- Fix #579 Unable to parse TimeSpan given from the FormatCommandLine by [@gsscoder, PR# 580](https://github.com/commandlineparser/commandline/pull/580).
- Fix issue #339 for using custom struct having a constructor with string parameter by [moh-hassan, PR# 588](https://github.com/commandlineparser/commandline/pull/588).
- Fix issue #409 to avoid IOException break in Debug mode in WPF app by [moh-hassan, PR# 589 ](https://github.com/commandlineparser/commandline/pull/589).


## [2.7.82] - 2020-1-1
## [2.7.0] - 2020-1-1
### Added
- Add option groups feature by [@hadzhiyski](https://github.com/commandlineparser/commandline/pull/552) - When one or more options has group set, at least one of these properties should have set value (they behave as required).
- Add a new overload method for AutoBuild to enable HelpText customization by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/557).
- Improve spacing in HelpText by [@asherber](https://github.com/commandlineparser/commandline/pull/494) by adding a new option in the HelpText.
- Add a new option "SkipDefault" in UnParserSettings by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/550) to add the ability of skipping the options with a default value and fix [#541](https://github.com/commandlineparser/commandline/issues/541).
- Generate a new symbolic nuget Package by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/554) to Improve the debugging of Applications with the  NuGet package using [symbols experience](https://github.com/NuGet/Home/wiki/NuGet-Package-Debugging-&-Symbols-Improvements).
- Add Support to [SourceLink](https://github.com/dotnet/sourcelink/blob/master/docs/README.md) in the nuget package  [@moh-hassan](https://github.com/commandlineparser/commandline/pull/554).

### Changed
- Remove the Exception when both CompanyAttribute and CopyRightAttribute are null in the Excuting assembly and set the copyright text to a default value by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/557).
- Change the default copyright to include current year instead of 1 by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/557).
- Enabling c# 8 and Vs2019 image in Appveyor.

### Fixed
- Fix NullReferenceException when creating a default immutable instance by [@0xced](https://github.com/commandlineparser/commandline/pull/495).
- Fix issue [#496](https://github.com/commandlineparser/commandline/issues/496) - Cryptic error message with immutable option class by[@moh-hassan](https://github.com/commandlineparser/commandline/pull/555).
- Fix UnParserExtensions.FormatCommandLine by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/550) to resolve:
  -  Fix Quote for Options of type DatTime [#502](https://github.com/commandlineparser/commandline/issues/502) and [#528](https://github.com/commandlineparser/commandline/issues/258).
  - Fix Quote for options of type TimeSpan and DateTimeOffset.
  - Fix Nullable type [#305](https://github.com/commandlineparser/commandline/issues/305)

- Fix nuget Licence in nuget package by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/549) and fix issue  [#545](https://github.com/commandlineparser/commandline/issues/545).
- Fix PackageIconUrl warning in nuget package by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/551).
- Fix immutable nullException, Improve exception message when immutable type can't be created
- Fix Custom help for verbs issue[#529](https://github.com/commandlineparser/commandline/issues/529) by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/557).
- Fix --help switch throwing exception in F# [#366](https://github.com/commandlineparser/commandline/issues/366)
by [@WallaceKelly](https://github.com/commandlineparser/commandline/pull/493)

## [2.6.0] - 2019-07-31
### Added
- Support HelpText localization with ResourceType property by [@tkouba](https://github.com/commandlineparser/commandline/pull/356).
- Add demo for complete localization of command line help using resources by[@tkouba](https://github.com/commandlineparser/commandline/pull/485).
- Localize VerbAttribute  by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/473).
- Improve support for multiline help text by [@NeilMacMullen](https://github.com/commandlineparser/commandline/pull/456/).
- Reorder options in auto help text (issue #482) [@b3b00](https://github.com/commandlineparser/commandline/pull/484).
- Add IsHelp() and IsVersion() Extension methods to mange HelpText errors by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/467).

### Fixed
- Fix issues for HelpText.AutoBuild configuration (issues #224 , # 259) by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/467).
- Test maintainance: add missed tests and removing xUnit1013 warning by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/462).
- Fix issue #104 of nullable enum by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/453).
- Fix issue #418, modify version screen to print a new line at the end by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/443).


## [2.5.0] - 2019-04-27
### Added
- Add support to  NET40 and NET45 for both CSharp and FSharp by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/430).

 
### Changed
- Proposed changes for enhancement by [@Wind010](https://github.com/commandlineparser/commandline/pull/314), cover:appveyor.yml, ReflectionExtensions.cs and error.cs.
- Enhance the CSharp demo to run in multi-target net40;net45;netcoreapp2.0;netcoreapp2.1 by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/430).
- Added explicit support for .NET 4.6.1 and .NET Core 2.0 by [@ravenpride](https://github.com/commandlineparser/commandline/pull/400). 
- Convert commandline project to multi-target project netstandard2.0;net40;net45;net461.
- Convert commandline Test to multi-target project net461;netcoreapp2.0. 



### Fixed
- Fix the null EntryAssembly Exception in unit test of net4x projects: issues #389,#424 by [@moh-hassan](https://github.com/commandlineparser/commandline/pull/430).
- Fix the test case 'Add unit tests for Issue #389 and #392
- Fix CSC error CS7027: Error signing output with public key from file 'CommandLine.snk' -- Invalid public key in appveyor CI.
- Fix the error CS0234: The type or namespace name 'FSharp' for net40 Framework.
- Fix Mis-typed CommandLine.BaseAttribute.Default results in ArgumentException: Object of type 'X' cannot be converted to type 'Y' (issue #189) by[@Wind010](https://github.com/commandlineparser/commandline/pull/314).




## [2.4.3] - 2019-01-09
### Added
- Add support to  NetStandard2.0 by [@ViktorHofer](https://github.com/commandlineparser/commandline/pull/307) 
- Add strong name signing  [@ViktorHofer](https://github.com/commandlineparser/commandline/pull/307) 
-  Added AutoBuild and AutoVersion properties to control adding of implicit 'help' and 'version' options/verbs by [@Athari](https://github.com/commandlineparser/commandline/pull/256). 
- Added simpler C# Quick Start example at readme.md by [@lythix](https://github.com/commandlineparser/commandline/pull/274).
- Add validate feature in Set parameter, and throw exception, and show usage,Issue #283 by[@e673](https://github.com/commandlineparser/commandline/pull/286).


### Deprecated
- Drop support for NET40 and NET45


### Removed
- Disable faulty tests in netsatbdard2.0 and enable testing in CI.


### Fixed
- Fix grammar error in specification error message by [@DillonAd](https://github.com/commandlineparser/commandline/pull/276).
- Fix HelpText.AutoBuild Usage spacing  by[@ElijahReva](https://github.com/commandlineparser/commandline/pull/280).
- Fix type at readme.md file by [@matthewjberger](https://github.com/commandlineparser/commandline/pull/304)
- Fix not showing correct header info, issue #34 by[@tynar](https://github.com/commandlineparser/commandline/pull/312).
- Fix title of assembly renders oddly issue-#197 by [@Yiabiten](https://github.com/commandlineparser/commandline/pull/344).
- Fix nuget apikey by [@ericnewton76](https://github.com/commandlineparser/commandline/pull/386).
- Fix missing fsharp from github release deployment by @ericnewton76.
- Fix to Display Width Tests by [@Oddley](https://github.com/commandlineparser/commandline/pull/278).
- Fixing DisplayWidth for newer Mono  by [@Oddley](https://github.com/commandlineparser/commandline/pull/279).


## [2.3.0] - 2018-08-13
### Added
- Properly handle CaseInsensitiveEnumValues flag fixing issue #198 by [@niklaskarl](https://github.com/commandlineparser/commandline/pull/231).

### Changed
- Updated README examples quick start example for c# and Vb.net to work with the new API by [@loligans](https://github.com/commandlineparser/commandline/pull/218).
- Updated README by [@ericnewton76](https://github.com/commandlineparser/commandline/pull/208).
- Update copyright in unit tests 
- Patching appveyor dotnet csproj 
- Updates to appveyor to create a build matrix

### Fixed
- hotfix/issue #213 fsharp dependency by [@ericnewton76](https://github.com/commandlineparser/commandline/pull/215).


## [2.2.1] - 2018-01-10

## [2.2.0] - 2018-01-07 

## [1.9.71.2] - 2013-02-27: The starting bascode version
