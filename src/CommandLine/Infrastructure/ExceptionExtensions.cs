// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Infrastructure
{
    static class ExceptionExtensions
    {
        public static void RethrowWhenAbsentIn(this Exception exception, IEnumerable<Exception> validExceptions)
        {
            if (!validExceptions.Contains(exception))
            {
                throw exception;
            }
        }
    }
}
