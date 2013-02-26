#region License
// <copyright file="TargetCapabilitiesExtensions.cs" company="Giacomo Stelluti Scala">
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
using CommandLine.Infrastructure;
#endregion

namespace CommandLine.Parsing
{
    /// <summary>
    /// Utility extension methods for query target capabilities.
    /// </summary>
    internal static class TargetCapabilitiesExtensions
    {
        public static bool HasVerbs(this object target)
        {
            return ReflectionHelper.RetrievePropertyList<VerbOptionAttribute>(target).Count > 0;
        }

        public static bool HasHelp(this object target)
        {
            return ReflectionHelper.RetrieveMethod<HelpOptionAttribute>(target) != null;
        }

        public static bool HasVerbHelp(this object target)
        {
            return ReflectionHelper.RetrieveMethod<HelpVerbOptionAttribute>(target) != null;
        }

        public static bool CanReceiveParserState(this object target)
        {
            return ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(target).Count > 0;
        }
    }
}