#region License
//
// Command Line Library: StringUtil.cs
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
#endregion

namespace CommandLine.Internal
{
    internal static class StringUtil
    {
        public static string Spaces(int count)
        {
            return new string(' ', count);
        }

        public static bool IsNumeric(string value)
        {
            decimal temporary;
            return decimal.TryParse(value, out temporary);
        }

        public static bool IsWhiteSpace(int @char)
        {
            return @char == 0x09 || @char == 0x0B || @char == 0x0C || @char == 0x20 || @char == 0xA0 ||
                @char == 0x1680 || @char == 0x180E || (@char >= 8192 && @char <= 8202) || @char == 0x202F ||
                @char == 0x205F || @char == 0x3000 || @char == 0xFEFF;
        }

        public static bool IsLineTerminator(int @char)
        {
            return @char == 0x0A || @char == 0x0D || @char == 0x2028 || @char == 0x2029;
        }
    }
}