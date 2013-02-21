#region License
// <copyright file="AssemblyInfo.cs" company="Giacomo Stelluti Scala">
//   Copyright 2015-2013 Giacomo Stelluti Scala
// </copyright>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
#region Using Directives
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

[assembly: AssemblyTitle("CommandLine.dll")]
[assembly: AssemblyDescription("Command Line Parser Library allows CLR applications to define a syntax for parsing command line arguments.")]
[assembly: AssemblyCulture("")]
[assembly: InternalsVisibleTo("CommandLine.Tests, PublicKey=" +
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