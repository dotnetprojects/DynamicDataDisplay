using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Windows.Markup;
using System.Globalization;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Describes a rectangle in viewport or data coordinates.
	/// </summary>
	[Serializable]
	[ValueSerializer(typeof(DataRectSerializer))]
	[TypeConverter(typeof(DataRectConverter))]
	public struct DataRect : IEquatable<DataRect>, IFormattable
	{
		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="rect">Source rect.</param>
		public DataRect(Rect rect)
		{
			xMin = rect.X;
			yMin = rect.Y;
			width = rect.Width;
			height = rect.Height;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="size">The size.</param>
		public DataRect(Size size)
		{
			if (size.IsEmpty)
			{
				this = emptyRect;
			}
			else
			{
				xMin = yMin = 0.0;
				width = size.Width;
				height = size.Height;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="size">The size.</param>
		public DataRect(Point location, Size size)
		{
			if (size.IsEmpty)
			{
				this = emptyRect;
			}
			else
			{
				xMin = location.X;
				yMin = location.Y;
				width = size.Width;
				height = size.Height;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="point1">The point1.</param>
		/// <param name="point2">The point2.</param>
		public DataRect(Point point1, Point point2)
		{
			xMin = Math.Min(point1.X, point2.X);
			yMin = Math.Min(point1.Y, point2.Y);
			width = Math.Max((double)(Math.Max(point1.X, point2.X) - xMin), 0);
			height = Math.Max((double)(Math.Max(point1.Y, point2.Y) - yMin), 0);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <param name="vector">The vector.</param>
		public DataRect(Point point, Vector vector) : this(point, point + vector) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="xMin">The minimal x.</param>
		/// <param name="yMin">The minimal y.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public DataRect(double xMin, double yMin, double width, double height)
		{
			if ((width < 0) || (height < 0))
				throw new ArgumentException(Strings.Exceptions.WidthAndHeightCannotBeNegative);

			this.xMin = xMin;
			this.yMin = yMin;
			this.width = width;
			this.height = height;
		}

		#endregion

		#region Static

		/// <summary>
		/// Creates the DataRect from minimal and maximal 'x' and 'y' coordinates.
		/// </summary>
		/// <param name="xMin">The x min.</param>
		/// <param name="yMin">The y min.</param>
		/// <param name="xMax">The x max.</param>
		/// <param name="yMax">The y max.</param>
		/// <returns></returns>
		public static DataRect Create(double xMin, double yMin, double xMax, double yMax)
		{
			DataRect rect = new DataRect(xMin, yMin, xMax - xMin, yMax - yMin);
			return rect;
		}

		public static DataRect FromPoints(double x1, double y1, double x2, double y2)
		{
			return new DataRect(new Point(x1, y1), new Point(x2, y2));
		}

		public static DataRect FromCenterSize(Point center, double width, double height)
		{
			DataRect rect = new DataRect(center.X - width / 2, center.Y - height / 2, width, height);
			return rect;
		}

		public static DataRect FromCenterSize(Point center, Size size)
		{
			return FromCenterSize(center, size.Width, size.Height);
		}

		public static DataRect Intersect(DataRect rect1, DataRect rect2)
		{
			rect1.Intersect(rect2);
			return rect1;
		}

		public static implicit operator DataRect(Rect rect)
		{
			return new DataRect(rect);
		}

		#endregion

		public Rect ToRect()
		{
			return new Rect(xMin, yMin, width, height);
		}

		public void Intersect(DataRect rect)
		{
			if (!IntersectsWith(rect))
			{
				this = DataRect.Empty;
				return;
			}

			DataRect res = new DataRect();

			double x = Math.Max(this.XMin, rect.XMin);
			double y = Math.Max(this.YMin, rect.YMin);
			res.width = Math.Max((double)(Math.Min(this.XMax, rect.XMax) - x), 0.0);
			res.height = Math.Max((double)(Math.Min(this.YMax, rect.YMax) - y), 0.0);
			res.xMin = x;
			res.yMin = y;

			this = res;
		}

		public bool IntersectsWith(DataRect rect)
		{
			if (IsEmpty || rect.IsEmpty)
				return false;

			return ((((rect.XMin <= this.XMax) && (rect.XMax >= this.XMin)) && (rect.YMax >= this.YMin)) && (rect.YMin <= this.YMax));
		}

		private double xMin;
		private double yMin;
		private double width;
		private double height;

		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get { return width < 0; }
		}

		/// <summary>
		/// Gets the bottom.
		/// </summary>
		/// <value>The bottom.</value>
		public double YMin
		{
			get { return yMin; }
			set
			{
				if (this.IsEmpty)
					throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

				yMin = value;
			}
		}

		/// <summary>
		/// Gets the maximal y value.
		/// </summary>
		/// <value>The top.</value>
		public double YMax
		{
			get
			{
				if (IsEmpty)
					return Double.PositiveInfinity;

				return yMin + height;
			}
		}

		/// <summary>
		/// Gets the minimal x value.
		/// </summary>
		/// <value>The left.</value>
		public double XMin
		{
			get { return xMin; }
			set
			{
				if (this.IsEmpty)
					throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

				xMin = value;
			}
		}

		/// <summary>
		/// Gets the maximal x value.
		/// </summary>
		/// <value>The right.</value>
		public double XMax
		{
			get
			{
				if (IsEmpty)
					return Double.PositiveInfinity;

				return xMin + width;
			}
		}

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>The location.</value>
		public Point Location
		{
			get { return new Point(xMin, yMin); }
			set
			{
				if (IsEmpty)
					throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

				xMin = value.X;
				yMin = value.Y;
			}
		}

		public Point XMaxYMax
		{
			get { return new Point(XMax, YMax); }
		}

		public Point XMinYMin
		{
			get { return new Point(xMin, yMin); }
		}

		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		/// <value>The size.</value>
		public Size Size
		{
			get
			{
				if (IsEmpty)
					return Size.Empty;

				return new Size(width, height);
			}
			set
			{
				if (value.IsEmpty)
				{
					this = emptyRect;
				}
				else
				{
					if (IsEmpty)
						throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

					width = value.Width;
					height = value.Height;
				}
			}
		}

		public double Width
		{
			get { return width; }
			set
			{
				if (this.IsEmpty)
					throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);
				if (value < 0)
					throw new ArgumentOutOfRangeException(Strings.Exceptions.DataRectSizeCannotBeNegative);

				width = value;
			}
		}

		public double Height
		{
			get { return height; }
			set
			{
				if (this.IsEmpty)
					throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);
				if (value < 0)
					throw new ArgumentOutOfRangeException(Strings.Exceptions.DataRectSizeCannotBeNegative);

				height = value;
			}
		}

		private static readonly DataRect emptyRect = CreateEmptyRect();

		public static DataRect Empty
		{
			get { return DataRect.emptyRect; }
		}

		private static DataRect CreateEmptyRect()
		{
			DataRect rect = new DataRect();
			rect.xMin = Double.PositiveInfinity;
			rect.yMin = Double.PositiveInfinity;
			rect.width = Double.NegativeInfinity;
			rect.height = Double.NegativeInfinity;
			return rect;
		}

		private static readonly DataRect infinite = new DataRect(Double.MinValue / 2, Double.MinValue / 2, Double.MaxValue, Double.MaxValue);
		public static DataRect Infinite
		{
			get { return infinite; }
		}

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
			if (obj == null)
				return false;

			if (!(obj is DataRect))
				return false;

			DataRect other = (DataRect)obj;

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
			if (IsEmpty)
				return 0;

			return xMin.GetHashCode() ^
					width.GetHashCode() ^
					yMin.GetHashCode() ^
					height.GetHashCode();
		}

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> containing a fully qualified type name.
		/// </returns>
		public override string ToString()
		{
			if (IsEmpty)
				return "Empty";

			return String.Format("({0};{1}) -> {2}*{3}", xMin, yMin, width, height);
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(DataRect rect1, DataRect rect2)
		{
			return rect1.Equals(rect2);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(DataRect rect1, DataRect rect2)
		{
			return !rect1.Equals(rect2);
		}

		public static bool EqualEps(DataRect rect1, DataRect rect2, double eps)
		{
			double width = Math.Min(rect1.Width, rect2.Width);
			double height = Math.Min(rect1.Height, rect2.Height);
			return Math.Abs(rect1.XMin - rect2.XMin) < width * eps &&
				   Math.Abs(rect1.XMax - rect2.XMax) < width * eps &&
				   Math.Abs(rect1.YMin - rect2.YMin) < height * eps &&
				   Math.Abs(rect1.YMax - rect2.YMax) < height * eps;
		}

		#endregion

		#region IEquatable<DataRect> Members

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		public bool Equals(DataRect other)
		{
			if (this.IsEmpty)
				return other.IsEmpty;

			return xMin == other.xMin &&
					width == other.width &&
					yMin == other.yMin &&
					height == other.height;
		}

		#endregion

		/// <summary>
		/// Determines whether this DataRect contains point with specified coordinates.
		/// </summary>
		/// <param name="x">The x coordinate of point.</param>
		/// <param name="y">The y coordinate of point.</param>
		/// <returns>
		/// 	<c>true</c> if contains point with specified coordinates; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(double x, double y)
		{
			if (this.IsEmpty)
				return false;

			return x >= xMin &&
				x <= XMax &&
				y >= yMin &&
				y <= YMax;
		}

		public bool Contains(Point point)
		{
			return Contains(point.X, point.Y);
		}

		public bool Contains(DataRect rect)
		{
			if (this.IsEmpty || rect.IsEmpty)
				return false;

			return
				this.xMin <= rect.xMin &&
				this.yMin <= rect.yMin &&
				this.XMax >= rect.XMax &&
				this.YMax >= rect.YMax;
		}

		public void Offset(Vector offsetVector)
		{
			if (this.IsEmpty)
				throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

			this.xMin += offsetVector.X;
			this.yMin += offsetVector.Y;
		}

		public void Offset(double offsetX, double offsetY)
		{
			if (this.IsEmpty)
				throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

			this.xMin += offsetX;
			this.yMin += offsetY;
		}

		public static DataRect Offset(DataRect rect, double offsetX, double offsetY)
		{
			rect.Offset(offsetX, offsetY);
			return rect;
		}

		internal void UnionFinite(DataRect rect)
		{
			if (!rect.IsEmpty)
			{
				if (rect.xMin.IsInfinite())
					rect.xMin = 0;
				if (rect.yMin.IsInfinite())
					rect.yMin = 0;
				if (rect.width.IsInfinite())
					rect.width = 0;
				if (rect.height.IsInfinite())
					rect.height = 0;
			}

			Union(rect);
		}

		public void Union(DataRect rect)
		{
			if (IsEmpty)
			{
				this = rect;
				return;
			}
			else if (!rect.IsEmpty)
			{
				double minX = Math.Min(xMin, rect.xMin);
				double minY = Math.Min(yMin, rect.yMin);

				if (rect.width == Double.PositiveInfinity || this.width == Double.PositiveInfinity)
				{
					this.width = Double.PositiveInfinity;
				}
				else
				{
					double maxX = Math.Max(XMax, rect.XMax);
					this.width = Math.Max(maxX - minX, 0.0);
				}

				if (rect.height == Double.PositiveInfinity || this.height == Double.PositiveInfinity)
				{
					this.height = Double.PositiveInfinity;
				}
				else
				{
					double maxY = Math.Max(YMax, rect.YMax);
					this.height = Math.Max(maxY - minY, 0.0);
				}

				this.xMin = minX;
				this.yMin = minY;
			}
		}

		public void Union(Point point)
		{
			this.Union(new DataRect(point, point));
		}

		public static DataRect Union(DataRect rect, Point point)
		{
			rect.Union(point);

			return rect;
		}

		public static DataRect Union(DataRect rect1, DataRect rect2)
		{
			rect1.Union(rect2);

			return rect1;
		}

		internal string ConvertToString(string format, IFormatProvider provider)
		{
			if (IsEmpty)
				return "Empty";

			char listSeparator = TokenizerHelper.GetNumericListSeparator(provider);
			return String.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}", listSeparator, xMin, yMin, width, height);
		}

		/// <summary>
		/// Parses the specified string as a DataRect.
		/// </summary>
		/// <remarks>
		/// There are three possible string patterns, that are recognized as string representation of DataRect:
		/// 1) Literal string "Empty" - represents an DataRect.Empty rect;
		/// 2) String in format "d,d,d,d", where d is a floating-point number with '.' as decimal separator - is considered as a string 
		/// of "XMin,YMin,Width,Height";
		/// 3) String in format "d,d d,d", where d is a floating-point number with '.' as decimal separator - is considered as a string
		/// of "XMin,YMin XMax,YMax".
		/// </remarks>
		/// <param name="source">The source.</param>
		/// <returns>DataRect, parsed from the given input string.</returns>
		public static DataRect Parse(string source)
		{
			DataRect rect;
			IFormatProvider cultureInfo = CultureInfo.GetCultureInfo("en-us");

			if (source == "Empty")
			{
				rect = DataRect.Empty;
			}
			else
			{
				// format X,Y,Width,Height
				string[] values = source.Split(',');
				if (values.Length == 4)
				{
					rect = new DataRect(
						Convert.ToDouble(values[0], cultureInfo),
						Convert.ToDouble(values[1], cultureInfo),
						Convert.ToDouble(values[2], cultureInfo),
						Convert.ToDouble(values[3], cultureInfo)
						);
				}
				else
				{
					// format XMin, YMin - XMax, YMax
					values = source.Split(new Char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
					rect = DataRect.Create(
						Convert.ToDouble(values[0], cultureInfo),
						Convert.ToDouble(values[1], cultureInfo),
						Convert.ToDouble(values[2], cultureInfo),
						Convert.ToDouble(values[3], cultureInfo)
						);
				}
			}

			return rect;
		}

		#region IFormattable Members

		string IFormattable.ToString(string format, IFormatProvider formatProvider)
		{
			return ConvertToString(format, formatProvider);
		}

		#endregion
	}
}
