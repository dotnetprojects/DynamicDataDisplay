using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Isolines
{
	/// <summary>
	/// Edge identifier - indicates which side of cell isoline crosses.
	/// </summary>
	internal enum Edge
	{
		// todo check if everything is ok with None.
		None = 0,
		/// <summary>
		/// Isoline crosses left boundary of cell (bit 0)
		/// </summary>
		Left = 1,
		/// <summary>
		/// Isoline crosses top boundary of cell (bit 1)
		/// </summary>
		Top = 2,
		/// <summary>
		/// Isoline crosses right boundary of cell (bit 2)
		/// </summary>
		Right = 4,
		/// <summary>
		/// Isoline crosses bottom boundary of cell (bit 3)
		/// </summary>
		Bottom = 8
	}

	[Flags]
	internal enum CellBitmask
	{
		None = 0,
		LeftTop = 1,
		LeftBottom = 8,
		RightBottom = 4,
		RightTop = 2
	}

	internal static class IsolineExtensions
	{
		internal static bool IsDiagonal(this CellBitmask bitmask)
		{
			return bitmask == (CellBitmask.RightBottom | CellBitmask.LeftTop) ||
				bitmask == (CellBitmask.LeftBottom | CellBitmask.RightTop);
		}

		internal static bool IsAppropriate(this SubCell sub, Edge edge)
		{
			switch (sub)
			{
				case SubCell.LeftBottom:
					return edge == Edge.Left || edge == Edge.Bottom;
				case SubCell.LeftTop:
					return edge == Edge.Left || edge == Edge.Top;
				case SubCell.RightBottom:
					return edge == Edge.Right || edge == Edge.Bottom;
				case SubCell.RightTop:
				default:
					return edge == Edge.Right || edge == Edge.Top;
			}
		}
	}
}
