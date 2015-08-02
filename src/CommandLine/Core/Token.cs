// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine.Core
{
    internal enum TokenType { Name, Value }

    internal class Token : IEquatable<Token>
    {
        private readonly TokenType tag;
        private readonly string text;

        private Token(TokenType tag, string text)
        {
            this.tag = tag;
            this.text = text;
        }

        public static Token Name(string text)
        {
            return new Token(TokenType.Name, text);
        }

        public static Token Value(string text)
        {
            return new Token(TokenType.Value, text);
        }

        public TokenType Tag
        {
            get { return tag; }
        }

        public string Text
        {
            get { return text; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Token;
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

        public bool Equals(Token other)
        {
            if (other == null)
            {
                return false;
            }

            return Tag.Equals(other.Tag) && Text.Equals(other.Text);
        }
    }

    internal static class TokenExtensions
    {
        public static bool IsName(this Token token)
        {
            return token.Tag == TokenType.Name;
        }

        public static bool IsValue(this Token token)
        {
            return token.Tag == TokenType.Value;
        }
    }
}