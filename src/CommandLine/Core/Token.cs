// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine.Core
{
    enum TokenType { Name, Value }

    abstract class Token
    {
        private readonly TokenType tag;
        private readonly string text;

        protected Token(TokenType tag, string text)
        {
            this.tag = tag;
            this.text = text;
        }

        public static Token Name(string text)
        {
            return new Name(text);
        }

        public static Token Value(string text)
        {
            return new Value(text);
        }

        public static Token Value(string text, bool explicitlyAssigned)
        {
            return new Value(text, explicitlyAssigned);
        }

        public static Token ValueForced(string text)
        {
            return new Value(text, false, true, false);
        }

        public static Token ValueFromSeparator(string text)
        {
            return new Value(text, false, false, true);
        }

        public TokenType Tag
        {
            get { return tag; }
        }

        public string Text
        {
            get { return text; }
        }
    }

    class Name : Token, IEquatable<Name>
    {
        public Name(string text)
            : base(TokenType.Name, text)
        {
        }

        public override bool Equals(object obj)
        {
            var other = obj as Name;
            if (other != null)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new {Tag, Text}.GetHashCode();
        }

        public bool Equals(Name other)
        {
            if (other == null)
            {
                return false;
            }

            return Tag.Equals(other.Tag) && Text.Equals(other.Text);
        }
    }

    class Value : Token, IEquatable<Value>
    {
        private readonly bool explicitlyAssigned;
        private readonly bool forced;
        private readonly bool fromSeparator;

        public Value(string text)
            : this(text, false, false, false)
        {
        }

        public Value(string text, bool explicitlyAssigned)
            : this(text, explicitlyAssigned, false, false)
        {
        }

        public Value(string text, bool explicitlyAssigned, bool forced, bool fromSeparator)
            : base(TokenType.Value, text)
        {
            this.explicitlyAssigned = explicitlyAssigned;
            this.forced = forced;
            this.fromSeparator = fromSeparator;
        }

        /// <summary>
        /// Whether this value came from a long option with "=" separating the name from the value
        /// </summary>
        public bool ExplicitlyAssigned
        {
            get { return explicitlyAssigned; }
        }

        /// <summary>
        /// Whether this value came from a sequence specified with a separator (e.g., "--files a.txt,b.txt,c.txt")
        /// </summary>
        public bool FromSeparator
        {
            get { return fromSeparator; }
        }

        /// <summary>
        /// Whether this value came from args after the -- separator (when EnableDashDash = true)
        /// </summary>
        public bool Forced
        {
            get { return forced; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Value;
            if (other != null)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { Tag, Text }.GetHashCode();
        }

        public bool Equals(Value other)
        {
            if (other == null)
            {
                return false;
            }

            return Tag.Equals(other.Tag) && Text.Equals(other.Text) && this.Forced == other.Forced;
        }
    }

    static class TokenExtensions
    {
        public static bool IsName(this Token token)
        {
            return token.Tag == TokenType.Name;
        }

        public static bool IsValue(this Token token)
        {
            return token.Tag == TokenType.Value;
        }

        public static bool IsValueFromSeparator(this Token token)
        {
            return token.IsValue() && ((Value)token).FromSeparator;
        }

        public static bool IsValueForced(this Token token)
        {
            return token.IsValue() && ((Value)token).Forced;
        }
    }
}
