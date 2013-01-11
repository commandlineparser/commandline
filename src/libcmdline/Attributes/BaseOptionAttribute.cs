#region License
//
// Command Line Library: BaseOptionAttribute.cs
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
using System;
using System.Resources;
using CommandLine.Internal;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Provides base properties for creating an attribute, used to define rules for command line parsing.
    /// </summary>
    public abstract class BaseOptionAttribute : Attribute
    {
        /// <summary>
        /// Short name of this command line option. You can use only one character.
        /// </summary>
        public virtual char? ShortName
        {
            get { return _shortName; }
            internal set
            {
                //if (value != null && value.Length > 1)
                //    throw new ArgumentException("ShortName length must be 1 character or null.");
                if (value != null && (StringUtil.IsWhiteSpace(value.Value) || StringUtil.IsLineTerminator(value.Value)))
                {
                    throw new ArgumentException("ShortName with whitespace or line terminator character is not allowed.");
                }
                _shortName = value;
            }
        }

        /// <summary>
        /// Long name of this command line option. This name is usually a single english word.
        /// </summary>
        public string LongName { get; internal set; }

        /// <summary>
        /// True if this command line option is required.
        /// </summary>
        public virtual bool Required { get; set; }

        /// <summary>
        /// Gets or sets mapped property default value.
        /// </summary>
        public virtual object DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                _defaultValue = value;
                _hasDefaultValue = true;
            }
        }

        /// <summary>
        /// A short description of this command line option. Usually a sentence summary. 
        /// </summary>
        public string HelpText
        {
            get { return _helpText;}
            set
            {
                if (_helpTextKey != null)
                {
                    throw new InvalidOperationException(
                        "You are not allowed to set both HelpText and HelpTextKey.");
                }
                _helpText = value;
            }
        }

        internal bool HasShortName
        {
            get { return _shortName != null; }
        }

        internal bool HasLongName
        {
            get { return !string.IsNullOrEmpty(LongName); }
        }

        internal bool HasDefaultValue
        {
            get { return _hasDefaultValue; }
        }

        /// <summary>
        /// This is the name of the string resource in the ResourceManager that should be used as HelpText.
        /// </summary>
        public string HelpTextKey
        {
            get { return _helpTextKey; }
            set
            {
                if (ResourceManager == null)
                {
                    throw new InvalidOperationException(
                        "You need to assign a ResourceManager to set HelpTextKey.");
                }
                if (_helpText != null)
                {
                    throw new InvalidOperationException(
                        "You are not allowed to set both HelpText and HelpTextKey.");
                }
                _helpTextKey = value;
                _helpText = ResourceManager.GetString(value);
            }
        }

        /// <summary>
        /// The resource manager used to retrieve the resourced strings.
        /// </summary>
        public static ResourceManager ResourceManager;

        private char? _shortName;
        private object _defaultValue;
        private bool _hasDefaultValue;
        private string _helpText;
        private string _helpTextKey;
    }
}