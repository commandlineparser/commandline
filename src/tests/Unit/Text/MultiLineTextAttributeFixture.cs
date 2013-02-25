#region License
//
// Command Line Library: MultiLineTextAttributeFixture.cs
//
// Author:
//   Robert Kayman (rkayman@gmail.com) / https://github.com/rkayman
// 
// Copyright (C) 2013 Robert Kayman
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
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using FluentAssertions;
#endregion

namespace CommandLine.Tests.Unit.Text
{  
    public class MultiLineTextAttributeFixture
    {
        [Fact]
        public void Assembly_license_should_offer_read_only_property_named_value()
        {
            IEnumerable<AssemblyLicenseAttribute> licenseAttributes = this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyLicenseAttribute), false) as AssemblyLicenseAttribute[];

            licenseAttributes.Count().Should().Be(1);

            string license = licenseAttributes.Single().Value;
            string[] lines = license.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines[0].Should().Be(@"This is free software. You may redistribute copies of it under the terms of");
            lines[1].Should().Be(@"the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
        }

        [Fact]
        public void Assembly_usage_should_offer_read_only_property_named_value()
        {
            IEnumerable<AssemblyUsageAttribute> usageAttributes = this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyUsageAttribute), false) as AssemblyUsageAttribute[];

            usageAttributes.Count().Should().Be(1);
            usageAttributes.Single().Value.Should().Be(@"[no usage, this is a dll]" + Environment.NewLine);
        }
    }
}

