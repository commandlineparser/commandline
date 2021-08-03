// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine
{
    /// <summary>
    /// Defines how commandline options are being parsed.
    /// </summary>
    public enum OptionsParseMode
    {
        /// <summary>
        /// Options that start with a double dash must be defined using its full name. E.g. git rebase --interactive
        /// Options that start with a single dash are interpreted as list of short named options. E.g. git clean -xdf
        /// </summary>
        Default,
        
        /// <summary>
        /// Options that start with a single or double dash are interpreted as short or full named option.
        /// </summary>
        SingleOrDoubleDash,
        
        /// <summary>
        /// Options that start with a single dash are interpreted as short or full named option.
        /// Options that start with a double dash are considered an invalid input.
        /// </summary>
        SingleDashOnly
    }
}
