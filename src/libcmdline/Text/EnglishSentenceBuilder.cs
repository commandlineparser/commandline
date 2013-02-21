#region License
// <copyright file="EnglishSentenceBuilder.cs" company="Giacomo Stelluti Scala">
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
    /// Models an english sentence builder, currently the default one.
    /// </summary>
    public class EnglishSentenceBuilder : BaseSentenceBuilder
    {
        /// <summary>
        /// Gets a string containing word 'option' in english.
        /// </summary>
        /// <value>The word 'option' in english.</value>
        public override string OptionWord
        {
            get { return "option"; }
        }

        /// <summary>
        /// Gets a string containing the word 'and' in english.
        /// </summary>
        /// <value>The word 'and' in english.</value>
        public override string AndWord
        {
            get { return "and"; }
        }

        /// <summary>
        /// Gets a string containing the sentence 'required option missing' in english.
        /// </summary>
        /// <value>The sentence 'required option missing' in english.</value>
        public override string RequiredOptionMissingText
        {
            get { return "required option is missing"; }
        }

        /// <summary>
        /// Gets a string containing the sentence 'violates format' in english.
        /// </summary>
        /// <value>The sentence 'violates format' in english.</value>
        public override string ViolatesFormatText
        {
            get { return "violates format"; }
        }

        /// <summary>
        /// Gets a string containing the sentence 'violates mutual exclusiveness' in english.
        /// </summary>
        /// <value>The sentence 'violates mutual exclusiveness' in english.</value>
        public override string ViolatesMutualExclusivenessText
        {
            get { return "violates mutual exclusiveness"; }
        }

        /// <summary>
        /// Gets a string containing the error heading text in english.
        /// </summary>
        /// <value>The error heading text in english.</value>
        public override string ErrorsHeadingText
        {
            get { return "ERROR(S):"; }
        }
    }
}
