#region License
// <copyright file="ReflectionCache.cs" company="Giacomo Stelluti Scala">
//   Copyright 2015-2013 Giacomo Stelluti Scala
// </copyright>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#endregion

namespace CommandLine.Infrastructure
{
    internal sealed class ReflectionCache
    {
        private static readonly ReflectionCache Singleton;
        private readonly IDictionary<Pair<Type, object>, WeakReference> _cache;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Singleton, by design")]
        static ReflectionCache()
        {
            Singleton = new ReflectionCache();
        }

        private ReflectionCache()
        {
            _cache = new Dictionary<Pair<Type, object>, WeakReference>();
        }

        public static ReflectionCache Instance
        {
            get { return Singleton; }
        }

        public object this[Pair<Type, object> key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                return _cache.ContainsKey(key) ? _cache[key].Target : null;
            }

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                _cache[key] = new WeakReference(value);
            }
        }
    }
}
