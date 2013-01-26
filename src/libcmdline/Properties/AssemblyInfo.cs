#region License
//
// Command Line Library: AssemblyInfo.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2013 Giacomo Stelluti Scala
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
//
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
    "00240000048000009400000006020000002400005253413100040000010001000bf5a5553b4857" +
    "02b1eab63601bf8e4dce95f709b02aa8359f91fa50861870a9890ffb2f2ed32e80afceb8f83a54" +
    "5169a05d18db95f6250f1c8073e90d8f3ff9cf7c4df48e58f4d6f0937e78a466bae12ecb3c70ec" +
    "c1dc4b680c937a0530dc99623cfc2c0a8ca56237f8ccae2403768d64fc4c0146d0a74d81f61030" +
    "749d1fb8")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
//[assembly: AssemblyCompany("")]
//[assembly: AssemblyTrademark("")]