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

namespace Microsoft.Research.DynamicDataDisplay.Common
{
    /// <summary>
    /// Describes a width, a height and location of a rectangle.
    /// This type is very similar to <see cref="System.Windows.Rect"/>, but has one important difference:
    /// while <see cref="System.Windows.Rect"/> describes a rectangle in screen coordinates, where y axis
    /// points to bottom (that's why <see cref="System.Windows.Rect"/>'s Bottom property is greater than Top).
    /// This type describes rectange in usual coordinates, and y axis point to top.
    /// </summary>
    [DebuggerDisplay("{XMin};{YMin} → {Width}*{Height}")]
    public struct DataRect
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
        public DataRect(Point point, Vector vector)
            : this(point, new Point(point.X + vector.X,point.Y+vector.Y))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRect"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public DataRect(double x, double y, double width, double height)
        {
            if ((width < 0) || (height < 0))
            {
                throw new ArgumentException(WidthAndHeightCannotBeNegative);
            }
            this.xMin = x;
            this.yMin = y;
            this.width = width;
            this.height = height;
        }

        #endregion

        #region Static

        public static DataRect FromPoints(double x1, double y1, double x2, double y2)
        {
            return new DataRect(new Point(x1, y1), new Point(x2, y2));
        }

        public static implicit operator DataRect(Rect rect)
        {
            return rect.ToDataRect();
        }

        public static implicit operator Rect(DataRect rect)
        {
            return rect.ToRect();
        }

        #endregion

        public Rect ToRect()
        {
            return new Rect(xMin, yMin, width, height);
        }

        public DataRect Intersect(DataRect rect)
        {
            if (!IntersectsWith(rect))
                return DataRect.Empty;

            DataRect res = new DataRect();

            double x = Math.Max(this.XMin, rect.XMin);
            double y = Math.Max(this.YMin, rect.YMin);
            res.width = Math.Max((double)(Math.Min(this.XMax, rect.XMax) - x), 0.0);
            res.height = Math.Max((double)(Math.Min(this.YMax, rect.YMax) - y), 0.0);
            res.xMin = x;
            res.yMin = y;

            return res;
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
        }

        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>The top.</value>
        public double YMax
        {
            get
            {
                if (IsEmpty)
                {
                    return Double.PositiveInfinity;
                }
                return yMin + height;
            }
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>The left.</value>
        public double XMin
        {
            get { return xMin; }
        }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>The right.</value>
        public double XMax
        {
            get
            {
                if (IsEmpty)
                {
                    return Double.PositiveInfinity;
                }
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
                {
                    throw new InvalidOperationException(CannotModifyEmptyRect);
                }
                xMin = value.X;
                yMin = value.Y;
            }
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
                {
                    return Size.Empty;
                }
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
                    {
                        throw new InvalidOperationException(CannotModifyEmptyRect);
                    }
                    width = value.Width;
                    height = value.Height;
                }
            }
        }

        public double Width
        {
            get { return width; }
        }

        public double Height
        {
            get { return height; }
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

        private static readonly string CannotModifyEmptyRect = "Cannot modify an empty rect.";
        private static readonly string WidthAndHeightCannotBeNegative = "Width and height cannot be negative";
    }
}
