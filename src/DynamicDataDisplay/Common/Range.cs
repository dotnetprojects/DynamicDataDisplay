using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// An ordered pair of values, representing a segment.
	/// </summary>
	/// <typeparam name="T">Type of each of two values of range.</typeparam>
	[Serializable]
	[DebuggerDisplay(@"{Min} — {Max}")]
	[TypeConverter(typeof(RangeConverter))]
	public struct Range<T> : IEquatable<Range<T>>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Range&lt;T&gt;"/> struct.
		/// </summary>
		/// <param name="min">The minimal value of segment.</param>
		/// <param name="max">The maximal value of segment.</param>
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
		/// <summary>
		/// Gets the minimal value of segment.
		/// </summary>
		/// <value>The min.</value>
		public T Min
		{
			get { return min; }
		}

		private readonly T max;
		/// <summary>
		/// Gets the maximal value of segment.
		/// </summary>
		/// <value>The max.</value>
		public T Max
		{
			get { return max; }
		}

        static  float floatEps = (float)1e-10;
        static  double doubleEps = 1e-10;


		public static bool operator ==(Range<T> first, Range<T> second)
		{
			return (first.min.Equals(second.min) && first.max.Equals(second.max) ||
                first.IsEmpty && second.IsEmpty);
		}

		public static bool operator !=(Range<T> first, Range<T> second)
		{
            return !(first == second);
		}

        public static bool EqualEps(Range<double> first, Range<double> second,double eps)
        {
            double delta = Math.Min(first.GetLength(), second.GetLength());
            return Math.Abs(first.Min - second.Min) < eps * delta &&
                Math.Abs(first.Max - second.Max) < eps * delta;
        }

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		/// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj is Range<T>)
			{
				Range<T> other = (Range<T>)obj;
				return (min.Equals(other.min) && max.Equals(other.max) || IsEmpty && other.IsEmpty);
			}
			else
				return false;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			return min.GetHashCode() ^ max.GetHashCode();
		}

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> containing a fully qualified type name.
		/// </returns>
		public override string ToString()
		{
			return String.Format("{0} — {1}", min, max);
		}

		/// <summary>
		/// Gets a value indicating whether this range is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get {
                if (typeof(T) is IComparable)
                    return ((IComparable)min).CompareTo(max) >= 0;
                else
                    return min.Equals(max);
            }
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		public bool Equals(Range<T> other)
		{
			return this == other;
		}
	}
}
