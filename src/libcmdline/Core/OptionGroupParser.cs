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

namespace CommandLine.Core
{
    sealed class OptionGroupParser : ArgumentParser
    {
        public OptionGroupParser(bool ignoreUnkwnownArguments)
        {
            _ignoreUnkwnownArguments = ignoreUnkwnownArguments;
        }

        public override PresentParserState Parse(IArgumentEnumerator argumentEnumerator, OptionMap map, object options)
        {
            IArgumentEnumerator group = new OneCharStringEnumerator(argumentEnumerator.Current.Substring(1));
            while (group.MoveNext())
            {
                var option = map[group.Current];
                if (option == null)
                {
                    return _ignoreUnkwnownArguments ? PresentParserState.MoveOnNextElement : PresentParserState.Failure;
                }
                option.IsDefined = true;

                ArgumentParser.EnsureOptionArrayAttributeIsNotBoundToScalar(option);

                if (!option.IsBoolean)
                {
                    if (argumentEnumerator.IsLast && group.IsLast)
                    {
                        return PresentParserState.Failure;
                    }
                    bool valueSetting;
                    if (!group.IsLast)
                    {
                        if (!option.IsArray)
                        {
                            valueSetting = option.SetValue(group.GetRemainingFromNext(), options);
                            if (!valueSetting)
                            {
                                DefineOptionThatViolatesFormat(option);
                            }
                            return ArgumentParser.BooleanToParserState(valueSetting);
                        }

                        ArgumentParser.EnsureOptionAttributeIsArrayCompatible(option);

                        var items = ArgumentParser.GetNextInputValues(argumentEnumerator);
                        items.Insert(0, @group.GetRemainingFromNext());

                        valueSetting = option.SetValue(items, options);
                        if (!valueSetting)
                        {
                            DefineOptionThatViolatesFormat(option);
                        }
                        return ArgumentParser.BooleanToParserState(valueSetting, true);
                    }

                    if (!argumentEnumerator.IsLast && !ArgumentParser.IsInputValue(argumentEnumerator.Next))
                    {
                        return PresentParserState.Failure;
                    }
                    else
                    {
                        if (!option.IsArray)
                        {
                            valueSetting = option.SetValue(argumentEnumerator.Next, options);
                            if (!valueSetting)
                            {
                                DefineOptionThatViolatesFormat(option);
                            }
                            return ArgumentParser.BooleanToParserState(valueSetting, true);
                        }

                        ArgumentParser.EnsureOptionAttributeIsArrayCompatible(option);

                        var items = ArgumentParser.GetNextInputValues(argumentEnumerator);

                        valueSetting = option.SetValue(items, options);
                        if (!valueSetting)
                        {
                            DefineOptionThatViolatesFormat(option);
                        }
                        return ArgumentParser.BooleanToParserState(valueSetting);
                    }
                }

                if (!@group.IsLast && map[@group.Next] == null)
                {
                    return PresentParserState.Failure;
                }
                if (!option.SetValue(true, options))
                {
                    return PresentParserState.Failure;
                }
            }

            return PresentParserState.Success;
        }

        private readonly bool _ignoreUnkwnownArguments;
    }
}