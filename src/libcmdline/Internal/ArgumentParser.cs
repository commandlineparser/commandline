#region License
//
// Command Line Library: CommandLine.cs
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
    internal abstract class ArgumentParser
    {
        protected ArgumentParser()
        {
            PostParsingState = new List<ParsingError>();
        }

        public abstract ParserState Parse(IArgumentEnumerator argumentEnumerator, OptionMap map, object options);

        public List<ParsingError> PostParsingState { get; private set; }

        protected void DefineOptionThatViolatesFormat(OptionInfo option)
        {
            PostParsingState.Add(new ParsingError(option.ShortName, option.LongName, true));
        }

        public static ArgumentParser Create(string argument, bool ignoreUnknownArguments = false)
        {
            if (StringUtil.IsNumeric(argument)) { return null; }
            if (argument.Equals("-", StringComparison.InvariantCulture)) { return null; }
            if (argument[0] == '-' && argument[1] == '-')
            {
                return new LongOptionParser(ignoreUnknownArguments);
            }
            if (argument[0] == '-')
            {
                return new OptionGroupParser(ignoreUnknownArguments);
            }
            return null;
        }

        public static bool IsInputValue(string argument)
        {
            if (StringUtil.IsNumeric(argument)) { return true; }
            if (argument.Length > 0)
            {
                return argument.Equals("-", StringComparison.InvariantCulture) || argument[0] != '-';
            }
            return true;
        }
#if UNIT_TESTS
        public static IList<string> PublicWrapperOfGetNextInputValues(IArgumentEnumerator ae)
        {
            return GetNextInputValues(ae);
        }
#endif
        protected static IList<string> GetNextInputValues(IArgumentEnumerator ae)
        {
            IList<string> list = new List<string>();
            while (ae.MoveNext())
            {
                if (IsInputValue(ae.Current)) { list.Add(ae.Current); }
                else { break; }
            }
            if (!ae.MovePrevious()) { throw new CommandLineParserException(); }
            return list;
        }

        public static bool CompareShort(string argument, char? option, bool caseSensitive)
        {
            return string.Compare(argument, string.Concat("-", new string(option.Value, 1)), !caseSensitive) == 0;
        }

        public static bool CompareLong(string argument, string option, bool caseSensitive)
        {
            return string.Compare(argument, "--" + option, !caseSensitive) == 0;
        }

        protected static ParserState BooleanToParserState(bool value)
        {
            return BooleanToParserState(value, false);
        }

        protected static ParserState BooleanToParserState(bool value, bool addMoveNextIfTrue)
        {
            if (value && !addMoveNextIfTrue) { return ParserState.Success; }
            if (value)
            {
                return ParserState.Success | ParserState.MoveOnNextElement;
            }
            return ParserState.Failure;
        }

        protected static void EnsureOptionAttributeIsArrayCompatible(OptionInfo option)
        {
            if (!option.IsAttributeArrayCompatible)
            {
                throw new CommandLineParserException();
            }
        }

        protected static void EnsureOptionArrayAttributeIsNotBoundToScalar(OptionInfo option)
        {
            if (!option.IsArray && option.IsAttributeArrayCompatible)
            {
                throw new CommandLineParserException();
            }
        }
    }
}