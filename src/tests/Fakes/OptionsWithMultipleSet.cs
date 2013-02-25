#region License
//
// Command Line Library: OptionsWithMultipleSet.cs
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

namespace CommandLine.Tests.Fakes
{
    class OptionsWithMultipleSet
    {
        public OptionsWithMultipleSet()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
            Hue = 0;
            Saturation = 0;
            Value = 0;
        }

        // rgb mutually exclusive set
        [Option('r', "red", MutuallyExclusiveSet = "rgb")]
        public byte Red { get; set; }

        [Option('g', "green", MutuallyExclusiveSet = "rgb")]
        public byte Green { get; set; }

        [Option('b', "blue", MutuallyExclusiveSet = "rgb")]
        public byte Blue { get; set; }

        // hsv mutually exclusive set
        [Option('h', "hue", MutuallyExclusiveSet = "hsv")]
        public short Hue { get; set; }

        [Option('s', "saturation", MutuallyExclusiveSet = "hsv")]
        public byte Saturation { get; set; }

        [Option('v', "value", MutuallyExclusiveSet = "hsv")]
        public byte Value { get; set; }
    }

}