// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine.Core;
using CommandLine.Text;
using CSharpx;
using RailwaySharp.ErrorHandling;

namespace CommandLine
{
    /// <summary>
    /// Provides methods to parse command line arguments.
    /// </summary>
    public class Parser : IDisposable
    {
        private bool disposed;
        private readonly ParserSettings settings;
        private static readonly Lazy<Parser> DefaultParser = new Lazy<Parser>(
            () => new Parser(new ParserSettings() ));

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Parser"/> class.
        /// </summary>
        [Obsolete("Calling the constructor directly is deprecated - prefer Parser.Default")]
        public Parser()
        {
            settings = new ParserSettings { Consumed = true };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class,
        /// configurable with <see cref="ParserSettings"/> using a delegate.
        /// </summary>
        /// <param name="configuration">The <see cref="Action&lt;ParserSettings&gt;"/> delegate used to configure
        /// aspects and behaviors of the parser.</param>
        [Obsolete("Calling the constructor directly is deprecated - prefer Parser.Default.Set....")]
        public Parser(Action<ParserSettings> configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            settings = new ParserSettings();
            configuration(settings);
            settings.Consumed = true;
        }

        internal Parser(ParserSettings settings)
        {
            this.settings = settings;
            this.settings.Consumed = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CommandLine.Parser"/> class.
        /// </summary>
        ~Parser()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the singleton instance created with basic defaults.
        /// </summary>
        public static Parser Default
        {
            get { return DefaultParser.Value; }
        }

        /// <summary>
        /// Gets the instance that implements <see cref="CommandLine.ParserSettings"/> in use.
        /// </summary>
        public ParserSettings Settings
        {
            get { return settings; }
        }

        /// <summary>
        /// Parses a string array of command line arguments constructing values in an instance of type <typeparamref name="T"/>.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// </summary>
        /// <typeparam name="T">Type of the target instance built with parsed value.</typeparam>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing an instance of type <typeparamref name="T"/> with parsed values
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        public ParserResult<T> ParseArguments<T>(IEnumerable<string> args)
        {
            if (args == null) throw new ArgumentNullException("args");

            var factory = typeof(T).IsMutable()
                ? Maybe.Just<Func<T>>(Activator.CreateInstance<T>)
                : Maybe.Nothing<Func<T>>();

            return MakeParserResult(
                InstanceBuilder.Build(
                    factory,
                    (arguments, optionSpecs) => Tokenize(arguments, optionSpecs, settings),
                    args,
                    settings.NameComparer,
                    settings.CaseInsensitiveEnumValues,
                    settings.ParsingCulture,
                    settings.AutoHelp,
                    settings.AutoVersion,
                    HandleUnknownArguments(settings.IgnoreUnknownArguments)),
                settings);
        }

        /// <summary>
        /// Parses a string array of command line arguments constructing values in an instance of type <typeparamref name="T"/>.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// </summary>
        /// <typeparam name="T">Type of the target instance built with parsed value.</typeparam>
        /// <param name="factory">A <see cref="System.Func{T}"/> delegate used to initialize the target instance.</param>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing an instance of type <typeparamref name="T"/> with parsed values
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        public ParserResult<T> ParseArguments<T>(Func<T> factory, IEnumerable<string> args)
            where T : new()
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (!typeof(T).IsMutable()) throw new ArgumentException("factory");
            if (args == null) throw new ArgumentNullException("args");

            return MakeParserResult(
                InstanceBuilder.Build(
                    Maybe.Just(factory),
                    (arguments, optionSpecs) => Tokenize(arguments, optionSpecs, settings),
                    args,
                    settings.NameComparer,
                    settings.CaseInsensitiveEnumValues,
                    settings.ParsingCulture,
                    settings.AutoHelp,
                    settings.AutoVersion,
                    HandleUnknownArguments(settings.IgnoreUnknownArguments)),
                settings);
        }

        /// <summary>
        /// Parses a string array of command line arguments for verb commands scenario, constructing the proper instance from the array of types supplied by <paramref name="types"/>.
        /// Grammar rules are defined decorating public properties with appropriate attributes.
        /// The <see cref="CommandLine.VerbAttribute"/> must be applied to types in the array.
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments, normally supplied by application entry point.</param>
        /// <param name="types">A <see cref="System.Type"/> array used to supply verb alternatives.</param>
        /// <returns>A <see cref="CommandLine.ParserResult{T}"/> containing the appropriate instance with parsed values as a <see cref="System.Object"/>
        /// and a sequence of <see cref="CommandLine.Error"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if one or more arguments are null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="types"/> array is empty.</exception>
        /// <remarks>All types must expose a parameterless constructor. It's strongly recommended to use a generic overload.</remarks>
        public ParserResult<object> ParseArguments(IEnumerable<string> args, params Type[] types)
        {
            if (args == null) throw new ArgumentNullException("args");
            if (types == null) throw new ArgumentNullException("types");
            if (types.Length == 0) throw new ArgumentOutOfRangeException("types");

            return MakeParserResult(
                InstanceChooser.Choose(
                    (arguments, optionSpecs) => Tokenize(arguments, optionSpecs, settings),
                    types,
                    args,
                    settings.NameComparer,
                    settings.CaseInsensitiveEnumValues,
                    settings.ParsingCulture,
                    settings.AutoHelp,
                    settings.AutoVersion,
                    HandleUnknownArguments(settings.IgnoreUnknownArguments)),
                settings);
        }

        /// <summary>
        /// Frees resources owned by the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private static Result<IEnumerable<Token>, Error> Tokenize(
                IEnumerable<string> arguments,
                IEnumerable<OptionSpecification> optionSpecs,
                ParserSettings settings)
        {
            return
                Tokenizer.ConfigureTokenizer(
                    settings.NameComparer,
                    settings.IgnoreUnknownArguments,
                    settings.EnableDashDash)(arguments, optionSpecs);
        }

        private static ParserResult<T> MakeParserResult<T>(ParserResult<T> parserResult, ParserSettings settings)
        {
            return DisplayHelp(
                parserResult,
                settings.HelpTextConfiguration);
        }

        private static ParserResult<T> DisplayHelp<T>(ParserResult<T> parserResult, HelpTextConfiguration helpTextConfig)
        {
            parserResult.WithNotParsed(
                errors =>
                    Maybe.Merge(errors.ToMaybe(), helpTextConfig.HelpWriter.ToMaybe())
                        .Do((_, writer) => writer.Write(HelpText.AutoBuild(parserResult,helpTextConfig)))
                );

            return parserResult;
        }

        private static IEnumerable<ErrorType> HandleUnknownArguments(bool ignoreUnknownArguments)
        {
            return ignoreUnknownArguments
                ? Enumerable.Empty<ErrorType>().Concat(ErrorType.UnknownOptionError)
                : Enumerable.Empty<ErrorType>();
        }

        private void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (settings != null)
                    settings.Dispose();

                disposed = true;
            }
        }
        /// <summary>
        /// Allows the client to use a stream other than Console.Error for ouput.  NOTE - client should dispose the stream
        /// </summary>
        /// <param name="writer">The output stream to use </param>
        /// <returns>The parser</returns>
        public Parser SetTextWriter(TextWriter writer)
        {
            settings.Consumed = false;
            settings.HelpTextConfiguration = settings.HelpTextConfiguration.WithHelpWriter(writer);
            settings.Consumed = true;
            return this;
        }

        /// <summary>
        /// Allows the client to select the display width
        /// </summary>
        /// <param name="width">The desired width</param>
        /// <param name="policy">The policy to use when setting the width</param>
        /// <returns>The parser</returns>
        public Parser SetDisplayWidth(int width,WidthPolicy policy)
        {
            settings.Consumed = false;
            //Note that the parser constructor has probably already worked out the console width
            if (policy == WidthPolicy.FitToScreen)
                width = Math.Min(settings.HelpTextConfiguration.DisplayWidth, width);
            settings.HelpTextConfiguration = settings.HelpTextConfiguration.WithDisplayWidth(width);
            settings.Consumed = true;
            return this;
        }

        /// <summary>
        /// Allows the client to configure flags in the helpText class
        /// </summary>
        /// <param name="configurer">An action which can set flags in the HelpText class</param>
        /// <returns>The parser</returns>
        public Parser SetHelpTextConfiguration(Action<HelpText> configurer)
        {
            settings.Consumed = false;
            settings.HelpTextConfiguration = settings.HelpTextConfiguration.WithConfigurer(configurer);
            settings.Consumed = true;
            return this;
        }
       
        
        /// <summary>
        /// Allows the client to turn the automatic help verb on and off 
        /// </summary>
        /// <param name="enable">turn on/off</param>
        /// <returns>The parser</returns>
        public Parser SetAutoHelp(bool enable)
        {
            settings.Consumed = false;
            settings.HelpTextConfiguration = settings.HelpTextConfiguration.WithAutoHelp(enable);
            settings.Consumed = true;
            return this;
        }

        /// <summary>
        /// Allows the client to turn the automatic version reporting on and off
        /// </summary>
        /// <param name="enable">turn on/off</param>
        /// <returns>The parser</returns>
        public Parser SetAutoVersion(bool enable)
        {
            settings.Consumed = false;
            settings.HelpTextConfiguration = settings.HelpTextConfiguration.WithAutoVersion(enable);
            settings.Consumed = true;
            return this;
        }

    }
}
