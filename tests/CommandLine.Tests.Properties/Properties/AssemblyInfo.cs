// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Reflection;
using System.Runtime.InteropServices;
using CommandLine.Text;

[assembly: AssemblyTitle("CommandLine.Tests.Properties.dll")]
[assembly: AssemblyDescription("Command Line Parser Library allows CLR applications to define a syntax for parsing command line arguments.")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyLicense(
    "This is free software. You may redistribute copies of it under the terms of",
    "the MIT License <http://www.opensource.org/licenses/mit-license.php>.")]
[assembly: AssemblyUsage(
    "[no usage, this is a dll]")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
//[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: ComVisible(false)]
//[assembly: CLSCompliant(true)]
//[assembly: AssemblyCompany("")]
//[assembly: AssemblyTrademark("")]