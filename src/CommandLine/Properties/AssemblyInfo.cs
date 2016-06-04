// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("CommandLine.dll")]
[assembly: AssemblyDescription("Command Line Parser Library allows CLR applications to define a syntax for parsing command line arguments.")]
[assembly: AssemblyCulture("")]
[assembly: InternalsVisibleTo("CommandLine.Tests, PublicKey=" +
    "002400000480000094000000060200000024000052534131000400000100010015eb7571d696c0" +
    "75627830f9468969103bc35764467bdbccfc0850f2fbe6913ee233d5d7cf3bbcb870fd42e6a8cc" +
    "846d706b5cef35389e5b90051991ee8b6ed73ee1e19f108e409be69af6219b2e31862405f4b8ba" +
    "101662fbbb54ba92a35d97664fe65c90c2bebd07aef530b01b709be5ed01b7e4d67a6b01c8643e" +
    "42a20fb4")]
#if PLATFORM_DOTNET
[assembly: InternalsVisibleTo("CommandLine.DotNet.Tests, PublicKey=" +
    "002400000480000094000000060200000024000052534131000400000100010015eb7571d696c0" +
    "75627830f9468969103bc35764467bdbccfc0850f2fbe6913ee233d5d7cf3bbcb870fd42e6a8cc" +
    "846d706b5cef35389e5b90051991ee8b6ed73ee1e19f108e409be69af6219b2e31862405f4b8ba" +
    "101662fbbb54ba92a35d97664fe65c90c2bebd07aef530b01b709be5ed01b7e4d67a6b01c8643e" +
    "42a20fb4")]
#endif
[assembly: InternalsVisibleTo("CommandLine.FSharp, PublicKey=" +
    "002400000480000094000000060200000024000052534131000400000100010015eb7571d696c0" +
    "75627830f9468969103bc35764467bdbccfc0850f2fbe6913ee233d5d7cf3bbcb870fd42e6a8cc" +
    "846d706b5cef35389e5b90051991ee8b6ed73ee1e19f108e409be69af6219b2e31862405f4b8ba" +
    "101662fbbb54ba92a35d97664fe65c90c2bebd07aef530b01b709be5ed01b7e4d67a6b01c8643e" +
    "42a20fb4")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]