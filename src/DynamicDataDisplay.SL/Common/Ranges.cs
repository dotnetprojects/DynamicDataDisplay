using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay
{
    [DebuggerDisplay(@"{Min} — {Max}")]
    public struct Range<T> : IEquatable<Range<T>>
    {
        public Range(T min, T max)
        {
            this.min = min;
            this.max = max;

#if DEBUG
            if (min is IComparable)
            {
                IComparable c1 = (IComparable)min;
                IComparable c2 = (IComparable)max;

                DebugVerify.Is(c1.CompareTo(c2) <= 0);
            }
#endif
        }

        private readonly T min;
        public T Min
        {
            get { return min; }
        }

        private readonly T max;
        public T Max
        {
            get { return max; }
        }

        public static bool operator ==(Range<T> first, Range<T> second)
        {
            return (first.min.Equals(second.min) && first.max.Equals(second.max));
        }

        public static bool operator !=(Range<T> first, Range<T> second)
        {
            return (!first.min.Equals(second.min) || !first.max.Equals(second.max));
        }

        public override bool Equals(object obj)
        {
            if (obj is Range<T>)
            {
                Range<T> other = (Range<T>)obj;
                return (min.Equals(other.min) && max.Equals(other.max));
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return min.GetHashCode() ^ max.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} — {1}", min, max);
        }

        public bool IsEmpty
        {
            get { return min.Equals(max); }
        }

        public bool Equals(Range<T> other)
        {
            return this == other;
        }
    }
}
