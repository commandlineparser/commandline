// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using System.Text;
using CommandLine.Infrastructure;
using CSharpx;

namespace CommandLine.Text
{
    /// <summary>
    /// Models the heading part of an help text.
    /// You can assign it where you assign any <see cref="System.String"/> instance.
    /// </summary>
    public class HeadingInfo
    {
        private readonly string programName;
        private readonly string version;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HeadingInfo"/> class
        /// specifying program name and version.
        /// </summary>
        /// <param name="programName">The name of the program.</param>
        /// <param name="version">The version of the program.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="programName"/> is null or empty string.</exception>
        public HeadingInfo(string programName, string version = null)
        {
            if (string.IsNullOrWhiteSpace("programName")) throw new ArgumentException("programName");

            this.programName = programName;
            this.version = version;
        }

        /// <summary>
        /// An empty object used for initialization. 
        /// </summary>
        public static HeadingInfo Empty
        {
            get
            {
                return new HeadingInfo("");
            }
        }

        /// <summary>
        /// Gets the default heading instance.
        /// The title is retrieved from <see cref="AssemblyTitleAttribute"/>,
        /// or the assembly short name if its not defined.
        /// The version is retrieved from <see cref="AssemblyInformationalVersionAttribute"/>,
        /// or the assembly version if its not defined.
        /// </summary>
        public static HeadingInfo Default
        {
            get
            {
                var title = ReflectionHelper.GetAttribute<AssemblyTitleAttribute>()
                    .MapValueOrDefault(
                        titleAttribute => Path.GetFileNameWithoutExtension(titleAttribute.Title),
                        ReflectionHelper.GetAssemblyName());
                var version = ReflectionHelper.GetAttribute<AssemblyInformationalVersionAttribute>()
                    .MapValueOrDefault(
                        versionAttribute => versionAttribute.InformationalVersion,
                        ReflectionHelper.GetAssemblyVersion());
                return new HeadingInfo(title, version);
            }
        }

        /// <summary>
        /// Converts the heading to a <see cref="System.String"/>.
        /// </summary>
        /// <param name="info">This <see cref="CommandLine.Text.HeadingInfo"/> instance.</param>
        /// <returns>The <see cref="System.String"/> that contains the heading.</returns>
        public static implicit operator string(HeadingInfo info)
        {
            return info.ToString();
        }

        /// <summary>
        /// Returns the heading as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>The <see cref="System.String"/> that contains the heading.</returns>
        public override string ToString()
        {
            var isVersionNull = string.IsNullOrEmpty(version);
            return new StringBuilder(programName.Length +
                    (!isVersionNull ? version.Length + 1 : 0))
                .Append(programName)
                .AppendWhen(!isVersionNull, " ", version)
                .ToString();
        }

        /// <summary>
        /// Writes out a string and a new line using the program name specified in the constructor
        /// and <paramref name="message"/> parameter.
        /// </summary>
        /// <param name="message">The <see cref="System.String"/> message to write.</param>
        /// <param name="writer">The target <see cref="System.IO.TextWriter"/> derived type.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="message"/> is null or empty string.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="writer"/> is null.</exception>
        public void WriteMessage(string message, TextWriter writer)
        {
            if (string.IsNullOrWhiteSpace("message")) throw new ArgumentException("message");
            if (writer == null) throw new ArgumentNullException("writer");

            writer.WriteLine(
                new StringBuilder(programName.Length + message.Length + 2)
                    .Append(programName)
                    .Append(": ")
                    .Append(message)
                    .ToString());
        }

        /// <summary>
        /// Writes out a string and a new line using the program name specified in the constructor
        /// and <paramref name="message"/> parameter to standard output stream.
        /// </summary>
        /// <param name="message">The <see cref="System.String"/> message to write.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="message"/> is null or empty string.</exception>
        public void WriteMessage(string message)
        {
            WriteMessage(message, Console.Out);
        }

        /// <summary>
        /// Writes out a string and a new line using the program name specified in the constructor
        /// and <paramref name="message"/> parameter to standard error stream.
        /// </summary>
        /// <param name="message">The <see cref="System.String"/> message to write.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="message"/> is null or empty string.</exception>
        public void WriteError(string message)
        {
            WriteMessage(message, Console.Error);
        }
    }
}
