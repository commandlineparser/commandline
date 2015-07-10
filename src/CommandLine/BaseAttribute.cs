using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine
{
    public abstract class BaseAttribute
    {
        private int min;
        private int max;
        private object @default;

        protected BaseAttribute()
        {
            min = -1;
            max = -1;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a command line option is required.
        /// </summary>
        public bool Required
        {
            get;
            set;
        }

        /// <summary>
        /// When applied to <see cref="System.Collections.Generic.IEnumerable{T}"/> properties defines
        /// the lower range of items.
        /// </summary>
        /// <remarks>If not set, no lower range is enforced.</remarks>
        public int Min
        {
            get { return min; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentNullException("value");
                }

                min = value;
            }
        }

        /// <summary>
        /// When applied to <see cref="System.Collections.Generic.IEnumerable{T}"/> properties defines
        /// the upper range of items.
        /// </summary>
        /// <remarks>If not set, no upper range is enforced.</remarks>
        public int Max
        {
            get { return max; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentNullException("value");
                }

                max = value;
            }
        }

        /// <summary>
        /// Gets or sets mapped property default value.
        /// </summary>
        public object Default
        {
            get { return @default; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                @default = value;
            }
        }
    }
}
