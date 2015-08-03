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
    static class CultureHelper
    {
        public static Tuple<Action, Action> MakeCultureHandlers(CultureInfo newCulture)
        {
            var currentCulutre = Thread.CurrentThread.CurrentCulture;

            Action changer = () => Thread.CurrentThread.CurrentCulture = newCulture;

            Action resetter = () => Thread.CurrentThread.CurrentCulture = currentCulutre;

            return Tuple.Create(changer, resetter);
        }
    }
}
