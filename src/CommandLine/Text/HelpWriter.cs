// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandLine.Text
{
    /// <summary>
    /// Provides a way to write <see cref="HelpText"/> to a <see cref="TextWriter"/> with the option to distinguish between user requested and parsing error triggered help.
    /// </summary>
    public class HelpWriter
    {
        private static readonly Lazy<HelpWriter> DefaultWriter = new Lazy<HelpWriter>(() => new HelpWriter(Console.Out, Console.Error));

        private readonly TextWriter writer;
        private readonly TextWriter errorWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpWriter"/> class that writes all <see cref="HelpText"/> to the same <see cref="TextWriter"/>.
        /// The user is responsible for disposing the provided <see cref="TextWriter"/> instance.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write all <see cref="HelpText"/> to.</param>
        public HelpWriter(TextWriter writer)
            : this(writer, writer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpWriter"/> class that writes user requested <see cref="HelpText"/> to the <see cref="TextWriter"/> provided through
        /// <paramref name="writer"/> and error triggered <see cref="HelpText"/> to the <see cref="TextWriter"/> provided through <paramref name="errorWriter"/>.
        /// The user is responsible for disposing the provided <see cref="TextWriter"/> instances.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to write user requested <see cref="HelpText"/> to.</param>
        /// <param name="errorWriter">The <see cref="TextWriter"/> to write error triggered <see cref="HelpText"/> to.</param>
        public HelpWriter(TextWriter writer, TextWriter errorWriter)
        {
            this.writer = writer;
            this.errorWriter = errorWriter;
        }

        /// <summary>
        /// Gets a default <see cref="HelpWriter"/> that writes user requested <see cref="HelpText"/> to <see cref="Console.Out"/>
        /// and error triggered <see cref="HelpText"/> to <see cref="Console.Error"/>.
        /// </summary>
        public static HelpWriter Default
        {
            get { return DefaultWriter.Value; }
        }

        /// <summary>
        /// Writes the provided <see cref="HelpText"/> to the <see cref="TextWriter"/>(s) this <see cref="HelpWriter"/> was initialized with.
        /// </summary>
        /// <param name="errors">The <see cref="Error"/> instances to use when determining whether or not the <paramref name="helpText"/> was user requested or error triggered.</param>
        /// <param name="helpText">The <see cref="HelpText"/> to write to the <see cref="TextWriter"/>(s).</param>
        public void WriteHelpText(IEnumerable<Error> errors, HelpText helpText)
        {
            if (writer == null && errorWriter == null || helpText == null)
            {
                return;
            }

            TextWriter targetWriter = null;

            bool isUserRequestedError(Error error)
            {
                return error != null
                    && error.Tag == ErrorType.VersionRequestedError
                    || error.Tag == ErrorType.HelpRequestedError
                    || error.Tag == ErrorType.HelpVerbRequestedError;
            }

            if (writer == errorWriter || errors != null && errors.Any(isUserRequestedError))
            {
                targetWriter = writer;
            }
            else
            {
                targetWriter = errorWriter;
            }

            targetWriter?.Write(helpText);
        }
    }
}
