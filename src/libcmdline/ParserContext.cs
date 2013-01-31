#region License
//
// Command Line Library: ParserContext.cs
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
using CommandLine.Core;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Models context in which parsing occurs.
    /// </summary>
    sealed class ParserContext
    {
        private ParserContext() {}

        public ParserContext(string[] arguments, object target)
        {
            Arguments = arguments;
            Target = target;
        }

        public ParserContext ToCoreInstance(OptionInfo verbOption)
        {
            var newArguments = new string[Arguments.Length - 1];
            if (Arguments.Length > 1)
            {
                Array.Copy(Arguments, 1, newArguments, 0, Arguments.Length - 1);
            }
            return new ParserContext(newArguments, verbOption.GetValue(Target));
        }

        public string[] Arguments { get; private set; }

        public bool HasNoArguments()
        {
            return Arguments == null || Arguments.Length == 0;
        }

        public bool HasAtLeastOneArgument()
        {
            return !HasNoArguments() && Arguments.Length >= 1;
        }

        public string FirstArgument
        {
            get { return !HasNoArguments() ? Arguments[0] : null; }
        }

        public object Target { get; private set; }
    }
}