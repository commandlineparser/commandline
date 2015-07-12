// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Text;

namespace CommandLine.Text
{
    static class StringBuilderExtensions
    {        public static StringBuilder AppendWhen(this StringBuilder builder, bool condition, params string[] values)        {
            if (condition)            {                foreach (var value in values)                {                    builder.Append(value);                }            }            return builder;        }

        public static StringBuilder AppendWhen(this StringBuilder builder, bool condition, params char[] values)
        {
            if (condition)
            {
                foreach (var value in values)
                {
                    builder.Append(value);
                }
            }
            return builder;
        }
        public static string SafeToString(this StringBuilder builder)        {            return builder == null ? string.Empty : builder.ToString();        }

        public static int SafeLength(this StringBuilder builder)
        {
            return builder == null ? 0 : builder.Length;
        }    }
}
