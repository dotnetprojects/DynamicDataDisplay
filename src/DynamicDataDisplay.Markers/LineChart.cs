//#define _geom

//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
//using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
//using Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Filters;
//using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
//using filters = Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Filters;
//using System.Windows.Data;

//namespace Microsoft.Research.DynamicDataDisplay.Charts
//{
//    public class LineChart : Control, IPlotterElement
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="LineChart"/> class.
//        /// </summary>
//        public LineChart()
//        {
//            Viewport2D.SetIsContentBoundsHost(this, true);

//            filters.CollectionChanged += new NotifyCollectionChangedEventHandler(filters_CollectionChanged);
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="LineChart"/> class with specified data source.
//        /// </summary>
//        /// <param name="dataSource">The data source.</param>
//        public LineChart(IPointDataSource dataSource)
//            : this()
//        {
//            DataSource = dataSource;
//        }

//        #region UI look properties

//        public Brush Stroke
//        {
//            get { return (Brush)GetValue(StrokeProperty); }
//            set { SetValue(StrokeProperty, value); }
//        }

//        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
//          "Stroke",
//          typeof(Brush),
//          typeof(LineChart),
//          new FrameworkPropertyMetadata(Brushes.Blue));

//        public double StrokeThickness
//        {
//            get { return (double)GetValue(StrokeThicknessProperty); }
//            set { SetValue(StrokeThicknessProperty, value); }
//        }

//        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
//          "StrokeThickness",
//          typeof(double),
//          typeof(LineChart),
//          new FrameworkPropertyMetadata(1.0));

//        #endregion

//        #region DataSource

//        public IPointDataSource DataSource
//        {
//            get { return (IPointDataSource)GetValue(DataSourceProperty); }
//            set { SetValue(DataSourceProperty, value); }
//        }

//        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
//          "DataSource",
//          typeof(IPointDataSource),
//          typeof(LineChart),
//          new FrameworkPropertyMetadata(null, OnDataSourceChanged));

//        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            LineChart chart = (LineChart)d;
//            chart.OnDataSourceChanged((IPointDataSource)e.OldValue, (IPointDataSource)e.NewValue);
//            chart.RaiseRoutedEvent(DataSourceChangedEvent);
//        }

//        private bool hasContinousDataSource = false;
//        private void OnDataSourceChanged(IPointDataSource oldDataSource, IPointDataSource currentDataSource)
//        {
//            if (oldDataSource != null)
//            {
//                oldDataSource.Changed -= OnDataSourceChangedEvent;
//                INotifyCollectionChanged observableCollection = oldDataSource as INotifyCollectionChanged;
//                if (observableCollection != null)
//                    observableCollection.CollectionChanged -= observableCollection_CollectionChanged;
//            }

//            if (currentDataSource != null)
//            {
//                currentDataSource.Changed += OnDataSourceChangedEvent;
//                INotifyCollectionChanged observableCollection = currentDataSource as INotifyCollectionChanged;
//                if (observableCollection != null)
//                    observableCollection.CollectionChanged += observableCollection_CollectionChanged;
//            }

//            DependencyObject oldDependencyDataSource = oldDataSource as DependencyObject;
//            DependencyObject currentDependencyDataSource = currentDataSource as DependencyObject;

//            hasContinousDataSource = currentDependencyDataSource != null;

//            if (oldDependencyDataSource != null && currentDependencyDataSource == null && currentDataSource != null)
//            {
//                Viewport2D.SetIsContentBoundsHost(this, true);
//            }
//            else if (oldDependencyDataSource == null && currentDependencyDataSource != null)
//            {
//                Viewport2D.SetIsContentBoundsHost(this, false);
//            }

//            UpdateUIRepresentation();
//        }

//        private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
//        {
//            bool shouldUpdate = hasContinousDataSource;

//            if (e.PropertyName == "Visible")
//            {
//                DataRect prevRect = visibleWhileUpdate;
//                DataRect currRect = (DataRect)e.NewValue;

//                // todo тут отсчет должен идти не от текущего старого значения, а от того, при котором была отрисована линия
//                const double maxPercent = 0.05;
//                double widthRatio = Math.Abs(prevRect.Width / currRect.Width - 1);
//                double heightRatio = Math.Abs(prevRect.Height / currRect.Height - 1);

//                if (widthRatio > maxPercent || heightRatio > maxPercent)
//                    shouldUpdate = true;
//            }
//            else if (e.PropertyName == "Output")
//            {
//                shouldUpdate = true;
//            }

//            if (shouldUpdate)
//            {
//                UpdateUIRepresentation();
//            }
//        }

//        private void observableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            throw new NotImplementedException();
//        }

//        private void OnDataSourceChangedEvent(object sender, EventArgs e)
//        {
//            UpdateUIRepresentation();
//        }

//        public static readonly RoutedEvent DataSourceChangedEvent = EventManager.RegisterRoutedEvent(
//            "DataSourceChanged",
//            RoutingStrategy.Direct,
//            typeof(RoutedEventHandler),
//            typeof(LineChart));

//        public event RoutedEventHandler DataSourceChanged
//        {
//            add { AddHandler(DataSourceChangedEvent, value); }
//            remove { RemoveHandler(DataSourceChangedEvent, value); }
//        }

//        #endregion

//        #region Filters

//        private readonly filters.FilterCollection filters = new filters.FilterCollection { 
//            //new InclinationFilter2(),
//            //new FrequencyScreenXFilter()
//        };

//        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
//        public filters.FilterCollection Filters
//        {
//            get { return filters; }
//        }

//        private void filters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            UpdateUIRepresentation();
//        }

//        #endregion

//        private Plotter2D plotter;

//        private DataRect visibleWhileUpdate;
//        private bool isUpdating = false;
//        private LineSplitter splitter = new LineSplitter();
//        private ViewportMarginPanel panel = new ViewportMarginPanel();
//        private void UpdateUIRepresentation()
//        {
//            if (isUpdating)
//                return;

//            if (plotter == null)
//                return;

//            IPointDataSource dataSource = DataSource;
//            if (dataSource == null)
//                return;

//            visibleWhileUpdate = plotter.Viewport.Visible;

//            isUpdating = true;

//            panel.Children.Clear();

//            DependencyObject dependencyDataSource = dataSource as DependencyObject;
//            if (dependencyDataSource != null)
//            {
//                DataSource2dContext.SetVisibleRect(dependencyDataSource, plotter.Viewport.Visible);
//                DataSource2dContext.SetScreenRect(dependencyDataSource, plotter.Viewport.Output);
//            }

//            IEnumerable<Point> viewportPoints = dataSource;

//            var transform = plotter.Viewport.Transform;
//            if (!(transform.DataTransform is IdentityTransform))
//            {
//                viewportPoints = dataSource.DataToViewport(transform.DataTransform);
//            }

//            var screenPoints = viewportPoints.ViewportToScreen(transform);
//            var filteredPoints = filters.Filter(screenPoints, plotter.Viewport);

//            DataRect bounds = DataRect.Empty;

//            double strokeThickness = 3;

//            bool first = true;
//            Point lastPointOfPrevLine = new Point();

//            int overallCount = 0;

//            panel.BeginBatchAdd();

//            const int ptsInPolyline = 500;
//            foreach (var pointGroup in filteredPoints.Split(ptsInPolyline))
//            {
//                int ptsCount = ptsInPolyline;

//                if (!first)
//                    ptsCount++;

//                PointCollection pointCollection = new PointCollection(ptsCount);

//                if (!first)
//                    pointCollection.Add(lastPointOfPrevLine);
//                else
//                    first = false;

//                pointCollection.AddMany(pointGroup);

//                overallCount += pointCollection.Count - 1;

//                if (pointCollection.Count == 0) break;

//                lastPointOfPrevLine = pointCollection[pointCollection.Count - 1];

//                pointCollection.Freeze();

//                DataRect ithBounds = BoundsHelper.GetViewportBounds(pointCollection.ScreenToViewport(transform));
//#if geom
//                UIElement line = null;

//                StreamGeometry geometry = new StreamGeometry();
//                using (var dc = geometry.Open())
//                {
//                    dc.BeginFigure(pointCollection[0], false, false);
//                    dc.PolyLineTo(pointCollection, true, false);
//                }
//                geometry.Freeze();
//                GeometryDrawing drawing = new GeometryDrawing { Geometry = geometry, Pen = new Pen(Brushes.Blue, 1) };
//                drawing.Freeze();
//                DrawingBrush brush = new DrawingBrush { Drawing = drawing };
//                brush.Freeze();

//                var rectangle = new Rectangle { Fill = brush, IsHitTestVisible = false };
//                Rect ithScreenBounds = ithBounds.ViewportToScreen(transform);
//                if (true || ithScreenBounds.Width > 2000 || ithScreenBounds.Height > 2000 || ithScreenBounds.Width < 1 || ithScreenBounds.Height < 1)
//                {
//                    line = rectangle;
//                }
//                else
//                {
//                    Size intSize = new Size((int)ithScreenBounds.Width, (int)ithScreenBounds.Height);
//                    rectangle.Measure(intSize);
//                    rectangle.Arrange(new Rect(intSize));

//                    RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)ithScreenBounds.Width, (int)ithScreenBounds.Height, 96, 96, PixelFormats.Pbgra32);
//                    renderBitmap.Render(rectangle);
//                    renderBitmap.Freeze();
//                    line = new Image { Source = renderBitmap };
//                }
//#else
//                var line = CreateLine();
//                line.Points = pointCollection;
//#endif

//                bounds.Union(ithBounds);

//                ViewportRectPanel.SetViewportBounds(line, ithBounds);
//#if !geom
//                ViewportMarginPanel.SetScreenMargin(line, new Size(strokeThickness / 2, strokeThickness / 2));
//#endif
//                panel.Children.Add(line);

//                isUpdating = false;
//            }

//            panel.EndBatchAdd();

//            Debug.WriteLine("OverallCount = " + (overallCount + 1));

//            Viewport2D.SetContentBounds(this, bounds);

//            isUpdating = false;
//        }

//        protected virtual Polyline CreateLine()
//        {
//            Polyline line = new Polyline
//            {
//                Stretch = Stretch.Fill,
//                IsHitTestVisible = false,
//                //Fill = Brushes.DarkMagenta.MakeTransparent(0.1),
//            };
//            line.SetBinding(Shape.StrokeProperty, new Binding { Source = this, Path = new PropertyPath("Stroke") });
//            line.SetBinding(Shape.StrokeThicknessProperty, new Binding { Source = this, Path = new PropertyPath("StrokeThickness") });
//            return line;
//        }

//        #region IPlotterElement Members

//        public void OnPlotterAttached(Plotter plotter)
//        {
//            this.plotter = (Plotter2D)plotter;
//            panel.OnPlotterAttached(plotter);

//            this.plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;

//            UpdateUIRepresentation();
//        }

//        public void OnPlotterDetaching(Plotter plotter)
//        {
//            this.plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;

//            panel.OnPlotterDetaching(plotter);
//            this.plotter = null;
//        }

//        #endregion

//        #region IPlotterElement Members


//        public Plotter Plotter
//        {
//            get { throw new NotImplementedException(); }
//        }

//        #endregion
//    }
//}
