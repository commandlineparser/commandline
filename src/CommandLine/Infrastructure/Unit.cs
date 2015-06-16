// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;

namespace CommandLine.Infrastructure
{
    /// <summary>
    /// From https://github.com/Reactive-Extensions/Rx.NET/blob/master/Rx.NET/Source/System.Reactive.Core/Reactive/Unit.cs.
    /// </summary>
    internal struct Unit : IEquatable<Unit>
    {
        private static readonly Unit @default = new Unit();

        public bool Equals(Unit other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "()";
        }

        public static bool operator ==(Unit first, Unit second)
        {
            return true;
        }

        public static bool operator !=(Unit first, Unit second)
        {
            return false;
        }

        public static Unit Default { get { return @default; } }
    }
}
