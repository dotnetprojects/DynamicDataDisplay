using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Represents simple rectangle with corner coordinates of specified types.
	/// </summary>
	/// <typeparam name="THorizontal">The horizontal values type.</typeparam>
	/// <typeparam name="TVertical">The vertical values type.</typeparam>
	public struct GenericRect<THorizontal, TVertical> : IEquatable<GenericRect<THorizontal, TVertical>>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private THorizontal xMin;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TVertical yMin;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private THorizontal xMax;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TVertical yMax;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericRect&lt;THorizontal, TVertical&gt;"/> struct.
		/// </summary>
		/// <param name="xMin">The minimal x value.</param>
		/// <param name="yMin">The minimal y value.</param>
		/// <param name="xMax">The maximal x value.</param>
		/// <param name="yMax">The maximal y value.</param>
		public GenericRect(THorizontal xMin, TVertical yMin, THorizontal xMax, TVertical yMax)
		{
			this.xMin = xMin;
			this.xMax = xMax;
			this.yMin = yMin;
			this.yMax = yMax;
		}

		/// <summary>
		/// Gets the minimal X value.
		/// </summary>
		/// <value>The X min.</value>
		public THorizontal XMin { get { return xMin; } }
		/// <summary>
		/// Gets the minimal Y value.
		/// </summary>
		/// <value>The Y min.</value>
		public TVertical YMin { get { return yMin; } }
		/// <summary>
		/// Gets the maximal X value.
		/// </summary>
		/// <value>The X max.</value>
		public THorizontal XMax { get { return xMax; } }
		/// <summary>
		/// Gets the maximal Y value.
		/// </summary>
		/// <value>The Y max.</value>
		public TVertical YMax { get { return yMax; } }

		#region Object overrides

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		/// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (!(obj is GenericRect<THorizontal, TVertical>))
				return false;

			GenericRect<THorizontal, TVertical> other = (GenericRect<THorizontal, TVertical>)obj;

			return Equals(other);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			return
				xMin.GetHashCode() ^
				xMax.GetHashCode() ^
				yMin.GetHashCode() ^
				yMax.GetHashCode();
		}

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> containing a fully qualified type name.
		/// </returns>
		public override string ToString()
		{
			return String.Format("({0},{1}) - ({2},{3})", xMin, yMin, xMax, yMax);
		}

		#endregion

		#region IEquatable<GenericRect<THorizontal,TVertical>> Members

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		public bool Equals(GenericRect<THorizontal, TVertical> other)
		{
			return
				this.xMin.Equals(other.xMin) &&
				this.xMax.Equals(other.xMax) &&
				this.yMin.Equals(other.yMin) &&
				this.yMax.Equals(other.yMax);
		}

		#endregion

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(GenericRect<THorizontal, TVertical> rect1, GenericRect<THorizontal, TVertical> rect2)
		{
			return rect1.Equals(rect2);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(GenericRect<THorizontal, TVertical> rect1, GenericRect<THorizontal, TVertical> rect2)
		{
			return !rect1.Equals(rect2);
		}
	}
}
