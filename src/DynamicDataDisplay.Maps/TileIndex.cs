using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	/// <summary>
	/// Describes a tile of tiled map.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("({X}, {Y}) @ {Level}")]
	public struct TileIndex : IEquatable<TileIndex>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TileIndex"/> struct.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <param name="level">The level.</param>
		public TileIndex(int x, int y, double level)
		{
			this.x = x;
			this.y = y;
			this.level = level;
		}

		private readonly int x;
		/// <summary>
		/// Gets the X zero-based index of tile.
		/// Zero index is on the left of map.
		/// </summary>
		/// <value>The X.</value>
		public int X { get { return x; } }

		private readonly int y;
		/// <summary>
		/// Gets the Y zero-based index of tile.
		/// Zero index is on the bottom of map.
		/// </summary>
		/// <value>The Y.</value>
		public int Y { get { return y; } }

		private readonly double level;
		/// <summary>
		/// Gets the tile level.
		/// The less the value, the less number of tiles level contains.
		/// </summary>
		/// <value>The level.</value>
		public double Level { get { return level; } }

		/// <summary>
		/// Gets the lower tile.
		/// </summary>
		/// <returns></returns>
		public TileIndex GetLowerTile()
		{
			TileIndex result = new TileIndex((x - ((x < 0) ? 1 : 0)) / 2, (y - ((y < 0) ? 1 : 0)) / 2, level - 1);

			//if (!MapTileProvider.GetTileBoundsGeneric(result).Contains(MapTileProvider.GetTileBoundsGeneric(this)))
			//{
			//}

			return result;

		}

		/// <summary>
		/// Gets the lower tile.
		/// </summary>
		/// <param name="levelUp">Number of levels up.</param>
		/// <returns></returns>
		public TileIndex GetLowerTile(int levelUp)
		{
			TileIndex result = this;
			for (int i = 0; i < levelUp; i++)
			{
				result = result.GetLowerTile();
			}
			return result;
		}

		/// <summary>
		/// Gets a value indicating whether this instance has lower tile.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has lower tile; otherwise, <c>false</c>.
		/// </value>
		public bool HasLowerTile
		{
			get { return level > 0; }
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
			if (obj == null)
				return false;
			if (!(obj is TileIndex))
				return false;

			TileIndex other = (TileIndex)obj;

			return
				level == other.level &&
				x == other.x &&
				y == other.y;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			return x ^ y ^ level.GetHashCode();
		}

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> containing a fully qualified type name.
		/// </returns>
		public override string ToString()
		{
			return String.Format("({0}, {1}) @ {2}", x, y, level);
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="index1">The index1.</param>
		/// <param name="index2">The index2.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(TileIndex index1, TileIndex index2)
		{
			return index1.Equals(index2);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="index1">The index1.</param>
		/// <param name="index2">The index2.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(TileIndex index1, TileIndex index2)
		{
			return !(index1.Equals(index2));
		}

		#region IEquatable<TileIndex> Members

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		public bool Equals(TileIndex other)
		{
			return
				level == other.level &&
				x == other.x &&
				y == other.y;
		}

		#endregion

		/// <summary>
		/// Represents default EqualityComparer for tile indices.
		/// </summary>
		[Serializable]
		public class TileIndexEqualityComparer : IEqualityComparer<TileIndex>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="TileIndexEqualityComparer"/> class.
			/// </summary>
			public TileIndexEqualityComparer() { }

			#region IEqualityComparer<TileIndex> Members

			/// <summary>
			/// Determines whether the specified objects are equal.
			/// </summary>
			/// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
			/// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
			/// <returns>
			/// true if the specified objects are equal; otherwise, false.
			/// </returns>
			public bool Equals(TileIndex x, TileIndex y)
			{
				// todo what is the best order of comparings here?
				return
					x.level == y.level &&
					x.x == y.x &&
					x.y == y.y;
			}

			/// <summary>
			/// Returns a hash code for the specified object.
			/// </summary>
			/// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
			/// <returns>A hash code for the specified object.</returns>
			/// <exception cref="T:System.ArgumentNullException">
			/// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
			/// </exception>
			public int GetHashCode(TileIndex obj)
			{
				return obj.x ^ obj.y ^ obj.level.GetHashCode();
			}

			#endregion
		}
	}
}
