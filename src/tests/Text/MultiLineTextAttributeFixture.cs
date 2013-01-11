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
using NUnit.Framework;
using Should.Fluent;
#endregion

namespace CommandLine.Text.Tests
{
	[TestFixture]
	public sealed class MultiLineTextAttributeFixture
	{
		[Test]
		public void AssemblyLicenseShouldOfferReadOnlyPropertyNamedValue()
		{
			IEnumerable<AssemblyLicenseAttribute> licenseAttributes = this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyLicenseAttribute), false) as AssemblyLicenseAttribute[];

			licenseAttributes.Count().Should().Equal(1);

			string license = licenseAttributes.Single().Value;
			string[] lines = license.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			lines[0].Should().Equal(@"This is free software. You may redistribute copies of it under the terms of");
			lines[1].Should().Equal(@"the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
		}

		[Test]
		public void AssemblyUsageShouldOfferReadOnlyPropertyNamedValue()
		{
			IEnumerable<AssemblyUsageAttribute> usageAttributes = this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyUsageAttribute), false) as AssemblyUsageAttribute[];

			usageAttributes.Count().Should().Equal(1);
			usageAttributes.Single().Value.Should().Equal(@"[no usage, this is a dll]" + Environment.NewLine);
		}
	}
}
