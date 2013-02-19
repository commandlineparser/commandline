#region License
//
// Command Line Library: Pair.cs
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
#endregion

namespace CommandLine.Helpers
{
    internal sealed class Pair<TLeft, TRight>
        where TLeft : class
        where TRight : class
    {
        private readonly TLeft left;
        private readonly TRight right;

        public Pair(TLeft left, TRight right)
        {
            this.left = left;
            this.right = right;
        }

        public TLeft Left
        {
            get { return this.left; }
        }

        public TRight Right
        {
            get { return this.right; }
        }

        public override int GetHashCode()
        {
            int leftHash = this.left == null ? 0 : this.left.GetHashCode();
            int rightHash = this.right == null ? 0 : this.right.GetHashCode();

            return leftHash ^ rightHash;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Pair<TLeft, TRight>;

            if (other == null)
            {
                return false;
            }

            return Equals(this.left, other.left) && Equals(this.right, other.right);
        }
    }
}