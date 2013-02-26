#region License
// <copyright file="CharExtensions.cs" company="Giacomo Stelluti Scala">
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

namespace CommandLine.Extensions
{
    /// <summary>
    /// Utility extension methods for System.Char.
    /// </summary>
    internal static class CharExtensions
    {
        public static bool IsWhiteSpace(this char c)
        {
            switch (c)
            {
                // Regular
                case '\f':
                case '\v':
                case ' ':
                case '\t':
                    return true;

                // Unicode
                default:
                    return c > 127 && char.IsWhiteSpace(c);
            }
        }

        public static bool IsLineTerminator(this char c)
        {
            switch (c)
            {
                case '\xD':
                case '\xA':
                case '\x2028':
                case '\x2029':
                    return true;

                default:
                    return false;
            }
        }
    }
}