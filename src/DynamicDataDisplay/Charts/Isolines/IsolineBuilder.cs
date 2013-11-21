using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Isolines
{
    /// <summary>
    /// Generates geometric object for isolines of the input 2d scalar field.
    /// </summary>
    public sealed class IsolineBuilder
    {
        /// <summary>
        /// The density of isolines means the number of levels to draw.
        /// </summary>
        private int density = 12;

        private bool[,] processed;

        /// <summary>Number to be treated as missing value. NaN if no missing value is specified</summary>
        private double missingValue = Double.NaN;

        static IsolineBuilder()
        {
            SetCellDictionaries();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsolineBuilder"/> class.
        /// </summary>
        public IsolineBuilder() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsolineBuilder"/> class for specified 2d scalar data source.
        /// </summary>
        /// <param name="dataSource">The data source with 2d scalar data.</param>
        public IsolineBuilder(IDataSource2D<double> dataSource)
        {
            DataSource = dataSource;
        }

        public double MissingValue
        {
            get
            {
                return missingValue;
            }
            set
            {
                missingValue = value;
            }
        }

        #region Private methods

        private static Dictionary<int, Dictionary<int, Edge>> dictChooser = new Dictionary<int, Dictionary<int, Edge>>();
        private static void SetCellDictionaries()
        {
            var bottomDict = new Dictionary<int, Edge>();
            bottomDict.Add((int)CellBitmask.RightBottom, Edge.Right);
            bottomDict.Add(Edge.Left,
                CellBitmask.LeftTop,
                CellBitmask.LeftBottom | CellBitmask.RightBottom | CellBitmask.RightTop,
                CellBitmask.LeftTop | CellBitmask.RightBottom | CellBitmask.RightTop,
                CellBitmask.LeftBottom);
            bottomDict.Add(Edge.Right,
                CellBitmask.RightTop,
                CellBitmask.LeftBottom | CellBitmask.RightBottom | CellBitmask.LeftTop,
                CellBitmask.LeftBottom | CellBitmask.LeftTop | CellBitmask.RightTop);
            bottomDict.Add(Edge.Top,
                CellBitmask.RightBottom | CellBitmask.RightTop,
                CellBitmask.LeftBottom | CellBitmask.LeftTop);

            var leftDict = new Dictionary<int, Edge>();
            leftDict.Add(Edge.Top,
                CellBitmask.LeftTop,
                CellBitmask.LeftBottom | CellBitmask.RightBottom | CellBitmask.RightTop);
            leftDict.Add(Edge.Right,
                CellBitmask.LeftTop | CellBitmask.RightTop,
                CellBitmask.LeftBottom | CellBitmask.RightBottom);
            leftDict.Add(Edge.Bottom,
                CellBitmask.RightBottom | CellBitmask.RightTop | CellBitmask.LeftTop,
                CellBitmask.LeftBottom);

            var topDict = new Dictionary<int, Edge>();
            topDict.Add(Edge.Right,
                CellBitmask.RightTop,
                CellBitmask.LeftTop | CellBitmask.LeftBottom | CellBitmask.RightBottom);
            topDict.Add(Edge.Right,
                CellBitmask.RightBottom,
                CellBitmask.LeftTop | CellBitmask.LeftBottom | CellBitmask.RightTop);
            topDict.Add(Edge.Left,
                CellBitmask.RightBottom | CellBitmask.RightTop | CellBitmask.LeftTop,
                CellBitmask.LeftBottom,
                CellBitmask.LeftTop,
                CellBitmask.LeftBottom | CellBitmask.RightBottom | CellBitmask.RightTop);
            topDict.Add(Edge.Bottom,
                CellBitmask.RightBottom | CellBitmask.RightTop,
                CellBitmask.LeftTop | CellBitmask.LeftBottom);

            var rightDict = new Dictionary<int, Edge>();
            rightDict.Add(Edge.Top,
                CellBitmask.RightTop,
                CellBitmask.LeftBottom | CellBitmask.RightBottom | CellBitmask.LeftTop);
            rightDict.Add(Edge.Left,
                CellBitmask.LeftTop | CellBitmask.RightTop,
                CellBitmask.LeftBottom | CellBitmask.RightBottom);
            rightDict.Add(Edge.Bottom,
                CellBitmask.RightBottom,
                CellBitmask.LeftTop | CellBitmask.LeftBottom | CellBitmask.RightTop);

            dictChooser.Add((int)Edge.Left, leftDict);
            dictChooser.Add((int)Edge.Right, rightDict);
            dictChooser.Add((int)Edge.Bottom, bottomDict);
            dictChooser.Add((int)Edge.Top, topDict);
        }

        private Edge GetOutEdge(Edge inEdge, ValuesInCell cv, IrregularCell rect, double value)
        {
            // value smaller than all values in corners or 
            // value greater than all values in corners
            if (!cv.ValueBelongTo(value))
            {
                throw new IsolineGenerationException(Strings.Exceptions.IsolinesValueIsOutOfCell);
            }

            CellBitmask cellVal = cv.GetCellValue(value);
            var dict = dictChooser[(int)inEdge];
            if (dict.ContainsKey((int)cellVal))
            {
                Edge result = dict[(int)cellVal];
                switch (result)
                {
                    case Edge.Left:
                        if (cv.LeftTop.IsNaN() || cv.LeftBottom.IsNaN())
                            result = Edge.None;
                        break;
                    case Edge.Right:
                        if (cv.RightTop.IsNaN() || cv.RightBottom.IsNaN())
                            result = Edge.None;
                        break;
                    case Edge.Top:
                        if (cv.RightTop.IsNaN() || cv.LeftTop.IsNaN())
                            result = Edge.None;
                        break;
                    case Edge.Bottom:
                        if (cv.LeftBottom.IsNaN() || cv.RightBottom.IsNaN())
                            result = Edge.None;
                        break;
                }
                return result;
            }
            else if (cellVal.IsDiagonal())
            {
                return GetOutForOpposite(inEdge, cellVal, value, cv, rect);
            }

            const double near_zero = 0.0001;
            const double near_one = 1 - near_zero;

            double lt = cv.LeftTop;
            double rt = cv.RightTop;
            double rb = cv.RightBottom;
            double lb = cv.LeftBottom;

            switch (inEdge)
            {
                case Edge.Left:
                    if (value == lt)
                        value = near_one * lt + near_zero * lb;
                    else if (value == lb)
                        value = near_one * lb + near_zero * lt;
                    else
                        return Edge.None;
                    // Now this is possible because of missing value
                    //throw new IsolineGenerationException(Strings.Exceptions.IsolinesUnsupportedCase);
                    break;
                case Edge.Top:
                    if (value == rt)
                        value = near_one * rt + near_zero * lt;
                    else if (value == lt)
                        value = near_one * lt + near_zero * rt;
                    else
                        return Edge.None;
                    // Now this is possibe because of missing value
                    //throw new IsolineGenerationException(Strings.Exceptions.IsolinesUnsupportedCase);
                    break;
                case Edge.Right:
                    if (value == rb)
                        value = near_one * rb + near_zero * rt;
                    else if (value == rt)
                        value = near_one * rt + near_zero * rb;
                    else
                        return Edge.None;
                    // Now this is possibe because of missing value
                    //throw new IsolineGenerationException(Strings.Exceptions.IsolinesUnsupportedCase);
                    break;
                case Edge.Bottom:
                    if (value == rb)
                        value = near_one * rb + near_zero * lb;
                    else if (value == lb)
                        value = near_one * lb + near_zero * rb;
                    else
                        return Edge.None;
                    // Now this is possibe because of missing value
                    //throw new IsolineGenerationException(Strings.Exceptions.IsolinesUnsupportedCase);
                    break;
            }

            // Recursion?
            //return GetOutEdge(inEdge, cv, rect, value);

            return Edge.None;
        }

        private Edge GetOutForOpposite(Edge inEdge, CellBitmask cellVal, double value, ValuesInCell cellValues, IrregularCell rect)
        {
            Edge outEdge;

            SubCell subCell = GetSubCell(inEdge, value, cellValues);

            int iters = 1000; // max number of iterations
            do
            {
                ValuesInCell subValues = cellValues.GetSubCell(subCell);
                IrregularCell subRect = rect.GetSubRect(subCell);
                outEdge = GetOutEdge(inEdge, subValues, subRect, value);
                if (outEdge == Edge.None)
                    return Edge.None;
                bool isAppropriate = subCell.IsAppropriate(outEdge);
                if (isAppropriate)
                {
                    ValuesInCell sValues = subValues.GetSubCell(subCell);

                    Point point = GetPointXY(outEdge, value, subValues, subRect);
                    segments.AddPoint(point);
                    return outEdge;
                }
                else
                {
                    subCell = GetAdjacentEdge(subCell, outEdge);
                }

                byte e = (byte)outEdge;
                inEdge = (Edge)((e > 2) ? (e >> 2) : (e << 2));
                iters--;
            } while (iters >= 0);

            throw new IsolineGenerationException(Strings.Exceptions.IsolinesDataIsUndetailized);
        }

        private static SubCell GetAdjacentEdge(SubCell sub, Edge edge)
        {
            SubCell res = SubCell.LeftBottom;

            switch (sub)
            {
                case SubCell.LeftBottom:
                    res = edge == Edge.Top ? SubCell.LeftTop : SubCell.RightBottom;
                    break;
                case SubCell.LeftTop:
                    res = edge == Edge.Bottom ? SubCell.LeftBottom : SubCell.RightTop;
                    break;
                case SubCell.RightBottom:
                    res = edge == Edge.Top ? SubCell.RightTop : SubCell.LeftBottom;
                    break;
                case SubCell.RightTop:
                default:
                    res = edge == Edge.Bottom ? SubCell.RightBottom : SubCell.LeftTop;
                    break;
            }

            return res;
        }

        private static SubCell GetSubCell(Edge inEdge, double value, ValuesInCell vc)
        {
            double lb = vc.LeftBottom;
            double rb = vc.RightBottom;
            double rt = vc.RightTop;
            double lt = vc.LeftTop;

            SubCell res = SubCell.LeftBottom;
            switch (inEdge)
            {
                case Edge.Left:
                    res = (Math.Abs(value - lb) < Math.Abs(value - lt)) ? SubCell.LeftBottom : SubCell.LeftTop;
                    break;
                case Edge.Top:
                    res = (Math.Abs(value - lt) < Math.Abs(value - rt)) ? SubCell.LeftTop : SubCell.RightTop;
                    break;
                case Edge.Right:
                    res = (Math.Abs(value - rb) < Math.Abs(value - rt)) ? SubCell.RightBottom : SubCell.RightTop;
                    break;
                case Edge.Bottom:
                default:
                    res = (Math.Abs(value - lb) < Math.Abs(value - rb)) ? SubCell.LeftBottom : SubCell.RightBottom;
                    break;
            }

            ValuesInCell subValues = vc.GetSubCell(res);
            bool valueInside = subValues.ValueBelongTo(value);
            if (!valueInside)
            {
                throw new IsolineGenerationException(Strings.Exceptions.IsolinesDataIsUndetailized);
            }

            return res;
        }

        private static Point GetPoint(double value, double a1, double a2, Vector v1, Vector v2)
        {
            double ratio = (value - a1) / (a2 - a1);

            Verify.IsTrue(0 <= ratio && ratio <= 1);

            Vector r = (1 - ratio) * v1 + ratio * v2;
            return new Point(r.X, r.Y);
        }

        private Point GetPointXY(Edge edge, double value, ValuesInCell vc, IrregularCell rect)
        {
            double lt = vc.LeftTop;
            double lb = vc.LeftBottom;
            double rb = vc.RightBottom;
            double rt = vc.RightTop;

            switch (edge)
            {
                case Edge.Left:
                    return GetPoint(value, lb, lt, rect.LeftBottom, rect.LeftTop);
                case Edge.Top:
                    return GetPoint(value, lt, rt, rect.LeftTop, rect.RightTop);
                case Edge.Right:
                    return GetPoint(value, rb, rt, rect.RightBottom, rect.RightTop);
                case Edge.Bottom:
                    return GetPoint(value, lb, rb, rect.LeftBottom, rect.RightBottom);
                default:
                    throw new InvalidOperationException();
            }
        }

        private bool BelongsToEdge(double value, double edgeValue1, double edgeValue2, bool onBoundary)
        {
            if (!Double.IsNaN(missingValue) && (edgeValue1 == missingValue || edgeValue2 == missingValue))
                return false;

            if (onBoundary)
            {
                return (edgeValue1 <= value && value < edgeValue2) ||
                (edgeValue2 <= value && value < edgeValue1);
            }
            else
            {
                return (edgeValue1 < value && value < edgeValue2) ||
                    (edgeValue2 < value && value < edgeValue1);
            }
        }

        private bool IsPassed(Edge edge, int i, int j, byte[,] edges)
        {
            switch (edge)
            {
                case Edge.Left:
                    return (i == 0) || (edges[i, j] & (byte)edge) != 0;
                case Edge.Bottom:
                    return (j == 0) || (edges[i, j] & (byte)edge) != 0;
                case Edge.Top:
                    return (j == edges.GetLength(1) - 2) || (edges[i, j + 1] & (byte)Edge.Bottom) != 0;
                case Edge.Right:
                    return (i == edges.GetLength(0) - 2) || (edges[i + 1, j] & (byte)Edge.Left) != 0;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void MakeEdgePassed(Edge edge, int i, int j)
        {
            switch (edge)
            {
                case Edge.Left:
                case Edge.Bottom:
                    edges[i, j] |= (byte)edge;
                    break;
                case Edge.Top:
                    edges[i, j + 1] |= (byte)Edge.Bottom;
                    break;
                case Edge.Right:
                    edges[i + 1, j] |= (byte)Edge.Left;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private Edge TrackLine(Edge inEdge, double value, ref int x, ref int y, out double newX, out double newY)
        {
            // Getting output edge
            ValuesInCell vc = (missingValue.IsNaN()) ?
                (new ValuesInCell(values[x, y],
                    values[x + 1, y],
                    values[x + 1, y + 1],
                    values[x, y + 1])) :
                (new ValuesInCell(values[x, y],
                    values[x + 1, y],
                    values[x + 1, y + 1],
                    values[x, y + 1],
                    missingValue));

            IrregularCell rect = new IrregularCell(
                grid[x, y],
                grid[x + 1, y],
                grid[x + 1, y + 1],
                grid[x, y + 1]);

            Edge outEdge = GetOutEdge(inEdge, vc, rect, value);
            if (outEdge == Edge.None)
            {
                newX = newY = -1; // Impossible cell indices
                return Edge.None;
            }

            // Drawing new segment
            Point point = GetPointXY(outEdge, value, vc, rect);
            newX = point.X;
            newY = point.Y;
            segments.AddPoint(point);
            processed[x, y] = true;

            // Whether out-edge already was passed?
            if (IsPassed(outEdge, x, y, edges)) // line is closed
            {
                //MakeEdgePassed(outEdge, x, y); // boundaries should be marked as passed too
                return Edge.None;
            }

            // Make this edge passed
            MakeEdgePassed(outEdge, x, y);

            // Getting next cell's indices
            switch (outEdge)
            {
                case Edge.Left:
                    x--;
                    return Edge.Right;
                case Edge.Top:
                    y++;
                    return Edge.Bottom;
                case Edge.Right:
                    x++;
                    return Edge.Left;
                case Edge.Bottom:
                    y--;
                    return Edge.Top;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void TrackLineNonRecursive(Edge inEdge, double value, int x, int y)
        {
            int s = x, t = y;

            ValuesInCell vc = (missingValue.IsNaN()) ?
                (new ValuesInCell(values[x, y],
                    values[x + 1, y],
                    values[x + 1, y + 1],
                    values[x, y + 1])) :
                (new ValuesInCell(values[x, y],
                    values[x + 1, y],
                    values[x + 1, y + 1],
                    values[x, y + 1],
                    missingValue));

            IrregularCell rect = new IrregularCell(
                grid[x, y],
                grid[x + 1, y],
                grid[x + 1, y + 1],
                grid[x, y + 1]);

            Point point = GetPointXY(inEdge, value, vc, rect);

            segments.StartLine(point, (value - minMax.Min) / (minMax.Max - minMax.Min), value);

            MakeEdgePassed(inEdge, x, y);

            //processed[x, y] = true;

            double x2, y2;
            do
            {
                inEdge = TrackLine(inEdge, value, ref s, ref t, out x2, out y2);
            } while (inEdge != Edge.None);
        }

        #endregion

        private bool HasIsoline(int x, int y)
        {
            return (edges[x,y] != 0 && 
                ((x < edges.GetLength(0) - 1 && edges[x+1,y] != 0) ||
                 (y < edges.GetLength(1) - 1 && edges[x,y+1] != 0)));
        }

        /// <summary>Finds isoline for specified reference value</summary>
        /// <param name="value">Reference value</param>
        private void PrepareCells(double value)
        {
            double currentRatio = (value - minMax.Min) / (minMax.Max - minMax.Min);

            if (currentRatio < 0 || currentRatio > 1)
                return; // No contour lines for such value

            int xSize = dataSource.Width;
            int ySize = dataSource.Height;
            int x, y;
            for (x = 0; x < xSize; x++)
                for (y = 0; y < ySize; y++)
                    edges[x, y] = 0;

            processed = new bool[xSize, ySize];

            // Looking in boundaries.
            // left
            for (y = 1; y < ySize; y++)
            {
                if (BelongsToEdge(value, values[0, y - 1], values[0, y], true) &&
                    (edges[0, y - 1] & (byte)Edge.Left) == 0)
                {
                    TrackLineNonRecursive(Edge.Left, value, 0, y - 1);
                }
            }

            // bottom
            for (x = 0; x < xSize - 1; x++)
            {
                if (BelongsToEdge(value, values[x, 0], values[x + 1, 0], true)
                    && (edges[x, 0] & (byte)Edge.Bottom) == 0)
                {
                    TrackLineNonRecursive(Edge.Bottom, value, x, 0);
                };
            }

            // right
            x = xSize - 1;
            for (y = 1; y < ySize; y++)
            {
                // Is this correct?
                //if (BelongsToEdge(value, values[0, y - 1], values[0, y], true) &&
                //    (edges[0, y - 1] & (byte)Edge.Left) == 0)
                //{
                //    TrackLineNonRecursive(Edge.Left, value, 0, y - 1);
                //};

                if (BelongsToEdge(value, values[x, y - 1], values[x, y], true) &&
                    (edges[x, y - 1] & (byte)Edge.Left) == 0)
                {
                    TrackLineNonRecursive(Edge.Right, value, x - 1, y - 1);
                };
            }

            // horizontals
            for (x = 1; x < xSize - 1; x++)
                for (y = 1; y < ySize - 1; y++)
                {
                    if ((edges[x, y] & (byte)Edge.Bottom) == 0 &&
                        BelongsToEdge(value, values[x, y], values[x + 1, y], false) &&
                        !processed[x,y-1])
                    {
                        TrackLineNonRecursive(Edge.Top, value, x, y-1);
                    }
                    if ((edges[x, y] & (byte)Edge.Bottom) == 0 &&
                        BelongsToEdge(value, values[x, y], values[x + 1, y], false) &&
                        !processed[x,y])
                    {
                        TrackLineNonRecursive(Edge.Bottom, value, x, y);
                    }
                    if ((edges[x, y] & (byte)Edge.Left) == 0 &&
                        BelongsToEdge(value, values[x, y], values[x, y - 1], false) &&
                        !processed[x-1,y-1])
                    {
                       TrackLineNonRecursive(Edge.Right, value, x - 1, y - 1);
                    }
                    if ((edges[x, y] & (byte)Edge.Left) == 0 &&
                        BelongsToEdge(value, values[x, y], values[x, y - 1], false) &&
                        !processed[x,y-1])
                    {
                       TrackLineNonRecursive(Edge.Left, value, x, y - 1);
                    }
                }
        }

        /// <summary>
        /// Builds isoline data for 2d scalar field contained in data source.
        /// </summary>
        /// <returns>Collection of data describing built isolines.</returns>
        public IsolineCollection BuildIsoline()
        {
            VerifyDataSource();

            segments = new IsolineCollection();

            minMax = (Double.IsNaN(missingValue) ? dataSource.GetMinMax() : dataSource.GetMinMax(missingValue));

            segments.Min = minMax.Min;
            segments.Max = minMax.Max;

            if (!minMax.IsEmpty)
            {
                values = dataSource.Data;
                double[] levels = GetLevelsForIsolines();

                foreach (double level in levels)
                {
                    PrepareCells(level);
                }

                if (segments.Lines.Count > 0 && segments.Lines[segments.Lines.Count - 1].OtherPoints.Count == 0)
                    segments.Lines.RemoveAt(segments.Lines.Count - 1);
            }
            return segments;
        }

        /// <summary>
        /// Builds isoline data for the specified level in 2d scalar field.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public IsolineCollection BuildIsoline(double level)
        {
            VerifyDataSource();

            segments = new IsolineCollection();

            minMax = (Double.IsNaN(missingValue) ? dataSource.GetMinMax() : dataSource.GetMinMax(missingValue));
            if (!minMax.IsEmpty)
            {
                values = dataSource.Data;


                PrepareCells(level);

                if (segments.Lines.Count > 0 && segments.Lines[segments.Lines.Count - 1].OtherPoints.Count == 0)
                    segments.Lines.RemoveAt(segments.Lines.Count - 1);
            }
            return segments;
        }

        private void VerifyDataSource()
        {
            if (dataSource == null)
                throw new InvalidOperationException(Strings.Exceptions.IsolinesDataSourceShouldBeSet);
        }

        IsolineCollection segments;

        private double[,] values;
        private byte[,] edges;
        private Point[,] grid;

        private Range<double> minMax;
        private IDataSource2D<double> dataSource;
        /// <summary>
        /// Gets or sets the data source - 2d scalar field.
        /// </summary>
        /// <value>The data source.</value>
        public IDataSource2D<double> DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    value.VerifyNotNull("value");

                    dataSource = value;
                    grid = dataSource.Grid;
                    edges = new byte[dataSource.Width, dataSource.Height];
                }
            }
        }

        private const double shiftPercent = 0.05;
        private double[] GetLevelsForIsolines()
        {
            double[] levels;
            double min = minMax.Min;
            double max = minMax.Max;

            double step = (max - min) / (density - 1);
            double delta = (max - min);

            levels = new double[density];
            levels[0] = min + delta * shiftPercent;
            levels[levels.Length - 1] = max - delta * shiftPercent;

            for (int i = 1; i < levels.Length - 1; i++)
                levels[i] = min + i * step;

            return levels;
        }
    }
}
