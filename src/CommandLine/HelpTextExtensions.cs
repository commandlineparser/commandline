// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine
{
    public static class HelpTextExtensions
    {
        /// <summary>
        ///  return true when errors contain HelpXXXError
        /// </summary>
        public static bool IsHelp(this IEnumerable<Error> errs)
        {
            if (errs.Any(x => x.Tag == ErrorType.HelpRequestedError ||
                            x.Tag == ErrorType.HelpVerbRequestedError))
                return true;
            //when  AutoHelp=false in parser, help is disabled  and Parser raise UnknownOptionError
            return errs.Any(x => (x is UnknownOptionError ee ? ee.Token : "") == "help");
        }

        /// <summary>
        ///  return true when errors contain VersionXXXError
        /// </summary>
        public static bool IsVersion(this IEnumerable<Error> errs)
        {
            if (errs.Any(x => x.Tag == ErrorType.VersionRequestedError))
                return true;
            //when  AutoVersion=false in parser, Version is disabled  and Parser raise UnknownOptionError
            return errs.Any(x => (x is UnknownOptionError ee ? ee.Token : "") == "version");
        }
		 /// <summary>
        ///  redirect errs to Console.Error, and to Console.Out for help/version error
        /// </summary>
		 public static TextWriter Output(this IEnumerable<Error> errs)
        {
           if (errs.IsHelp() || errs.IsVersion())
			   return Console.Out;
		   return Console.Error;
        }
    }
}
 

