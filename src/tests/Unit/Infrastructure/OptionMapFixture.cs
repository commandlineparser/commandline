#region License
//
// Command Line Library: OptionMapFixture.cs
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
using System.Collections.Generic;

using CommandLine.Parsing;

using Xunit;
using FluentAssertions;
using CommandLine;
using CommandLine.Infrastructure;
#endregion

namespace CommandLine.Tests.Unit.Infrastructure
{
    public class OptionMapFixture
    {
        #region Helper Nested Class
        class OptionMapBuilder
        {
            private readonly OptionMap _optionMap;
            private readonly List<OptionInfo> _options;
            private readonly List<string> _names;

            public OptionMapBuilder(int capacity)
            {
                _optionMap = new OptionMap(capacity, new ParserSettings(true));
                _options = new List<OptionInfo>(capacity);
                _names = new List<string>(capacity);
            }

            public void AppendOption(string longName)
            {
                var oa = new OptionAttribute(longName);
                var oi = oa.CreateOptionInfo();
                _optionMap[oa.UniqueName] = oi;
                _options.Add(oi);
                _names.Add(oa.UniqueName);
            }

            public void AppendOption(char shortName, string longName)
            {
                var oa = new OptionAttribute(shortName, longName);
                var oi = oa.CreateOptionInfo();
                _optionMap[oa.UniqueName] = oi;
                _options.Add(oi);
                _names.Add(oa.UniqueName);
            }

            public List<OptionInfo> Options
            {
                get { return _options; }
            }

            public List<string> Names
            {
                get { return _names; }
            }

            public OptionMap OptionMap
            {
                get { return _optionMap; }
            }
        }
        #endregion

        [Fact]
        public void Manage_options()
        {
            var omBuilder = new OptionMapBuilder(3);
            omBuilder.AppendOption('p', "pretend");
            omBuilder.AppendOption("newuse");
            omBuilder.AppendOption('D', null);

            var optionMap = omBuilder.OptionMap;

            omBuilder.Options[0].Should().BeSameAs(optionMap[omBuilder.Names[0]]);
            omBuilder.Options[1].Should().BeSameAs(optionMap[omBuilder.Names[1]]);
            omBuilder.Options[2].Should().BeSameAs(optionMap[omBuilder.Names[2]]);
        }

        //[Fact]
        //public void RetrieveNotExistentShortOption()
        //{
        //    var shortOi = _optionMap["y"];
        //    shortOi.Should().BeNull();
        //}

        //[Fact]
        //public void RetrieveNotExistentLongOption()
        //{
        //    var longOi = _optionMap["nomorebugshere"];
        //    longOi.Should().BeNull();
        //}

        private static OptionMap CreateMap (ref OptionMap map, IDictionary<string, OptionInfo> optionCache)
        {
            if (map == null)
            {
                map = new OptionMap (3, new ParserSettings (true));
            }

            var attribute1 = new OptionAttribute('p', "pretend");
            var attribute2 = new OptionAttribute("newuse");
            var attribute3 = new OptionAttribute('D', null);

            var option1 = attribute1.CreateOptionInfo();
            var option2 = attribute2.CreateOptionInfo();
            var option3 = attribute3.CreateOptionInfo();

            map[attribute1.UniqueName] = option1;
            map[attribute2.UniqueName] = option2;
            map[attribute3.UniqueName] = option3;

            if (optionCache != null)
            {
                optionCache[attribute1.UniqueName] = option1;
                optionCache[attribute1.UniqueName] = option2;
                optionCache[attribute2.UniqueName]= option3;
            }

            return map;
        }
    }
}

