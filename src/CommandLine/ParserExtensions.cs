// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace CommandLine
{
    /// <summary>
    /// Defines generic overloads for <see cref="CommandLine.Parser.ParseArguments(IEnumerable&lt;string&gt;,Type[])"/>.
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <typeparam name="T7">The type of the seventh verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6, T7>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <typeparam name="T7">The type of the seventh verb.</typeparam>
        /// <typeparam name="T8">The type of the eighth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6, T7, T8>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <typeparam name="T7">The type of the seventh verb.</typeparam>
        /// <typeparam name="T8">The type of the eighth verb.</typeparam>
        /// <typeparam name="T9">The type of the ninth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <typeparam name="T7">The type of the seventh verb.</typeparam>
        /// <typeparam name="T8">The type of the eighth verb.</typeparam>
        /// <typeparam name="T9">The type of the ninth verb.</typeparam>
        /// <typeparam name="T10">The type of the tenth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <typeparam name="T7">The type of the seventh verb.</typeparam>
        /// <typeparam name="T8">The type of the eighth verb.</typeparam>
        /// <typeparam name="T9">The type of the ninth verb.</typeparam>
        /// <typeparam name="T10">The type of the tenth verb.</typeparam>
        /// <typeparam name="T11">The type of the eleventh verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <typeparam name="T7">The type of the seventh verb.</typeparam>
        /// <typeparam name="T8">The type of the eighth verb.</typeparam>
        /// <typeparam name="T9">The type of the ninth verb.</typeparam>
        /// <typeparam name="T10">The type of the tenth verb.</typeparam>
        /// <typeparam name="T11">The type of the eleventh verb.</typeparam>
        /// <typeparam name="T12">The type of the twelfth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11), typeof(T12) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <typeparam name="T7">The type of the seventh verb.</typeparam>
        /// <typeparam name="T8">The type of the eighth verb.</typeparam>
        /// <typeparam name="T9">The type of the ninth verb.</typeparam>
        /// <typeparam name="T10">The type of the tenth verb.</typeparam>
        /// <typeparam name="T11">The type of the eleventh verb.</typeparam>
        /// <typeparam name="T12">The type of the twelfth verb.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <typeparam name="T7">The type of the seventh verb.</typeparam>
        /// <typeparam name="T8">The type of the eighth verb.</typeparam>
        /// <typeparam name="T9">The type of the ninth verb.</typeparam>
        /// <typeparam name="T10">The type of the tenth verb.</typeparam>
        /// <typeparam name="T11">The type of the eleventh verb.</typeparam>
        /// <typeparam name="T12">The type of the twelfth verb.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth verb.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <typeparam name="T7">The type of the seventh verb.</typeparam>
        /// <typeparam name="T8">The type of the eighth verb.</typeparam>
        /// <typeparam name="T9">The type of the ninth verb.</typeparam>
        /// <typeparam name="T10">The type of the tenth verb.</typeparam>
        /// <typeparam name="T11">The type of the eleventh verb.</typeparam>
        /// <typeparam name="T12">The type of the twelfth verb.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth verb.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth verb.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15) });
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from types as generic arguments.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <typeparam name="T1">The type of the first verb.</typeparam>
        /// <typeparam name="T2">The type of the second verb.</typeparam>
        /// <typeparam name="T3">The type of the third verb.</typeparam>
        /// <typeparam name="T4">The type of the fourth verb.</typeparam>
        /// <typeparam name="T5">The type of the fifth verb.</typeparam>
        /// <typeparam name="T6">The type of the sixth verb.</typeparam>
        /// <typeparam name="T7">The type of the seventh verb.</typeparam>
        /// <typeparam name="T8">The type of the eighth verb.</typeparam>
        /// <typeparam name="T9">The type of the ninth verb.</typeparam>
        /// <typeparam name="T10">The type of the tenth verb.</typeparam>
        /// <typeparam name="T11">The type of the eleventh verb.</typeparam>
        /// <typeparam name="T12">The type of the twelfth verb.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth verb.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth verb.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth verb.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth verb.</typeparam>
        /// <param name="parser">A <see cref="CommandLine.Parser"/> instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <remarks>All types must expose a parameterless constructor.</remarks>
        public static ParserResult<object> ParseArguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.ParseArguments(args, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16) });
        }
    }
}