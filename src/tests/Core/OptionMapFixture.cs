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
using NUnit.Framework;
using Should.Fluent;
using CommandLine;
using CommandLine.Internal;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public sealed class OptionMapFixture
    {
        #region Helper Nested Class
        class OptionMapBuilder
        {
            private readonly OptionMap _optionMap;
            private readonly List<OptionInfo> _options;
            private readonly List<string> _names;

            public OptionMapBuilder(int capacity)
            {
                _optionMap = new OptionMap(capacity, new CommandLineParserSettings(true));
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
        private static OptionMap _optionMap;
        private static OptionMapBuilder _omBuilder;

        [SetUp]
        public void CreateInstance()
        {
            _omBuilder = new OptionMapBuilder(3);
            _omBuilder.AppendOption('p', "pretend");
            _omBuilder.AppendOption("newuse");
            _omBuilder.AppendOption('D', null);

            _optionMap = _omBuilder.OptionMap;
        }

        [TearDown]
        public void ShutdownInstance()
        {
            _optionMap = null;
        }

        [Test]
        public void ManageOptions()
        {
            _omBuilder.Options[0].Should().Be.SameAs(_optionMap[_omBuilder.Names[0]]);
            _omBuilder.Options[1].Should().Be.SameAs(_optionMap[_omBuilder.Names[1]]);
            _omBuilder.Options[2].Should().Be.SameAs(_optionMap[_omBuilder.Names[2]]);
        }

        [Test]
        public void RetrieveNotExistentShortOption()
        {
            var shortOi = _optionMap["y"];
            shortOi.Should().Be.Null();
        }

        [Test]
        public void RetrieveNotExistentLongOption()
        {
            var longOi = _optionMap["nomorebugshere"];
            longOi.Should().Be.Null();
        }

        private static OptionMap CreateMap (ref OptionMap map, IDictionary<string, OptionInfo> optionCache)
        {
            if (map == null)
            {
                map = new OptionMap (3, new CommandLineParserSettings (true));
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