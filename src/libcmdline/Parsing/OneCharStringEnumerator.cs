#region License
// <copyright file="OneCharStringEnumerator.cs" company="Giacomo Stelluti Scala">
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
using System;

using CommandLine.Infrastructure;

#endregion

namespace CommandLine.Parsing
{
    internal sealed class OneCharStringEnumerator : IArgumentEnumerator
    {
        private readonly string _data;
        private string _currentElement;
        private int _index;

        public OneCharStringEnumerator(string value)
        {
            Assumes.NotNullOrEmpty(value, "value");
            _data = value;
            _index = -1;
        }

        public string Current
        {
            get
            {
                if (_index == -1)
                {
                    throw new InvalidOperationException();
                }

                if (_index >= _data.Length)
                {
                    throw new InvalidOperationException();
                }

                return _currentElement;
            }
        }

        public string Next
        {
            get
            {
                if (_index == -1)
                {
                    throw new InvalidOperationException();
                }

                if (_index > _data.Length)
                {
                    throw new InvalidOperationException();
                }

                if (IsLast)
                {
                    return null;
                }

                return _data.Substring(_index + 1, 1);
            }
        }

        public bool IsLast
        {
            get { return _index == _data.Length - 1; }
        }

        public bool MoveNext()
        {
            if (_index < (_data.Length - 1))
            {
                _index++;
                _currentElement = _data.Substring(_index, 1);
                return true;
            }

            _index = _data.Length;
            return false;
        }

        public string GetRemainingFromNext()
        {
            if (_index == -1)
            {
                throw new InvalidOperationException();
            }

            if (_index > _data.Length)
            {
                throw new InvalidOperationException();
            }

            return _data.Substring(_index + 1);
        }

        public bool MovePrevious()
        {
            throw new NotSupportedException();
        }
    }
}