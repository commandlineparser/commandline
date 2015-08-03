// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLine.Tests
{
    struct CultureHandlers
    {
        public Action Changer;
        public Action Resetter;
    }

    static class CultureHelper
    {
        public static CultureHandlers MakeCultureHandlers(CultureInfo newCulture)
        {
            var currentCulutre = Thread.CurrentThread.CurrentCulture;

            Action changer = () => Thread.CurrentThread.CurrentCulture = newCulture;

            Action resetter = () => Thread.CurrentThread.CurrentCulture = currentCulutre;

            return new CultureHandlers { Changer = changer, Resetter = resetter };
        }
    }
}
