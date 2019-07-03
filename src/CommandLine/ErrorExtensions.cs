// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Core;
using CommandLine.Infrastructure;

namespace CommandLine
{
    static class ErrorExtensions
    {
        public static ParserResult<T> ToParserResult<T>(this IEnumerable<Error> errors, T instance)
        {
            return errors.Any()
                ? (ParserResult<T>)new NotParsed<T>(instance.GetType().ToTypeInfo(), errors)
                : (ParserResult<T>)new Parsed<T>(instance);
        }

        public static IEnumerable<Error> OnlyMeaningfulOnes(this IEnumerable<Error> errors)
        {
            return errors
                .Where(e => !e.StopsProcessing)
                .Where(e => !(e.Tag == ErrorType.UnknownOptionError
                    && ((UnknownOptionError)e).Token.EqualsOrdinalIgnoreCase("help")));
        }
        /// <summary>
        ///  return true when errors contain HelpXXXError
        /// </summary>
        public static bool  IsHelp  (this IEnumerable<Error> errs)
        {
            if (errs.Any(x=>x.Tag == ErrorType.HelpRequestedError || 
                            x.Tag == ErrorType.HelpVerbRequestedError))
                return true;
            //when  AutoHelp=false in parser, help is disabled  and Parser raise UnknownOptionError
            if(  errs.Any(x=> (x is UnknownOptionError ee ? ee.Token:"") == "help"))
                return true;
            return false;
        }

        /// <summary>
        ///  return true when errors contain VersionXXXError
        /// </summary>
        public static bool  IsVersion  (this IEnumerable<Error> errs)
        {
            if (errs.Any(x=>x.Tag == ErrorType.VersionRequestedError ))
                return true;
            //when  AutoVersion=false in parser, Version is disabled  and Parser raise UnknownOptionError
            if(  errs.Any(x=> (x is UnknownOptionError ee ? ee.Token:"") == "version"))
                return true;
            return false;
        }
    }
}
