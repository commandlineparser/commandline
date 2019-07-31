# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
-  Added AutoHelp and AutoVersion properties to control adding of implicit 'help' and 'version' options/verbs by [@Athari](https://github.com/commandlineparser/commandline/pull/256). 
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

## [1.9.71.2] - 2013-02-27
The starting bascode version