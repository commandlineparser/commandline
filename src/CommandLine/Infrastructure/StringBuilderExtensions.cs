// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Text;

namespace CommandLine.Infrastructure
{
    static class StringBuilderExtensions
    {
        public static StringBuilder AppendWhen(this StringBuilder builder, bool condition, params string[] values)
        {
            if (condition)
                foreach (var value in values)
                    builder.Append(value);

            return builder;
        }

        public static StringBuilder AppendWhen(this StringBuilder builder, bool condition, params char[] values)
        {
            if (condition)
                foreach (var value in values)
                    builder.Append(value);

            return builder;
        }

        public static StringBuilder AppendFormatWhen(this StringBuilder builder, bool condition, string format, params object[] args)
        {
            return condition
                ? builder.AppendFormat(format, args)
                : builder;
        }

        public static StringBuilder AppendIf(this StringBuilder builder, bool condition, string ifTrue, string ifFalse)
        {
            return condition
                ? builder.Append(ifTrue)
                : builder.Append(ifFalse);
        }

        public static StringBuilder BimapIf(this StringBuilder builder, bool condition,
            Func<StringBuilder, StringBuilder> ifTrue, Func<StringBuilder, StringBuilder> ifFalse)
        {
            return condition
                ? ifTrue(builder)
                : ifFalse(builder);
        }

        public static StringBuilder MapIf(this StringBuilder builder, bool condition,
            Func<StringBuilder, StringBuilder> ifTrue)
        {
            return condition
                ? ifTrue(builder)
                : builder;
        }

        public static StringBuilder AppendIfNotEmpty(this StringBuilder builder, params string[] values)
        {
            foreach (var value in values)
                if (value.Length > 0)
                    builder.Append(value);

            return builder;
        }

        public static string SafeToString(this StringBuilder builder)
        {
            return builder == null
                ? string.Empty
                : builder.ToString();
        }

        public static int SafeLength(this StringBuilder builder)
        {
            return builder == null ? 0 : builder.Length;
        }

        public static StringBuilder TrimEnd(this StringBuilder builder, char c)
        {
            return builder.Length > 0
                ? builder.Remove(builder.Length - 1, 1)
                : builder;
        }

        public static StringBuilder TrimEndIfMatch(this StringBuilder builder, char c)
        {
            if (builder.Length > 0)
                if (builder[builder.Length - 1] == c)
                    builder.Remove(builder.Length - 1, 1);

            return builder;
        }

        public static StringBuilder TrimEndIfMatchWhen(this StringBuilder builder, bool condition, char c)
        {
            return condition
                ? builder.TrimEndIfMatch(c)
                : builder;
        }

        public static int TrailingSpaces(this StringBuilder builder)
        {
            var bound = builder.Length - 1;
            if (builder.Length == 0) return 0;
            if (builder[bound] != ' ') return 0;
            var c = 0;
            for (var i = bound; i <= bound; i--)
            {
                if (i < 0) break;
                if (builder[i] != ' ') break;
                c++;
            }
            return c;
        }
    }
}