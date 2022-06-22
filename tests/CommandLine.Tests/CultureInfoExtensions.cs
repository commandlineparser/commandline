// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Globalization;
using System.Threading;

namespace CommandLine.Tests
{
    struct CultureHandlers
    {
        public Action ChangeCulture;
        public Action ResetCulture;
    }

    static class CultureInfoExtensions
    {
        public static CultureHandlers MakeCultureHandlers(this CultureInfo newCulture)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;

            Action changer = () => Thread.CurrentThread.CurrentCulture = newCulture;

            Action resetter = () => Thread.CurrentThread.CurrentCulture = currentCulture;

            return new CultureHandlers { ChangeCulture = changer, ResetCulture = resetter };
        }
    }
}
