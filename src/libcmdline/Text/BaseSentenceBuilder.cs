#region License
// <copyright file="BaseSentenceBuilder.cs" company="Giacomo Stelluti Scala">
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

namespace CommandLine.Text
{
    /// <summary>
    /// Models an abstract sentence builder.
    /// </summary>
    public abstract class BaseSentenceBuilder
    {
        /// <summary>
        /// Gets a string containing word 'option'.
        /// </summary>
        /// <value>The word 'option'.</value>
        public abstract string OptionWord { get; }

        /// <summary>
        /// Gets a string containing the word 'and'.
        /// </summary>
        /// <value>The word 'and'.</value>
        public abstract string AndWord { get; }

        /// <summary>
        /// Gets a string containing the sentence 'required option missing'.
        /// </summary>
        /// <value>The sentence 'required option missing'.</value>
        public abstract string RequiredOptionMissingText { get; }

        /// <summary>
        /// Gets a string containing the sentence 'violates format'.
        /// </summary>
        /// <value>The sentence 'violates format'.</value>
        public abstract string ViolatesFormatText { get; }

        /// <summary>
        /// Gets a string containing the sentence 'violates mutual exclusiveness'.
        /// </summary>
        /// <value>The sentence 'violates mutual exclusiveness'.</value>
        public abstract string ViolatesMutualExclusivenessText { get; }

        /// <summary>
        /// Gets a string containing the error heading text.
        /// </summary>
        /// <value>The error heading text.</value>
        public abstract string ErrorsHeadingText { get; }

        /// <summary>
        /// Creates the built in sentence builder.
        /// </summary>
        /// <returns>The built in sentence builder.</returns>
        public static BaseSentenceBuilder CreateBuiltIn()
        {
            return new EnglishSentenceBuilder();
        }
    }
}
