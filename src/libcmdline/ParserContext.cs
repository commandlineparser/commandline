#region License
// <copyright file="ParserContext.cs" company="Giacomo Stelluti Scala">
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

namespace CommandLine
{
    #region Using Directives
    using System;
    using CommandLine.Core;
    #endregion

    /// <summary>
    /// Models context in which parsing occurs.
    /// </summary>
    internal sealed class ParserContext
    {
        public ParserContext(string[] arguments, object target)
        {
            this.Arguments = arguments;
            this.Target = target;
        }

        public object Target { get; private set; }

        public string[] Arguments { get; private set; }

        public string FirstArgument
        {
            get { return !this.HasNoArguments() ? this.Arguments[0] : null; }
        }

        public ParserContext ToCoreInstance(OptionInfo verbOption)
        {
            var newArguments = new string[this.Arguments.Length - 1];
            if (this.Arguments.Length > 1)
            {
                Array.Copy(this.Arguments, 1, newArguments, 0, this.Arguments.Length - 1);
            }

            return new ParserContext(newArguments, verbOption.GetValue(this.Target));
        }

        public bool HasNoArguments()
        {
            return this.Arguments == null || this.Arguments.Length == 0;
        }

        public bool HasAtLeastOneArgument()
        {
            return !this.HasNoArguments() && this.Arguments.Length >= 1;
        }
    }
}