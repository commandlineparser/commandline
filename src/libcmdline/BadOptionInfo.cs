#region License
// <copyright file="BadOptionInfo.cs" company="Giacomo Stelluti Scala">
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
    /// <summary>
    /// Models a bad parsed option.
    /// </summary>
    public sealed class BadOptionInfo
    {
        internal BadOptionInfo()
        {
        }

        internal BadOptionInfo(char? shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
        }

        /// <summary>
        /// Gets the short name of the option.
        /// </summary>
        /// <value>Returns the short name of the option.</value>
        public char? ShortName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the long name of the option.
        /// </summary>
        /// <value>Returns the long name of the option.</value>
        public string LongName
        {
            get;
            internal set;
        }
    }
}