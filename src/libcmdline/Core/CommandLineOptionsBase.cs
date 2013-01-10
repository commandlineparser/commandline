#region License
//
// Command Line Library: CommandLineOptionsBase.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using CommandLine.Internal;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Provides the abstract base class for a strongly typed options target. Used when you need to get parsing errors.
    /// </summary>
    public abstract class CommandLineOptionsBase
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="CommandLineOptionsBase"/> derived class 
        /// </summary>
        protected CommandLineOptionsBase()
        {
            LastPostParsingState = new PostParsingState();
        }

        /// <summary>
        /// Provides data of the state final parser to derived classes .
        /// </summary>
        protected PostParsingState LastPostParsingState { get; private set; }

        internal PostParsingState InternalLastPostParsingState
        {
            get { return LastPostParsingState; }
        }
    }
}