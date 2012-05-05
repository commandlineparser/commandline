#region License
//
// Command Line Library: LongOptionParser.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
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
//
#endregion

namespace CommandLine
{
    sealed class LongOptionParser : ArgumentParser
    {
        public LongOptionParser()
            : base()
        {
        }

        public sealed override ParserState Parse(IArgumentEnumerator argumentEnumerator, OptionMap map, object options)
        {
            var parts = argumentEnumerator.Current.Substring(2).Split(new char[] { '=' }, 2);
            var option = map[parts[0]];
            var valueSetting = false;

            if (option == null)
                return ParserState.Failure;

            option.IsDefined = true;

            ArgumentParser.EnsureOptionArrayAttributeIsNotBoundToScalar(option);

            if (!option.IsBoolean)
            {
                if (parts.Length == 1 && (argumentEnumerator.IsLast || !ArgumentParser.IsInputValue(argumentEnumerator.Next)))
                    return ParserState.Failure;

                if (parts.Length == 2)
                {
                    if (!option.IsArray)
                    {
                        valueSetting = option.SetValue(parts[1], options);
                        if (!valueSetting)
                            this.DefineOptionThatViolatesFormat(option);

                        return ArgumentParser.BooleanToParserState(valueSetting);
                    }
                    else
                    {
                        ArgumentParser.EnsureOptionAttributeIsArrayCompatible(option);

                        var items = ArgumentParser.GetNextInputValues(argumentEnumerator);
                        items.Insert(0, parts[1]);

                        valueSetting = option.SetValue(items, options);
                        if (!valueSetting)
                            this.DefineOptionThatViolatesFormat(option);

                        return ArgumentParser.BooleanToParserState(valueSetting);
                    }
                }
                else
                {
                    if (!option.IsArray)
                    {
                        valueSetting = option.SetValue(argumentEnumerator.Next, options);
                        if (!valueSetting)
                            this.DefineOptionThatViolatesFormat(option);

                        return ArgumentParser.BooleanToParserState(valueSetting, true);
                    }
                    else
                    {
                        ArgumentParser.EnsureOptionAttributeIsArrayCompatible(option);

                        var items = ArgumentParser.GetNextInputValues(argumentEnumerator);

                        valueSetting = option.SetValue(items, options);
                        if (!valueSetting)
                            this.DefineOptionThatViolatesFormat(option);

                        return ArgumentParser.BooleanToParserState(valueSetting, true);
                    }
                }
            }
            else
            {
                if (parts.Length == 2)
                    return ParserState.Failure;

                valueSetting = option.SetValue(true, options);
                if (!valueSetting)
                    this.DefineOptionThatViolatesFormat(option);

                return ArgumentParser.BooleanToParserState(valueSetting);
            }
        }
    }
}