#region License
//
// Command Line Library: SimpleOptions.cs
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
using CommandLine;
#endregion

namespace CommandLine.Tests.Fakes
{
    public class OptionsForAppWithPlugIns //: CommandLineOptionsBase
    {
        [Option('p', "plugin", Required = true, HelpText = "Plug-In to activate.")]
        public string PlugInName { get; set; }
    }

    public class OptionsOfPlugInX
    {
        [Option("filename", Required = true, HelpText = "Plug-In X input filename.")]
        public string InputFileName { get; set; }

        [Option('s', "seek", DefaultValue = 10, HelpText = "Start offset to begin read.")] 
        public long ReadOffset { get; set; }
    }
}