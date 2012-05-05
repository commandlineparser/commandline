#region License
//
// Command Line Library: StringUtil.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
// Contributor(s):
//   Steven Evans
// 
// Copyright (C) 2005 - 2012 Giacomo Stelluti Scala
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
using System.Globalization;
#endregion

namespace CommandLine
{
    static class StringUtil
    {
		/*
        public static bool StartsWithIgnoreCase(string strA, string strB)
        {
            if (strB == null)
            {
                return false;
            }
            return (string.Compare(strA, 0, strB, 0, strB.Length, StringComparison.OrdinalIgnoreCase) == 0);
        }
        */

		/*
        public static bool EqualsWithIgnoreCase(string strA, string strB)
        {
            if (strB == null)
            {
                return false;
            }
            return (string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 0);
        }
        */
		
		public static string Spaces(int count)
		{
			return new String(' ', count);
		}
    }
}

