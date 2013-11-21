using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Isolines
{
	/// <summary>
	/// Isoline's grid cell
	/// </summary>
	internal interface ICell
	{
		Vector LeftTop { get; }
		Vector LeftBottom { get; }
		Vector RightTop { get; }
		Vector RightBottom { get; }
	}

	internal sealed class IrregularCell : ICell
	{
		public IrregularCell(Vector leftBottom, Vector rightBottom, Vector rightTop, Vector leftTop)
		{
			this.leftBottom = leftBottom;
			this.rightBottom = rightBottom;
			this.rightTop = rightTop;
			this.leftTop = leftTop;
		}

		public IrregularCell(Point lb, Point rb, Point rt, Point lt)
		{
			leftTop = lt.ToVector();
			leftBottom = lb.ToVector();
			rightTop = rt.ToVector();
			rightBottom = rb.ToVector();
		}

		#region ICell Members

		private readonly Vector leftTop;
		public Vector LeftTop
		{
			get { return leftTop; }
		}

		private readonly Vector leftBottom;
		public Vector LeftBottom
		{
			get { return leftBottom; }
		}

		private readonly Vector rightTop;
		public Vector RightTop
		{
			get { return rightTop; }
		}

		private readonly Vector rightBottom;
		public Vector RightBottom
		{
			get { return rightBottom; }
		}

		#endregion

		#region Sides
		public Vector LeftSide
		{
			get { return (leftBottom + leftTop) / 2; }
		}

		public Vector RightSide
		{
			get { return (rightBottom + rightTop) / 2; }
		}
		public Vector TopSide
		{
			get { return (leftTop + rightTop) / 2; }
		}
		public Vector BottomSide
		{
			get { return (leftBottom + rightBottom) / 2; }
		}
		#endregion

		public Point Center
		{
			get { return ((LeftSide + RightSide) / 2).ToPoint(); }
		}

		public IrregularCell GetSubRect(SubCell sub)
		{
			switch (sub)
			{
				case SubCell.LeftBottom:
					return new IrregularCell(LeftBottom, BottomSide, Center.ToVector(), LeftSide);
				case SubCell.LeftTop:
					return new IrregularCell(LeftSide, Center.ToVector(), TopSide, LeftTop);
				case SubCell.RightBottom:
					return new IrregularCell(BottomSide, RightBottom, RightSide, Center.ToVector());
				case SubCell.RightTop:
				default:
					return new IrregularCell(Center.ToVector(), RightSide, RightTop, TopSide);
			}
		}
	}

	internal enum SubCell
	{
		LeftBottom = 0,
		LeftTop = 1,
		RightBottom = 2,
		RightTop = 3
	}

	internal class ValuesInCell
	{
        double min = Double.MaxValue, max = Double.MinValue;

        /// <summary>Initializes values in four corners of cell</summary>
        /// <param name="leftBottom"></param>
        /// <param name="rightBottom"></param>
        /// <param name="rightTop"></param>
        /// <param name="leftTop"></param>
        /// <remarks>Some or all values can be NaN. That means that value is not specified (misssing)</remarks>
		public ValuesInCell(double leftBottom, double rightBottom, double rightTop, double leftTop)
		{
			this.leftTop = leftTop;
			this.leftBottom = leftBottom;
			this.rightTop = rightTop;
			this.rightBottom = rightBottom;

            // Find max and min values (with respect to possible NaN values)
            if (!Double.IsNaN(leftTop))
            {
                if (min > leftTop)
                    min = leftTop;
                if (max < leftTop)
                    max = leftTop;
            }

            if (!Double.IsNaN(leftBottom))
            {
                if (min > leftBottom)
                    min = leftBottom;
                if (max < leftBottom)
                    max = leftBottom;
            }

            if (!Double.IsNaN(rightTop))
            {
                if (min > rightTop)
                    min = rightTop;
                if (max < rightTop)
                    max = rightTop;
            }

            if (!Double.IsNaN(rightBottom))
            {
                if (min > rightBottom)
                    min = rightBottom;
                if (max < rightBottom)
                    max = rightBottom;
            }

            left = (leftTop + leftBottom) / 2;
            bottom = (leftBottom + rightBottom) / 2;
            right = (rightTop + rightBottom) / 2;
            top = (rightTop + leftTop) / 2;
		}

        public ValuesInCell(double leftBottom, double rightBottom, double rightTop, double leftTop, double missingValue)
		{
			DebugVerify.IsNotNaN(leftBottom);
			DebugVerify.IsNotNaN(rightBottom);
			DebugVerify.IsNotNaN(rightTop);
			DebugVerify.IsNotNaN(leftTop);
           
            // Copy values and find min and max with respect to possible missing values
            if (leftTop != missingValue)
            {
                this.leftTop = leftTop;
                if (min > leftTop)
                    min = leftTop;
                if (max < leftTop)
                    max = leftTop;
            }
            else
                this.leftTop = Double.NaN;

            if (leftBottom != missingValue)
            {
                this.leftBottom = leftBottom;
                if (min > leftBottom)
                    min = leftBottom;
                if (max < leftBottom)
                    max = leftBottom;
            }
            else
                this.leftBottom = Double.NaN;

            if (rightTop != missingValue)
            {
                this.rightTop = rightTop;
                if (min > rightTop)
                    min = rightTop;
                if (max < rightTop)
                    max = rightTop;
            }
            else
                this.rightTop = Double.NaN;

            if (rightBottom != missingValue)
            {
                this.rightBottom = rightBottom;
                if (min > rightBottom)
                    min = rightBottom;
                if (max < rightBottom)
                    max = rightBottom;
            }
            else
                this.rightBottom = Double.NaN;

            left = (this.leftTop + this.leftBottom) / 2;
            bottom = (this.leftBottom + this.rightBottom) / 2;
            right = (this.rightTop + this.rightBottom) / 2;
            top = (this.rightTop + this.leftTop) / 2;


/*            
            if (leftTop != missingValue && )
            {
                if (leftBottom != missingValue)
                    left = (leftTop + leftBottom) / 2;
                else
                    left = Double.NaN;

                if (rightTop != missingValue)
                    top = (leftTop + rightTop) / 2;
                else
                    top = Double.NaN;
            }

            if (rightBottom != missingValue)
            {
                if (leftBottom != missingValue)
                    bottom = (leftBottom + rightBottom) / 2;
                else
                    bottom = Double.NaN;

                if (rightTop != missingValue)
                    right = (rightTop + rightBottom) / 2;
                else
                    right = Double.NaN;
            }*/
		}


		/*internal bool ValueBelongTo(double value)
		{
			IEnumerable<double> values = new double[] { leftTop, leftBottom, rightTop, rightBottom };

			return !(values.All(v => v > value) || values.All(v => v < value));
		}*/

        internal bool ValueBelongTo(double value)
        {
            return (min <= value && value <= max);
        }

		#region Edges
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly double leftTop;
		public double LeftTop { get { return leftTop; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly double leftBottom;
		public double LeftBottom { get { return leftBottom; } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly double rightTop;
		public double RightTop
		{
			get { return rightTop; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly double rightBottom;
		public double RightBottom
		{
			get { return rightBottom; }
		}
		#endregion

		#region Sides & center
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly double left;
		public double Left
		{
			get { return left; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly double right;
		public double Right
		{
			get { return right; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly double top;
		public double Top
		{
			get { return top; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly double bottom;
		public double Bottom
		{
			get { return bottom; }
		}

		public double Center
		{
			get { return (Left + Right) * 0.5; }
		}
		#endregion

		#region SubCells
		public ValuesInCell LeftTopCell
		{
			get { return new ValuesInCell(Left, Center, Top, LeftTop); }
		}

		public ValuesInCell RightTopCell
		{
			get { return new ValuesInCell(Center, Right, RightTop, Top); }
		}

		public ValuesInCell RightBottomCell
		{
			get { return new ValuesInCell(Bottom, RightBottom, Right, Center); }
		}

		public ValuesInCell LeftBottomCell
		{
			get { return new ValuesInCell(LeftBottom, Bottom, Center, Left); }
		}

		public ValuesInCell GetSubCell(SubCell subCell)
		{
			switch (subCell)
			{
				case SubCell.LeftBottom:
					return LeftBottomCell;
				case SubCell.LeftTop:
					return LeftTopCell;
				case SubCell.RightBottom:
					return RightBottomCell;
				case SubCell.RightTop:
				default:
					return RightTopCell;
			}
		}

		#endregion

		/// <summary>
		/// Returns bitmask of comparison of values at cell corners with reference value.
		/// Corresponding bit is set to one if value at cell corner is greater than reference value. 
		/// a------b
		/// | Cell |
		/// d------c
		/// </summary>
		/// <param name="a">Value at corner (see figure)</param>
		/// <param name="b">Value at corner (see figure)</param>
		/// <param name="c">Value at corner (see figure)</param>
		/// <param name="d">Value at corner (see figure)</param>
		/// <param name="value">Reference value</param>
		/// <returns>Bitmask</returns>
		public CellBitmask GetCellValue(double value)
		{
			CellBitmask n = CellBitmask.None;
			if (!Double.IsNaN(leftTop) && leftTop > value)
				n |= CellBitmask.LeftTop;
			if (!Double.IsNaN(leftBottom) && leftBottom > value)
				n |= CellBitmask.LeftBottom;
			if (!Double.IsNaN(rightBottom) && rightBottom > value)
				n |= CellBitmask.RightBottom;
			if (!Double.IsNaN(rightTop) && rightTop > value)
				n |= CellBitmask.RightTop;

			return n;
		}
	}
}
