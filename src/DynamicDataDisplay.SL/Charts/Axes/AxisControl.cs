using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.CoordinateTransforms;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public abstract partial class AxisControl<T> : ContentControl
    {
        protected Grid LayoutRoot;
        protected Path TicksPath;
        protected StackCanvas CommonLabelsCanvas;
        protected StackCanvas AdditionalLabelsCanvas;
        protected TranslateTransform AdditionalAxisTranslate;
        bool partsLoaded = false;
        private bool independent = true;
        ITicksInfo<T> ticksInfo;
        private T[] ticks;
        private List<FrameworkElement> labels;
        private const double increaseRatio = 3.0;
        private const double decreaseRatio = 1.6;
        private const int maxTickArrangeIterations = 12;
        bool drawTicksOnEmptyLabel = false;

        private double scrCoord1 = 0; // px
        private double scrCoord2 = 10; // px

        private Func<Size, double> getSize = size => size.Width;
        private Func<Point, double> getCoordinate = p => p.X;
        private Func<double, Point> createDataPoint = d => new Point(d, 0);
        private Func<double, Point> createScreenPoint1 = d => new Point(d, 0);
        private Func<double, double, Point> createScreenPoint2 = (d, size) => new Point(d, size);

        internal void MakeDependent()
        {
            independent = false;
        }

        protected AxisControl()
        {
            InitializeComponent();
            geomGroup = new GeometryGroup();
            TicksPath.Data = geomGroup;
            UpdateUILayout();
            UpdateSizeGetters();
            SizeChanged += new SizeChangedEventHandler(AxisControl_SizeChanged);
            IsHitTestVisible = false;
        }

        void AxisControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
            AdditionalAxisTranslate.X = ticksInfo.Ticks.Length/2;
            UpdateUIRepresentation();
        }

        private GeometryGroup geomGroup = null;

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


        /*protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (!rendered)
            {
                UpdateUIRepresentation();
            }
            rendered = true;
        }*/

        private void InitializeComponent()
        {
            LayoutRoot = new Grid();
            LayoutRoot.Background = new SolidColorBrush(Colors.White);
            TicksPath = new Path();
            TicksPath.Stroke = new SolidColorBrush(Colors.Black);
            CommonLabelsCanvas = new StackCanvas();
            AdditionalLabelsCanvas = new StackCanvas();
            AdditionalAxisTranslate = new TranslateTransform();
            LayoutRoot.Children.Add(TicksPath);
            LayoutRoot.Children.Add(CommonLabelsCanvas);
            LayoutRoot.Children.Add(AdditionalLabelsCanvas);
            this.Content = LayoutRoot;
        }

        private void UpdateUILayout()
        {
            CommonLabelsCanvas.Placement = placement;
            AdditionalLabelsCanvas.Placement = placement;
            //LayoutRoot.Background = new SolidColorBrush(Colors.Yellow);
            //CommonLabelsCanvas.Background = new SolidColorBrush(Color.FromArgb(200,255,200,200));
            //AdditionalLabelsCanvas.Background = new SolidColorBrush(Color.FromArgb(200, 255, 100, 100));
            LayoutRoot.ColumnDefinitions.Clear(); //TODO Do we need to clear/add children?
            LayoutRoot.RowDefinitions.Clear();
            LayoutRoot.Children.Clear();
            LayoutRoot.Margin = new Thickness(0.0);
            if (placement == AxisPlacement.Bottom || placement == AxisPlacement.Top)
            {
                LayoutRoot.RowDefinitions.Add(new RowDefinition());
                LayoutRoot.RowDefinitions.Add(new RowDefinition());
                LayoutRoot.RowDefinitions.Add(new RowDefinition());
            }
            else
            {
                LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
                LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
                LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            }
            switch (placement)
            {
                case AxisPlacement.Bottom:
                    Grid.SetRow(TicksPath, 0);
                    Grid.SetRow(CommonLabelsCanvas, 1);
                    Grid.SetRow(AdditionalLabelsCanvas, 2);
                    break;

                case AxisPlacement.Top:
                    Grid.SetRow(TicksPath, 2);
                    //TODO matrixTranform TickPath
                    Grid.SetRow(CommonLabelsCanvas, 1);
                    Grid.SetRow(AdditionalLabelsCanvas, 0);
                    break;

                case AxisPlacement.Left:
                    MatrixTransform reflection = new MatrixTransform();
                    reflection.Matrix = new Matrix(-1, 0, 0, 1, 0, 0);
                    TicksPath.RenderTransformOrigin = new Point(0.5, 0.5);
                    TicksPath.RenderTransform = reflection;
                    Grid.SetColumn(TicksPath, 2);
                    Grid.SetColumn(CommonLabelsCanvas, 1);
                    Grid.SetColumn(AdditionalLabelsCanvas, 0);
                    TicksPath.Margin = new Thickness(0, 0,0, 0);
                    CommonLabelsCanvas.Margin = new Thickness(1, 0, 1, 0);
                    AdditionalLabelsCanvas.Margin = new Thickness(1, 0, 1, 0);
                    break;

                case AxisPlacement.Right:
                    Grid.SetColumn(TicksPath, 0);
                    Grid.SetColumn(CommonLabelsCanvas, 1);
                    Grid.SetColumn(AdditionalLabelsCanvas, 2);
                    CommonLabelsCanvas.Margin = new Thickness(1, 0, 0, 0);
                    break;
            }
            LayoutRoot.Children.Add(TicksPath);
            LayoutRoot.Children.Add(CommonLabelsCanvas);
            LayoutRoot.Children.Add(AdditionalLabelsCanvas);
            partsLoaded = true;
        }

        private static AxisPlacement GetBetterPlacement(AxisPlacement placement)
        {
            switch (placement)
            {
                case AxisPlacement.Left:
                    return AxisPlacement.Left;
                case AxisPlacement.Right:
                    return AxisPlacement.Right;
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:
                    return AxisPlacement.Top;
                default:
                    throw new NotSupportedException();
            }
        }

        private AxisPlacement placement = AxisPlacement.Bottom;
        public AxisPlacement Placement
        {
            get { return placement; }
            set
            {
                if (placement != value)
                {
                    placement = value;
                    UpdateUILayout();
                    UpdateSizeGetters();
                }
            }
        }

        private Range<T> range;
        public Range<T> Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
                UpdateUIRepresentation();
            }
        }

        private bool drawMinorTicks = true;
        public bool DrawMinorTicks
        {
            get
            {
                return drawMinorTicks;
            }
            set
            {
                if (drawMinorTicks != value)
                {
                    drawMinorTicks = value;
                    UpdateUIRepresentation();
                }
            }
        }

        private bool drawMayorLabels = true;
        public bool DrawMayorLabels
        {
            get { return drawMayorLabels; }
            set
            {
                if (drawMayorLabels != value)
                {
                    drawMayorLabels = value;
                    UpdateUIRepresentation();
                }
            }
        }

        private ITicksProvider<T> ticksProvider;
        public ITicksProvider<T> TicksProvider
        {
            get { return ticksProvider; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (ticksProvider != value)
                {
                    ticksProvider = value;
                    UpdateUIRepresentation();
                }
            }
        }

        private LabelProviderBase<T> mayorLabelProvider;
        /// <summary>
        /// Gets or sets the mayor label provider, which creates labels for mayor ticks.
        /// If null, mayor labels will not be shown.
        /// </summary>
        /// <value>The mayor label provider.</value>
        public LabelProviderBase<T> MayorLabelProvider
        {
            get { return mayorLabelProvider; }
            set
            {
                if (mayorLabelProvider != value)
                {
                    DetachMayorLabelProvider();

                    mayorLabelProvider = value;

                    AttachMayorLabelProvider();

                    UpdateUIRepresentation();
                }
            }
        }

        private void DetachMayorLabelProvider()
        {
            if (mayorLabelProvider != null)
            {
                mayorLabelProvider.Changed -= mayorLabelProvider_Changed;
            }
        }

        private void AttachMayorLabelProvider()
        {
            if (mayorLabelProvider != null)
            {
                mayorLabelProvider.Changed += mayorLabelProvider_Changed;
            }
        }

        private void mayorLabelProvider_Changed(object sender, EventArgs e)
        {
            UpdateUIRepresentation();
        }

        private LabelProviderBase<T> labelProvider;
        /// <summary>
        /// Gets or sets the label provider, which generates labels for axis ticks.
        /// Should not be null.
        /// </summary>
        /// <value>The label provider.</value>
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

                    UpdateUIRepresentation();
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

        private void DetachLabelProvider()
        {
            if (labelProvider != null)
            {
                labelProvider.Changed -= labelProvider_Changed;
            }
        }

        private void labelProvider_Changed(object sender, EventArgs e)
        {
            UpdateUIRepresentation();
        }

        
      

        private ICoordinateConverter converter = new DefaultCoordinateConverter();
        public ICoordinateConverter Converter
        {
            get { return converter; }
            set { converter = value; }
        }

        private Func<T, double> convertToDouble;
        public Func<T, double> ConvertToDouble
        {
            get { return convertToDouble; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                convertToDouble = value;
            }
        }

        private bool updateOnCommonChange = true;

        internal IDisposable OpenUpdateRegion()
        {
            return new UpdateRegionHolder<T>(this);
        }

        private CoordinateTransform transform = CoordinateTransform.CreateDefault();

        public CoordinateTransform Transform
        {
            get { return transform; }
            set
            {
                transform = value;
                if (updateOnCommonChange)
                {
                    UpdateUIRepresentation();
                }
            }
        }

        private sealed class UpdateRegionHolder<TT> : IDisposable
        {
            private AxisControl<TT> owner;
            public UpdateRegionHolder(AxisControl<TT> owner)
            {
                this.owner = owner;
                owner.updateOnCommonChange = false;
            }

            #region IDisposable Members

            public void Dispose()
            {
                owner.updateOnCommonChange = true;
                owner.UpdateUIRepresentation();
                owner = null;
            }

            #endregion
        }

        private void InitConverter(Size newRenderSize)
        {
            Rect dataRect = CreateDataRect();
            converter.Init(new Rect(new Point(0,0),newRenderSize), dataRect);
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
                    Point p1 = new Point(min, min);
                    Point p2 = new Point(max, max);
                    dataRect = new Rect(p1,p2);
                    break;
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:
                    dataRect = new Rect(min, min, max - min, max - min);
                    break;
                default:
                    throw new NotSupportedException();
            }
            return dataRect;
        }

        public void UpdateUIRepresentation()
        {
            if (range.IsEmpty)
                return;
            if (!partsLoaded)
                return;

            usedTicks = 0;

            if (independent)
            {
                InitConverter(RenderSize);
            }
            PerformanceCounter.startStopwatch("Creating ticks");
            CreateTicks();
            PerformanceCounter.stopStopwatch("Creating ticks");

            Size renderSize = RenderSize;

            screenTicks = ticks.Select(tick => createDataPoint(convertToDouble(tick))).
                Select(p => p.ToScreen(converter)).Select(p => getCoordinate(p)).ToArray();

            
            PerformanceCounter.startStopwatch("Drawing ticks");
            DrawTicks(screenTicks, geomGroup);
            PerformanceCounter.stopStopwatch("Drawing ticks");

            PerformanceCounter.startStopwatch("Drawing minor ticks");
            if (drawMinorTicks)
                DoDrawMinorTicks(geomGroup);
            PerformanceCounter.stopStopwatch("Drawing minor ticks");

            
            PerformanceCounter.startStopwatch("Creating labels");
            CreateCommonLabels(screenTicks);
            PerformanceCounter.stopStopwatch("Creating labels");

            PerformanceCounter.startStopwatch("Drawing labels");
            if (drawMayorLabels)
                DoDrawMayorLabels();
            PerformanceCounter.stopStopwatch("Drawing labels");
            removeUnesedAllocatedTicks();
            
            allocatedTicks = usedTicks;

            //ScreenTicksChanged.Raise(this);
        }

        private void DoDrawMayorLabels()
        {
            AdditionalLabelsCanvas.Children.Clear();
            ITicksProvider<T> mayorTicksProvider = ticksProvider.MayorProvider;
            if (mayorTicksProvider != null && MayorLabelProvider != null)
            {
                Size renderSize = RenderSize;
                var mayorTicks = mayorTicksProvider.GetTicks(range, DefaultTicksProvider.DefaultTicksCount);

                double[] screenCoords = mayorTicks.Ticks.Select(tick => createDataPoint(convertToDouble(tick))).
                    Select(p => p.ToScreen(converter)).Select(p => getCoordinate(p)).ToArray();

                int expectedMayorLabelsCount = mayorTicks.Ticks.Length;
                // todo this is not the best decision - when displaying, for example,
                // milliseconds, it causes to create hundreds and thousands of textBlocks.
                double rangesRatio = GetRangesRatio(mayorTicks.Ticks.GetPairs().ToArray()[0], range);

                object info = mayorTicks.Info;
                MayorLabelsInfo newInfo = new MayorLabelsInfo
                {
                    Info = info,
                    MayorLabelsCount = (int)Math.Ceiling(rangesRatio)
                };

                var newMayorTicks = new TicksInfo<T>
                {
                    Info = newInfo,
                    Ticks = mayorTicks.Ticks,
                    TickSizes = mayorTicks.TickSizes
                };

                IEnumerable<FrameworkElement> additionalLabels = MayorLabelProvider.CreateLabels(newMayorTicks);

                int i = 0;
                foreach (FrameworkElement fe in additionalLabels)
                {
                    fe.Measure(renderSize);

                    StackCanvas.SetCoordinate(fe, screenCoords[i]);
                    StackCanvas.SetEndCoordinate(fe, screenCoords[i + 1]);

                    AdditionalLabelsCanvas.Children.Add(fe);
                    i++;
                }
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

        private void CreateCommonLabels(double[] screenTicksX)
        {
            Size renderSize = RenderSize;
            CommonLabelsCanvas.Children.Clear();
            List<FrameworkElement>.Enumerator labelsEnumerator = labels.GetEnumerator();
            //IEnumerator<UIElement> canvasEnumerator= CommonLabelsCanvas.Children.GetEnumerator();

            for (int i = 0; i < labels.Count;i++ )
            {
                FrameworkElement tickLabel = labels[i];

                Size tickLabelSize = new Size(tickLabel.ActualWidth, tickLabel.ActualHeight);

                StackCanvas.SetCoordinate(tickLabel, screenTicksX[i] - getSize(tickLabelSize) / 2);


                CommonLabelsCanvas.Children.Add(tickLabel);
                //if (i >= allocatedLabels)
                //{
                //    CommonLabelsCanvas.Children.Add(tickLabel);
                //}
                //else
                //{
                //    labelsEnumerator.MoveNext();
                //    i = CommonLabelsCanvas.Children.IndexOf(labelsEnumerator.Current);
                //    //labelsEnumerator.Current = tickLabel;
                //    //CommonLabelsCanvas.Children.
                //    CommonLabelsCanvas.Children[i] = tickLabel;

                //}
            }
            //for (int j = allocatedLabels-1; j >= ticks.Length; j--) {
            //    CommonLabelsCanvas.Children.RemoveAt(j);
            //}
            //allocatedLabels = CommonLabelsCanvas.Children.Count;
        }

        private TickChange CheckMinorTicksArrangement(ITicksInfo<T> minorTicks)
        {
            Size renderSize = RenderSize;
            TickChange result = TickChange.OK;
            if (minorTicks.Ticks.Length * 3 > getSize(renderSize))
                result = TickChange.Decrease;
            else if (minorTicks.Ticks.Length * 6 < getSize(renderSize))
                result = TickChange.Increase;
            return result;
        }

        private List<LineGeometry> allocatedTicksList = new List<LineGeometry>();

        private void DoDrawMinorTicks(GeometryGroup geomGroup)
        {
            ITicksProvider<T> minorTicksProvider = ticksProvider.MinorProvider;
            if (minorTicksProvider != null)
            {
                int minorTicksCount = DefaultTicksProvider.DefaultTicksCount;
                int prevActualTicksCount = -1;
                ITicksInfo<T> minorTicks;
                TickChange result = TickChange.OK;
                TickChange prevResult;
                int iteration = 0;
                //PerformanceCounter.startStopwatch("Minor ticks: computing tickCounts");
                do
                {
                    Verify.IsTrue(++iteration < maxTickArrangeIterations);

                    minorTicks = minorTicksProvider.GetTicks(range, minorTicksCount);

                    prevActualTicksCount = minorTicks.Ticks.Length;
                    prevResult = result;
                    result = CheckMinorTicksArrangement(minorTicks);
                    if (prevResult == TickChange.Decrease && result == TickChange.Increase)
                    {
                        // stop tick number oscillating
                        result = TickChange.OK;
                    }
                    if (result == TickChange.Decrease)
                    {
                        int newMinorTicksCount = minorTicksProvider.DecreaseTickCount(minorTicksCount);
                        if (newMinorTicksCount == minorTicksCount)
                        {
                            result = TickChange.OK;
                        }
                        minorTicksCount = newMinorTicksCount;
                    }
                    else if (result == TickChange.Increase)
                    {
                        int newCount = minorTicksProvider.IncreaseTickCount(minorTicksCount);
                        if (newCount == minorTicksCount)
                        {
                            result = TickChange.OK;
                        }
                        minorTicksCount = newCount;
                    }

                } while (result != TickChange.OK);
                //PerformanceCounter.stopStopwatch("Minor ticks: computing tickCounts");

                minorTicks = minorTicksProvider.GetTicks(range, minorTicksCount);


                double[] sizes = minorTicks.TickSizes;

                double[] screenCoords = minorTicks.Ticks.Select(
                    coord => getCoordinate(createDataPoint(convertToDouble(coord)).
                        ToScreen(converter))).ToArray();

                minorScreenTicks = new MinorTickInfo<double>[screenCoords.Length];
                for (int i = 0; i < screenCoords.Length; i++)
                {
                    minorScreenTicks[i] = new MinorTickInfo<double>(sizes[i], screenCoords[i]);
                }
                
                for (int i = 0; i < screenCoords.Length; i++) {
                    double screenCoord = screenCoords[i];
                    if (i + usedTicks < allocatedTicks)
                    {
                        //PerformanceCounter.startStopwatch("Minor ticks: getting allocated lines");
                        LineGeometry line = allocatedTicksList[i + usedTicks];
                        //PerformanceCounter.stopStopwatch("Minor ticks: getting allocated lines");
                        //PerformanceCounter.startStopwatch("Minor ticks: renewing allocated lines");
                        line.StartPoint = createScreenPoint1(screenCoord); ;
                        line.EndPoint = createScreenPoint2(screenCoord, sizes[i]);
                        //PerformanceCounter.stopStopwatch("Minor ticks: renewing allocated lines");
                        
                    }
                    else {
                        //PerformanceCounter.startStopwatch("Minor ticks: creating new lines");
                        LineGeometry line = new LineGeometry();
                        line.StartPoint = createScreenPoint1(screenCoord); ;
                        line.EndPoint = createScreenPoint2(screenCoord, sizes[i]);
                        geomGroup.Children.Add(line);
                        allocatedTicksList.Add(line);
                        //PerformanceCounter.stopStopwatch("Minor ticks: creating new lines");
                    }
                }
                usedTicks += screenCoords.Length;
            }
        }

        private void removeUnesedAllocatedTicks() {
            int removed = 0;
            for (int i = allocatedTicks-1; i >= usedTicks; i--)
            {
                geomGroup.Children.RemoveAt(i);               
                removed++;
            }
            if(allocatedTicks-usedTicks >0)
            allocatedTicksList.RemoveRange(usedTicks, allocatedTicks - usedTicks);
        }

        private MinorTickInfo<double>[] minorScreenTicks;
        public MinorTickInfo<double>[] MinorScreenTicks
        {
            get { return minorScreenTicks; }
        }


        public bool DrawTicksOnEmptyLabel
        {
            get { return drawTicksOnEmptyLabel; }
            // todo notify about change and redraw
            set { drawTicksOnEmptyLabel = value; }
        }

        private int allocatedTicks =0;
        private int usedTicks = 0;

        private void DrawTicks(double[] screenTicksX, GeometryGroup geomGroup)
        {
            for (int i = 0; i < screenTicksX.Length; i++)
            {
                if (i + usedTicks < allocatedTicks)
                {
                    LineGeometry line = allocatedTicksList[i + usedTicks]; ;
                    line.StartPoint = createScreenPoint1(screenTicksX[i]); ;
                    line.EndPoint = createScreenPoint2(screenTicksX[i], 1);                    
                }
                else
                {
                    LineGeometry line = new LineGeometry();
                    line.StartPoint = createScreenPoint1(screenTicksX[i]); ;
                    line.EndPoint = createScreenPoint2(screenTicksX[i], 1);
                    geomGroup.Children.Add(line);
                    allocatedTicksList.Add(line);
                }
            }
            //allocatedTicks = geomGroup.Children.Count;
            usedTicks += screenTicksX.Length;
        }

        private double GetCoordinateFromTick(T tick)
        {
            return getCoordinate(createDataPoint(convertToDouble(tick)).ToScreen(converter));
        }

        private TickChange CheckLabelsArrangement(IEnumerable<FrameworkElement> labels, T[] ticks)
        {
            var actualLabels = labels.Select((label, i) => new { Label = label, Index = i })
                .Where(el => el.Label != null)
                .Select(el => new { Label = el.Label, Tick = ticks[el.Index] })
                .ToList();

            actualLabels.ForEach(item => item.Label.Measure(RenderSize));

            var sizeInfos = actualLabels.Select(item =>
                new { X = GetCoordinateFromTick(item.Tick), Size = getSize(item.Label.DesiredSize) })
                .OrderBy(item => item.X).ToArray();

            TickChange res = TickChange.OK;

            int increaseCount = 0;
            for (int i = 0; i < sizeInfos.Length - 1; i++)
            {
                if ((sizeInfos[i].X + sizeInfos[i].Size * decreaseRatio) > sizeInfos[i + 1].X)
                {
                    res = TickChange.Decrease;
                    break;
                }
                if ((sizeInfos[i].X + sizeInfos[i].Size * increaseRatio) < sizeInfos[i + 1].X)
                {
                    increaseCount++;
                }
            }
            if (increaseCount > sizeInfos.Length / 2)
                res = TickChange.Increase;

            return res;
        }

        private void CreateTicks()
        {
            TickChange result = TickChange.OK;
            TickChange prevResult;

            int prevActualTickCount = -1;

            int tickCount = DefaultTicksProvider.DefaultTicksCount;
            int iteration = 0;
            do
            {
                Verify.IsTrue(++iteration < maxTickArrangeIterations);

                ticksInfo = ticksProvider.GetTicks(range, tickCount);
                ticks = ticksInfo.Ticks;

                if (ticks.Length == prevActualTickCount)
                {
                    result = TickChange.OK;
                    break;
                }

                prevActualTickCount = ticks.Length;

                labels = labelProvider.CreateLabels(ticksInfo);

                prevResult = result;
                result = CheckLabelsArrangement(labels, ticks);

                if (prevResult == TickChange.Decrease && result == TickChange.Increase)
                {
                    // stop tick number oscillating
                    result = TickChange.OK;
                }

                if (result != TickChange.OK)
                {
                    int prevTickCount = tickCount;
                    if (result == TickChange.Decrease)
                        tickCount = ticksProvider.DecreaseTickCount(tickCount);
                    else
                    {
                        tickCount = ticksProvider.IncreaseTickCount(tickCount);
                        DebugVerify.Is(tickCount >= prevTickCount);
                    }

                    // ticks provider cannot create less ticks or tick number didn't change
                    if (tickCount == 0 || prevTickCount == tickCount)
                    {
                        tickCount = prevTickCount;
                        result = TickChange.OK;
                    }
                }
            } while (result != TickChange.OK);
        }

        private double[] screenTicks;
        public double[] ScreenTicks
        {
            get { return screenTicks; }
        }
    }

    internal struct MayorLabelsInfo
    {
        public object Info { get; set; }
        public int MayorLabelsCount { get; set; }
    }

    internal enum TickChange
    {
        Increase = -1,
        OK = 0,
        Decrease = 1
    }
}