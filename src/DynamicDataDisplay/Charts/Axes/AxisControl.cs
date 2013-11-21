using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    /// <summary>
    /// Defines a base class for axis UI representation.
    /// Contains a number of properties that can be used to adjust ticks set and their look.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [TemplatePart(Name = "PART_AdditionalLabelsCanvas", Type = typeof(StackCanvas))]
    [TemplatePart(Name = "PART_CommonLabelsCanvas", Type = typeof(StackCanvas))]
    [TemplatePart(Name = "PART_TicksPath", Type = typeof(Path))]
    [TemplatePart(Name = "PART_ContentsGrid", Type = typeof(Grid))]
    public abstract class AxisControl<T> : AxisControlBase
    {
        private const string templateKey = "axisControlTemplate";
        private const string additionalLabelTransformKey = "additionalLabelsTransform";

        private const string PART_AdditionalLabelsCanvas = "PART_AdditionalLabelsCanvas";
        private const string PART_CommonLabelsCanvas = "PART_CommonLabelsCanvas";
        private const string PART_TicksPath = "PART_TicksPath";
        private const string PART_ContentsGrid = "PART_ContentsGrid";

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisControl&lt;T&gt;"/> class.
        /// </summary>
        protected AxisControl()
        {
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            Background = Brushes.Transparent;
            //ClipToBounds = true;
            Focusable = false;

            UpdateUIResources();
            UpdateSizeGetters();
        }

        internal void MakeDependent()
        {
            independent = false;
        }

        /// <summary>
        /// This conversion is performed to make horizontal one-string and two-string labels
        /// stay at one height.
        /// </summary>
        /// <param name="placement"></param>
        /// <returns></returns>
        private static AxisPlacement GetBetterPlacement(AxisPlacement placement)
        {
            switch (placement)
            {
                case AxisPlacement.Left:
                    return AxisPlacement.Left;
                case AxisPlacement.Right:
                    return AxisPlacement.Right;
                case AxisPlacement.Top:
                    return AxisPlacement.Top;
                case AxisPlacement.Bottom:
                    return AxisPlacement.Bottom;
                default:
                    throw new NotSupportedException();
            }
        }

        #region Properties

        private AxisPlacement placement = AxisPlacement.Bottom;
        /// <summary>
        /// Gets or sets the placement of axis control.
        /// Relative positioning of parts of axis depends on this value.
        /// </summary>
        /// <value>The placement.</value>
        public AxisPlacement Placement
        {
            get { return placement; }
            set
            {
                if (placement != value)
                {
                    placement = value;
                    UpdateUIResources();
                    UpdateSizeGetters();
                }
            }
        }

        private void UpdateSizeGetters()
        {
            switch (placement)
            {
                case AxisPlacement.Left:
                case AxisPlacement.Right:
                    getSize = size => size.Height;
                    getCoordinate = p => p.Y;
                    createScreenPoint1 = d => new Point(scrCoord1, d);
                    createScreenPoint2 = (d, size) => new Point(scrCoord2 * size, d);
                    break;
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:
                    getSize = size => size.Width;
                    getCoordinate = p => p.X;
                    createScreenPoint1 = d => new Point(d, scrCoord1);
                    createScreenPoint2 = (d, size) => new Point(d, scrCoord2 * size);
                    break;
                default:
                    break;
            }

            switch (placement)
            {
                case AxisPlacement.Left:
                    createDataPoint = d => new Point(0, d);
                    break;
                case AxisPlacement.Right:
                    createDataPoint = d => new Point(1, d);
                    break;
                case AxisPlacement.Top:
                    createDataPoint = d => new Point(d, 1);
                    break;
                case AxisPlacement.Bottom:
                    createDataPoint = d => new Point(d, 0);
                    break;
                default:
                    break;
            }
        }

        private void UpdateUIResources()
        {
            ResourceDictionary resources = new ResourceDictionary
            {
                Source = new Uri("/DynamicDataDisplay;component/Charts/Axes/AxisControlStyle.xaml", UriKind.Relative)
            };

            AxisPlacement placement = GetBetterPlacement(this.placement);
            ControlTemplate template = (ControlTemplate)resources[templateKey + placement.ToString()];
            Verify.AssertNotNull(template);
            var content = (FrameworkElement)template.LoadContent();

            if (ticksPath != null && ticksPath.Data != null)
            {
                GeometryGroup group = (GeometryGroup)ticksPath.Data;
                foreach (var child in group.Children)
                {
                    LineGeometry geometry = (LineGeometry)child;
                    lineGeomPool.Put(geometry);
                }
                group.Children.Clear();
            }

            ticksPath = (Path)content.FindName(PART_TicksPath);
            ticksPath.SnapsToDevicePixels = true;
            Verify.AssertNotNull(ticksPath);

            // as this method can be called not only on loading of axisControl, but when its placement changes, internal panels
            // can be not empty and their contents should be released
            if (commonLabelsCanvas != null && labelProvider != null)
            {
                foreach (UIElement child in commonLabelsCanvas.Children)
                {
                    if (child != null)
                    {
                        labelProvider.ReleaseLabel(child);
                    }
                }

                labels = null;
                commonLabelsCanvas.Children.Clear();
            }

            commonLabelsCanvas = (StackCanvas)content.FindName(PART_CommonLabelsCanvas);
            Verify.AssertNotNull(commonLabelsCanvas);
            commonLabelsCanvas.Placement = placement;

            if (additionalLabelsCanvas != null && majorLabelProvider != null)
            {
                foreach (UIElement child in additionalLabelsCanvas.Children)
                {
                    if (child != null)
                    {
                        majorLabelProvider.ReleaseLabel(child);
                    }
                }
            }

            additionalLabelsCanvas = (StackCanvas)content.FindName(PART_AdditionalLabelsCanvas);
            Verify.AssertNotNull(additionalLabelsCanvas);
            additionalLabelsCanvas.Placement = placement;

            mainGrid = (Grid)content.FindName(PART_ContentsGrid);
            Verify.AssertNotNull(mainGrid);

            mainGrid.SetBinding(Control.BackgroundProperty, new Binding { Path = new PropertyPath("Background"), Source = this });
            mainGrid.SizeChanged += new SizeChangedEventHandler(mainGrid_SizeChanged);

            Content = mainGrid;

            string transformKey = additionalLabelTransformKey + placement.ToString();
            if (resources.Contains(transformKey))
            {
                additionalLabelTransform = (Transform)resources[transformKey];
            }
        }

        void mainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (placement.IsBottomOrTop() && e.WidthChanged ||
               e.HeightChanged)
            {
                // this is performed because if not, whole axisControl's size was measured wrongly.
                InvalidateMeasure();
				UpdateUI();
            }
        }

        private bool updateOnCommonChange = true;

        internal IDisposable OpenUpdateRegion(bool forceUpdate)
        {
            return new UpdateRegionHolder<T>(this, forceUpdate);
        }

        private sealed class UpdateRegionHolder<TT> : IDisposable
        {
            private Range<TT> prevRange;
            private CoordinateTransform prevTransform;
            private AxisControl<TT> owner;
            private bool forceUpdate = false;

            public UpdateRegionHolder(AxisControl<TT> owner) : this(owner, false) { }

            public UpdateRegionHolder(AxisControl<TT> owner, bool forceUpdate)
            {
                this.owner = owner;
                owner.updateOnCommonChange = false;

                prevTransform = owner.transform;
                prevRange = owner.range;
                this.forceUpdate = forceUpdate;
            }

            #region IDisposable Members

            public void Dispose()
            {
                owner.updateOnCommonChange = true;

                bool shouldUpdate = owner.range != prevRange;

                var screenRect = owner.Transform.ScreenRect;
                var prevScreenRect = prevTransform.ScreenRect;
                if (owner.placement.IsBottomOrTop())
                {
                    shouldUpdate |= prevScreenRect.Width != screenRect.Width;
                }
                else
                {
                    shouldUpdate |= prevScreenRect.Height != screenRect.Height;
                }

                shouldUpdate |= owner.transform.DataTransform != prevTransform.DataTransform;
                shouldUpdate |= forceUpdate;

                if (shouldUpdate)
                {
                    owner.UpdateUI();
                }
                owner = null;
            }

            #endregion
        }

        private Range<T> range;
        /// <summary>
        /// Gets or sets the range, which ticks are generated for.
        /// </summary>
        /// <value>The range.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Range<T> Range
        {
            get { return range; }
            set
            {
                range = value;
                if (updateOnCommonChange)
                {
                    UpdateUI();
                }
            }
        }

        private bool drawMinorTicks = true;
        /// <summary>
        /// Gets or sets a value indicating whether to show minor ticks.
        /// </summary>
        /// <value><c>true</c> if show minor ticks; otherwise, <c>false</c>.</value>
        public bool DrawMinorTicks
        {
            get { return drawMinorTicks; }
            set
            {
                if (drawMinorTicks != value)
                {
                    drawMinorTicks = value;
                    UpdateUI();
                }
            }
        }

        private bool drawMajorLabels = true;
        /// <summary>
        /// Gets or sets a value indicating whether to show major labels.
        /// </summary>
        /// <value><c>true</c> if show major labels; otherwise, <c>false</c>.</value>
        public bool DrawMajorLabels
        {
            get { return drawMajorLabels; }
            set
            {
                if (drawMajorLabels != value)
                {
                    drawMajorLabels = value;
                    UpdateUI();
                }
            }
        }

        private bool drawTicks = true;
        public bool DrawTicks
        {
            get { return drawTicks; }
            set
            {
                if (drawTicks != value)
                {
                    drawTicks = value;
                    UpdateUI();
                }
            }
        }

        #region TicksProvider

        private ITicksProvider<T> ticksProvider;
        /// <summary>
        /// Gets or sets the ticks provider - generator of ticks for given range.
        /// 
        /// Should not be null.
        /// </summary>
        /// <value>The ticks provider.</value>
        public ITicksProvider<T> TicksProvider
        {
            get { return ticksProvider; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (ticksProvider != value)
                {
                    DetachTicksProvider();

                    ticksProvider = value;

                    AttachTicksProvider();

                    UpdateUI();
                }
            }
        }

        private void AttachTicksProvider()
        {
            if (ticksProvider != null)
            {
                ticksProvider.Changed += ticksProvider_Changed;
            }
        }

        private void ticksProvider_Changed(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void DetachTicksProvider()
        {
            if (ticksProvider != null)
            {
                ticksProvider.Changed -= ticksProvider_Changed;
            }
        }

        #endregion

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool ShouldSerializeContent()
        {
            return false;
        }

        protected override bool ShouldSerializeProperty(DependencyProperty dp)
        {
            // do not serialize template - for XAML serialization
            if (dp == TemplateProperty) return false;

            return base.ShouldSerializeProperty(dp);
        }

        #region MajorLabelProvider

        private LabelProviderBase<T> majorLabelProvider;
        /// <summary>
        /// Gets or sets the major label provider, which creates labels for major ticks.
        /// If null, major labels will not be shown.
        /// </summary>
        /// <value>The major label provider.</value>
        public LabelProviderBase<T> MajorLabelProvider
        {
            get { return majorLabelProvider; }
            set
            {
                if (majorLabelProvider != value)
                {
                    DetachMajorLabelProvider();

                    majorLabelProvider = value;

                    AttachMajorLabelProvider();

                    UpdateUI();
                }
            }
        }

        private void AttachMajorLabelProvider()
        {
            if (majorLabelProvider != null)
            {
                majorLabelProvider.Changed += majorLabelProvider_Changed;
            }
        }

        private void majorLabelProvider_Changed(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void DetachMajorLabelProvider()
        {
            if (majorLabelProvider != null)
            {
                majorLabelProvider.Changed -= majorLabelProvider_Changed;
            }
        }

        #endregion

        #region LabelProvider

        private LabelProviderBase<T> labelProvider;
        /// <summary>
        /// Gets or sets the label provider, which generates labels for axis ticks.
        /// Should not be null.
        /// </summary>
        /// <value>The label provider.</value>
        [NotNull]
        public LabelProviderBase<T> LabelProvider
        {
            get { return labelProvider; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (labelProvider != value)
                {
                    DetachLabelProvider();

                    labelProvider = value;

                    AttachLabelProvider();

                    UpdateUI();
                }
            }
        }

        private void AttachLabelProvider()
        {
            if (labelProvider != null)
            {
                labelProvider.Changed += labelProvider_Changed;
            }
        }

        private void labelProvider_Changed(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void DetachLabelProvider()
        {
            if (labelProvider != null)
            {
                labelProvider.Changed -= labelProvider_Changed;
            }
        }

        #endregion

        private CoordinateTransform transform = CoordinateTransform.CreateDefault();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CoordinateTransform Transform
        {
            get { return transform; }
            set
            {
                transform = value;
                if (updateOnCommonChange)
                {
                    UpdateUI();
                }
            }
        }

        #endregion

        private const double defaultSmallerSize = 1;
        private const double defaultBiggerSize = 150;
        protected override Size MeasureOverride(Size constraint)
        {
			var baseSize = base.MeasureOverride(constraint);

            mainGrid.Measure(constraint);
            Size gridSize = mainGrid.DesiredSize;
            Size result = gridSize;

            bool isHorizontal = placement == AxisPlacement.Bottom || placement == AxisPlacement.Top;
            if (Double.IsInfinity(constraint.Width) && isHorizontal)
            {
                result = new Size(defaultBiggerSize, gridSize.Height != 0 ? gridSize.Height : defaultSmallerSize);
            }
            else if (Double.IsInfinity(constraint.Height) && !isHorizontal)
            {
                result = new Size(gridSize.Width != 0 ? gridSize.Width : defaultSmallerSize, defaultBiggerSize);
            }

            return result;
        }

        //protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        //{
        //    base.OnRenderSizeChanged(sizeInfo);

        //    bool isHorizontal = placement == AxisPlacement.Top || placement == AxisPlacement.Bottom;
        //    if (isHorizontal && sizeInfo.WidthChanged || !isHorizontal && sizeInfo.HeightChanged)
        //    {
        //        UpdateUIRepresentation();
        //    }
        //}

        private void InitTransform(Size newRenderSize)
        {
            Rect dataRect = CreateDataRect();

            transform = transform.WithRects(dataRect, new Rect(newRenderSize));
        }

        private Rect CreateDataRect()
        {
            double min = convertToDouble(range.Min);
            double max = convertToDouble(range.Max);

            Rect dataRect;
            switch (placement)
            {
                case AxisPlacement.Left:
                case AxisPlacement.Right:
                    dataRect = new Rect(new Point(min, min), new Point(max, max));
                    break;
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:
                    dataRect = new Rect(new Point(min, min), new Point(max, max));
                    break;
                default:
                    throw new NotSupportedException();
            }
            return dataRect;
        }

        /// <summary>
        /// Gets the Path with ticks strokes.
        /// </summary>
        /// <value>The ticks path.</value>
        public Path TicksPath
        {
            get { return ticksPath; }
        }

        private Grid mainGrid;
        private StackCanvas additionalLabelsCanvas;
        private StackCanvas commonLabelsCanvas;
        private Path ticksPath;
        private bool rendered = false;
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (!rendered)
            {
                UpdateUI();
            }
            rendered = true;
        }

        private bool independent = true;

        private double scrCoord1 = 0; // px
        private double scrCoord2 = 10; // px
        /// <summary>
        /// Gets or sets the size of main axis ticks.
        /// </summary>
        /// <value>The size of the tick.</value>
        public double TickSize
        {
            get { return scrCoord2; }
            set
            {
                if (scrCoord2 != value)
                {
                    scrCoord2 = value;
                    UpdateUI();
                }
            }
        }

        private GeometryGroup geomGroup = new GeometryGroup();
        internal void UpdateUI()
        {
            if (range.IsEmpty)
                return;

            if (transform == null) return;

            if (independent)
            {
                InitTransform(RenderSize);
            }

            bool isHorizontal = Placement == AxisPlacement.Bottom || Placement == AxisPlacement.Top;
            if (transform.ScreenRect.Width == 0 && isHorizontal
                || transform.ScreenRect.Height == 0 && !isHorizontal)
                return;

            if (!IsMeasureValid)
            {
                InvalidateMeasure();
            }

            CreateTicks();

            // removing unfinite screen ticks
            var tempTicks = new List<T>(ticks);
            var tempScreenTicks = new List<double>(ticks.Length);
            var tempLabels = new List<UIElement>(labels);

            int i = 0;
            while (i < tempTicks.Count)
            {
                T tick = tempTicks[i];
                double screenTick = getCoordinate(createDataPoint(convertToDouble(tick)).DataToScreen(transform));
                if (screenTick.IsFinite())
                {
                    tempScreenTicks.Add(screenTick);
                    i++;
                }
                else
                {
                    tempTicks.RemoveAt(i);
                    tempLabels.RemoveAt(i);
                }
            }

            ticks = tempTicks.ToArray();
            screenTicks = tempScreenTicks.ToArray();
            labels = tempLabels.ToArray();

            // saving generated lines into pool
            for (i = 0; i < geomGroup.Children.Count; i++)
            {
                var geometry = (LineGeometry)geomGroup.Children[i];
                lineGeomPool.Put(geometry);
            }

            geomGroup = new GeometryGroup();
            geomGroup.Children = new GeometryCollection(lineGeomPool.Count);

            if (drawTicks)
                DoDrawTicks(screenTicks, geomGroup.Children);

            if (drawMinorTicks)
                DoDrawMinorTicks(geomGroup.Children);

            ticksPath.Data = geomGroup;

            DoDrawCommonLabels(screenTicks);

            if (drawMajorLabels)
                DoDrawMajorLabels();

            ScreenTicksChanged.Raise(this);
        }

        bool drawTicksOnEmptyLabel = false;
        /// <summary>
        /// Gets or sets a value indicating whether to draw ticks on empty label.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if draw ticks on empty label; otherwise, <c>false</c>.
        /// </value>
        public bool DrawTicksOnEmptyLabel
        {
            get { return drawTicksOnEmptyLabel; }
            set
            {
                if (drawTicksOnEmptyLabel != value)
                {
                    drawTicksOnEmptyLabel = value;
                    UpdateUI();
                }
            }
        }

        private readonly ResourcePool<LineGeometry> lineGeomPool = new ResourcePool<LineGeometry>();
        private void DoDrawTicks(double[] screenTicksX, ICollection<Geometry> lines)
        {
            for (int i = 0; i < screenTicksX.Length; i++)
            {
                if (labels[i] == null && !drawTicksOnEmptyLabel)
                    continue;

                Point p1 = createScreenPoint1(screenTicksX[i]);
                Point p2 = createScreenPoint2(screenTicksX[i], 1);

                LineGeometry line = lineGeomPool.GetOrCreate();

                line.StartPoint = p1;
                line.EndPoint = p2;
                lines.Add(line);
            }
        }

        private double GetRangesRatio(Range<T> nominator, Range<T> denominator)
        {
            double nomMin = ConvertToDouble(nominator.Min);
            double nomMax = ConvertToDouble(nominator.Max);
            double denMin = ConvertToDouble(denominator.Min);
            double denMax = ConvertToDouble(denominator.Max);

            return (nomMax - nomMin) / (denMax - denMin);
        }

        Transform additionalLabelTransform = null;
        private void DoDrawMajorLabels()
        {
            ITicksProvider<T> majorTicksProvider = ticksProvider.MajorProvider;
            additionalLabelsCanvas.Children.Clear();

            if (majorTicksProvider != null && majorLabelProvider != null)
            {
                additionalLabelsCanvas.Visibility = Visibility.Visible;

                Size renderSize = RenderSize;
                var majorTicks = majorTicksProvider.GetTicks(range, DefaultTicksProvider.DefaultTicksCount);

                double[] screenCoords = majorTicks.Ticks.Select(tick => createDataPoint(convertToDouble(tick))).
                    Select(p => p.DataToScreen(transform)).Select(p => getCoordinate(p)).ToArray();

                // todo this is not the best decision - when displaying, for example,
                // milliseconds, it causes to create hundreds and thousands of textBlocks.
                double rangesRatio = GetRangesRatio(majorTicks.Ticks.GetPairs().ToArray()[0], range);

                object info = majorTicks.Info;
                MajorLabelsInfo newInfo = new MajorLabelsInfo
                {
                    Info = info,
                    MajorLabelsCount = (int)Math.Ceiling(rangesRatio)
                };

                var newMajorTicks = new TicksInfo<T>
                {
                    Info = newInfo,
                    Ticks = majorTicks.Ticks,
                    TickSizes = majorTicks.TickSizes
                };

                UIElement[] additionalLabels = MajorLabelProvider.CreateLabels(newMajorTicks);

                for (int i = 0; i < additionalLabels.Length; i++)
                {
                    if (screenCoords[i].IsNaN())
                        continue;

                    UIElement tickLabel = additionalLabels[i];

                    tickLabel.Measure(renderSize);

                    StackCanvas.SetCoordinate(tickLabel, screenCoords[i]);
                    StackCanvas.SetEndCoordinate(tickLabel, screenCoords[i + 1]);

                    if (tickLabel is FrameworkElement)
                        ((FrameworkElement)tickLabel).LayoutTransform = additionalLabelTransform;

                    additionalLabelsCanvas.Children.Add(tickLabel);
                }
            }
            else
            {
                additionalLabelsCanvas.Visibility = Visibility.Collapsed;
            }
        }

        private int prevMinorTicksCount = DefaultTicksProvider.DefaultTicksCount;
        private const int maxTickArrangeIterations = 12;
        private void DoDrawMinorTicks(ICollection<Geometry> lines)
        {
            ITicksProvider<T> minorTicksProvider = ticksProvider.MinorProvider;
            if (minorTicksProvider != null)
            {
                int minorTicksCount = prevMinorTicksCount;
                int prevActualTicksCount = -1;
                ITicksInfo<T> minorTicks;
                TickCountChange result = TickCountChange.OK;
                TickCountChange prevResult;
                int iteration = 0;
                do
                {
                    Verify.IsTrue(++iteration < maxTickArrangeIterations);

                    minorTicks = minorTicksProvider.GetTicks(range, minorTicksCount);

                    prevActualTicksCount = minorTicks.Ticks.Length;
                    prevResult = result;
                    result = CheckMinorTicksArrangement(minorTicks);
                    if (prevResult == TickCountChange.Decrease && result == TickCountChange.Increase)
                    {
                        // stop tick number oscillating
                        result = TickCountChange.OK;
                    }
                    if (result == TickCountChange.Decrease)
                    {
                        int newMinorTicksCount = minorTicksProvider.DecreaseTickCount(minorTicksCount);
                        if (newMinorTicksCount == minorTicksCount)
                        {
                            result = TickCountChange.OK;
                        }
                        minorTicksCount = newMinorTicksCount;
                    }
                    else if (result == TickCountChange.Increase)
                    {
                        int newCount = minorTicksProvider.IncreaseTickCount(minorTicksCount);
                        if (newCount == minorTicksCount)
                        {
                            result = TickCountChange.OK;
                        }
                        minorTicksCount = newCount;
                    }

                } while (result != TickCountChange.OK);
                prevMinorTicksCount = minorTicksCount;

                double[] sizes = minorTicks.TickSizes;

                double[] screenCoords = minorTicks.Ticks.Select(
                    coord => getCoordinate(createDataPoint(convertToDouble(coord)).
                        DataToScreen(transform))).ToArray();

                minorScreenTicks = new MinorTickInfo<double>[screenCoords.Length];
                for (int i = 0; i < screenCoords.Length; i++)
                {
                    minorScreenTicks[i] = new MinorTickInfo<double>(sizes[i], screenCoords[i]);
                }

                for (int i = 0; i < screenCoords.Length; i++)
                {
                    double screenCoord = screenCoords[i];

                    Point p1 = createScreenPoint1(screenCoord);
                    Point p2 = createScreenPoint2(screenCoord, sizes[i]);

                    LineGeometry line = lineGeomPool.GetOrCreate();
                    line.StartPoint = p1;
                    line.EndPoint = p2;

                    lines.Add(line);
                }
            }
        }

        private TickCountChange CheckMinorTicksArrangement(ITicksInfo<T> minorTicks)
        {
            Size renderSize = RenderSize;
            TickCountChange result = TickCountChange.OK;
            if (minorTicks.Ticks.Length * 3 > getSize(renderSize))
                result = TickCountChange.Decrease;
            else if (minorTicks.Ticks.Length * 6 < getSize(renderSize))
                result = TickCountChange.Increase;
            return result;
        }

        private bool isStaticAxis = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is a static axis.
        /// If axis is static, its labels from sides are shifted so that they are not clipped by axis bounds.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is static axis; otherwise, <c>false</c>.
        /// </value>
        public bool IsStaticAxis
        {
            get { return isStaticAxis; }
            set
            {
                if (isStaticAxis != value)
                {
                    isStaticAxis = value;
                    UpdateUI();
                }
            }
        }

        private double ToScreen(T value)
        {
            return getCoordinate(createDataPoint(convertToDouble(value)).DataToScreen(transform));
        }

        private double staticAxisMargin = 1; // px

        private void DoDrawCommonLabels(double[] screenTicksX)
        {
            Size renderSize = RenderSize;

            commonLabelsCanvas.Children.Clear();

#if DEBUG
            if (labels != null)
            {
                foreach (FrameworkElement item in labels)
                {
                    if (item != null)
                        Debug.Assert(item.Parent == null);
                }
            }
#endif

            double minCoordUnsorted = ToScreen(range.Min);
            double maxCoordUnsorted = ToScreen(range.Max);

            double minCoord = Math.Min(minCoordUnsorted, maxCoordUnsorted);
            double maxCoord = Math.Max(minCoordUnsorted, maxCoordUnsorted);

            double maxCoordDiff = (maxCoord - minCoord) / labels.Length / 2.0;

            double minCoordToAdd = minCoord - maxCoordDiff;
            double maxCoordToAdd = maxCoord + maxCoordDiff;

            for (int i = 0; i < ticks.Length; i++)
            {
                FrameworkElement tickLabel = (FrameworkElement)labels[i];
                if (tickLabel == null) continue;

                Debug.Assert(((FrameworkElement)tickLabel).Parent == null);

                tickLabel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                double screenX = screenTicksX[i];
                double coord = screenX;

                tickLabel.HorizontalAlignment = HorizontalAlignment.Center;
                tickLabel.VerticalAlignment = VerticalAlignment.Center;

                if (isStaticAxis)
                {
                    // getting real size of label
                    tickLabel.Measure(renderSize);
                    Size tickLabelSize = tickLabel.DesiredSize;

                    if (Math.Abs(screenX - minCoord) < maxCoordDiff)
                    {
                        coord = minCoord + staticAxisMargin;
                        if (placement.IsBottomOrTop())
                            tickLabel.HorizontalAlignment = HorizontalAlignment.Left;
                        else
                            tickLabel.VerticalAlignment = VerticalAlignment.Top;
                    }
                    else if (Math.Abs(screenX - maxCoord) < maxCoordDiff)
                    {
                        coord = maxCoord - getSize(tickLabelSize) / 2 - staticAxisMargin;
                        if (!placement.IsBottomOrTop())
                        {
                            tickLabel.VerticalAlignment = VerticalAlignment.Bottom;
                            coord = maxCoord - staticAxisMargin;
                        }
                    }
                }

                // label is out of visible area
                if (coord < minCoord || coord > maxCoord)
                {
                    continue;
                }

                if (coord.IsNaN())
                    continue;

                StackCanvas.SetCoordinate(tickLabel, coord);

                commonLabelsCanvas.Children.Add(tickLabel);
            }
        }

        private double GetCoordinateFromTick(T tick)
        {
            return getCoordinate(createDataPoint(convertToDouble(tick)).DataToScreen(transform));
        }

        private Func<T, double> convertToDouble;
        /// <summary>
        /// Gets or sets the convertion of tick to double.
        /// Should not be null.
        /// </summary>
        /// <value>The convert to double.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<T, double> ConvertToDouble
        {
            get { return convertToDouble; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                convertToDouble = value;
                UpdateUI();
            }
        }

        internal event EventHandler ScreenTicksChanged;
        private double[] screenTicks;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double[] ScreenTicks
        {
            get { return screenTicks; }
        }

        private MinorTickInfo<double>[] minorScreenTicks;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MinorTickInfo<double>[] MinorScreenTicks
        {
            get { return minorScreenTicks; }
        }

        ITicksInfo<T> ticksInfo;
        private T[] ticks;
        private UIElement[] labels;
        private const double increaseRatio = 3.0;
        private const double decreaseRatio = 1.6;

        private Func<Size, double> getSize = size => size.Width;
        private Func<Point, double> getCoordinate = p => p.X;
        private Func<double, Point> createDataPoint = d => new Point(d, 0);

        private Func<double, Point> createScreenPoint1 = d => new Point(d, 0);
        private Func<double, double, Point> createScreenPoint2 = (d, size) => new Point(d, size);

        private int previousTickCount = DefaultTicksProvider.DefaultTicksCount;
        private void CreateTicks()
        {
            TickCountChange result = TickCountChange.OK;
            TickCountChange prevResult;

            int prevActualTickCount = -1;

            int tickCount = previousTickCount;
            int iteration = 0;

            do
            {
                Verify.IsTrue(++iteration < maxTickArrangeIterations);

                ticksInfo = ticksProvider.GetTicks(range, tickCount);
                ticks = ticksInfo.Ticks;

                if (ticks.Length == prevActualTickCount)
                {
                    result = TickCountChange.OK;
                    break;
                }

                prevActualTickCount = ticks.Length;

                if (labels != null)
                {
                    for (int i = 0; i < labels.Length; i++)
                    {
                        labelProvider.ReleaseLabel(labels[i]);
                    }
                }

                labels = labelProvider.CreateLabels(ticksInfo);

                prevResult = result;
                result = CheckLabelsArrangement(labels, ticks);

                if (prevResult == TickCountChange.Decrease && result == TickCountChange.Increase)
                {
                    // stop tick number oscillating
                    result = TickCountChange.OK;
                }

                if (result != TickCountChange.OK)
                {
                    int prevTickCount = tickCount;
                    if (result == TickCountChange.Decrease)
                        tickCount = ticksProvider.DecreaseTickCount(tickCount);
                    else
                    {
                        tickCount = ticksProvider.IncreaseTickCount(tickCount);
                        //DebugVerify.Is(tickCount >= prevTickCount);
                    }

                    // ticks provider could not create less ticks or tick number didn't change
                    if (tickCount == 0 || prevTickCount == tickCount)
                    {
                        tickCount = prevTickCount;
                        result = TickCountChange.OK;
                    }
                }
            } while (result != TickCountChange.OK);

            previousTickCount = tickCount;
        }

        private TickCountChange CheckLabelsArrangement(UIElement[] labels, T[] ticks)
        {
            var actualLabels = labels.Select((label, i) => new { Label = label, Index = i })
                .Where(el => el.Label != null)
                .Select(el => new { Label = el.Label, Tick = ticks[el.Index] })
                .ToList();

            actualLabels.ForEach(item => item.Label.Measure(RenderSize));

            var sizeInfos = actualLabels.Select(item =>
                new { X = GetCoordinateFromTick(item.Tick), Size = getSize(item.Label.DesiredSize) })
                .OrderBy(item => item.X).ToArray();

            TickCountChange res = TickCountChange.OK;

            int increaseCount = 0;
            for (int i = 0; i < sizeInfos.Length - 1; i++)
            {
                if ((sizeInfos[i].X + sizeInfos[i].Size * decreaseRatio) > sizeInfos[i + 1].X)
                {
                    res = TickCountChange.Decrease;
                    break;
                }
                if ((sizeInfos[i].X + sizeInfos[i].Size * increaseRatio) < sizeInfos[i + 1].X)
                {
                    increaseCount++;
                }
            }
            if (increaseCount > sizeInfos.Length / 2)
                res = TickCountChange.Increase;

            return res;
        }
    }

    [DebuggerDisplay("{X} + {Size}")]
    internal sealed class SizeInfo : IComparable<SizeInfo>
    {
        public double Size { get; set; }
        public double X { get; set; }


        public int CompareTo(SizeInfo other)
        {
            return X.CompareTo(other.X);
        }
    }

    internal enum TickCountChange
    {
        Increase = -1,
        OK = 0,
        Decrease = 1
    }

    /// <summary>
    /// Represents an auxiliary structure for storing additional info during major DateTime labels generation.
    /// </summary>
    public struct MajorLabelsInfo
    {
        public object Info { get; set; }
        public int MajorLabelsCount { get; set; }

        public override string ToString()
        {
            return String.Format("{0}, Count={1}", Info, MajorLabelsCount);
        }
    }
}
