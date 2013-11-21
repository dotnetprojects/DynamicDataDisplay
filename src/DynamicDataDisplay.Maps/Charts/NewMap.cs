using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using Microsoft.Research.DynamicDataDisplay.Maps;
using System.Collections.Generic;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;


namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
    [ContentProperty("SourceTileServer")]
    public class NewMap : FrameworkElement, IPlotterElement
    {
        public NewMap()
        {
            server.ImageLoaded += OnTileLoaded;
            server.SourceServerChanged += OnNetworkServerChanged;

#if DEBUG
            drawDebugBounds = false;
#endif
        }

        private TileProvider tileProvider = new MapTileProvider();
        public TileProvider TileProvider
        {
            get { return tileProvider; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                tileProvider = value;
            }
        }

        private readonly TileServerSystem server = new TileServerSystem();
        public TileServerSystem TileSystem
        {
            get { return server; }
        }

        [NotNull]
        public SourceTileServer SourceTileServer
        {
            get { return server.SourceServer; }
            set { server.SourceServer = value; }
        }

        public TileSystemMode Mode
        {
            get { return server.Mode; }
            set { server.Mode = value; }
        }

        public IWriteableTileServer FileTileServer
        {
            get { return server.FileServer; }
            set { server.FileServer = value; }
        }

        private readonly ViewportHostPanel panel = new ViewportHostPanel();
        public ViewportHostPanel Panel
        {
            get { return panel; }
        }

        protected virtual void OnTileLoaded(object sender, TileLoadResultEventArgs e)
        {
            if (e.Result == TileLoadResult.Success)
            {
                DataRect tileBounds = tileProvider.GetTileBounds(e.ID);

                bool containsInDrawn = drawnBounds.Contains(tileBounds) || drawnBounds.IsEmpty;
                bool shouldDraw = containsInDrawn && !invalidatePending && e.ID.Level <= tileProvider.Level;
                if (shouldDraw)
                {
                    DrawImage(e.Image, tileBounds, e.ID);
                }
            }
        }

        private int tileWidth = 256;
        private int tileHeight = 256;
        protected virtual void OnNetworkServerChanged(object sender, ValueChangedEventArgs<SourceTileServer> e)
        {
            if (e.PreviousValue != null)
            {
                RemoveLogicalChild(e.PreviousValue);
            }
            if (e.CurrentValue != null)
            {
                AddLogicalChild(e.CurrentValue);
            }

            SourceTileServer networkServer = server.SourceServer;
            if (networkServer != null)
            {
                tileProvider.MinLevel = networkServer.MinLevel;
                tileProvider.MaxLevel = networkServer.MaxLevel;

                MapTileProvider mapProvider = tileProvider as MapTileProvider;
                if (mapProvider != null)
                {
                    mapProvider.MaxLatitude = networkServer.MaxLatitude;
                }
                tileWidth = networkServer.TileWidth;
                tileHeight = networkServer.TileHeight;
            }

            BeginInvalidateVisual();
        }

        private bool drawDebugBounds = false;
        public bool DrawDebugBounds
        {
            get { return drawDebugBounds; }
            set { drawDebugBounds = value; }
        }

        private readonly Pen debugBoundsPen = new Pen(Brushes.Red.MakeTransparent(0.5), 1);

        bool showsWholeMap = false;
        DataRect drawnBounds = DataRect.Empty;
        DataRect visibleBounds;
        bool invalidatePending = false;
        bool rendering = false;
        protected void OnRender()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            if (plotter == null)
                return;

            panel.Children.Clear();

            freeChildren = new List<MapElement>(panel.Children.Count);
            for (int i = 0; i < panel.Children.Count; i++)
            {
                freeChildren.Add((MapElement)panel.Children[i]);
            }

            rendering = true;
            invalidatePending = false;

            var transform = plotter.Viewport.Transform;
            Rect output = plotter.Viewport.Output;
            DataRect visible = plotter.Viewport.Visible;
            visibleBounds = visible;

            drawnBounds = DataRect.Empty;

            var tileInfos = GetVisibleTiles();

            var lowerTilesList = GetLoadedLowerTiles(tileInfos);
            // displaying lower tiles
            foreach (var tile in lowerTilesList)
            {
                if (server.IsLoaded(tile))
                {
                    var bmp = server[tile];
                    DataRect visibleRect = tileProvider.GetTileBounds(tile);
                    DrawImage(bmp, visibleRect, tile);
                }
                else
                {
                    server.BeginLoadImage(tile);
                }
            }

            int count = 0;
            foreach (var tileInfo in tileInfos)
            {
                drawnBounds.Union(tileInfo.VisibleBounds);

                count++;
                if (server.IsLoaded(tileInfo.Tile))
                {
                    var bmp = server[tileInfo.Tile];
                    DrawImage(bmp, tileInfo.VisibleBounds, tileInfo.Tile);
                }
                else
                {
                    server.BeginLoadImage(tileInfo.Tile);
                }
            }
            showsWholeMap = count == MapTileProvider.GetTotalTilesCount(tileProvider.Level);

            foreach (var item in freeChildren)
            {
                panel.Children.Remove(item);
                pool.Put(item);
            }

            foreach (MapElement item in panel.Children)
            {
                if (item.Bitmap == null)
                {
                    panel.Children.Remove(item);
                    pool.Put(item);
                }
            }

            rendering = false;

            Debug.WriteLine("Drawn " + Environment.TickCount);
        }

        private List<MapElement> freeChildren = new List<MapElement>();
        private readonly ResourcePool<MapElement> pool = new ResourcePool<MapElement>();
        private void DrawImage(BitmapSource bmp, DataRect bounds, TileIndex id)
        {
            MapElement element = null;
            bool onPanel = false;
            if (freeChildren.Count > 0)
            {
                element = (MapElement)freeChildren[freeChildren.Count - 1];
                freeChildren.RemoveAt(freeChildren.Count - 1);
                onPanel = true;
            }
            else
            {
                element = pool.GetOrCreate();
            }

            element.Bitmap = bmp;
            ViewportPanel.SetViewportBounds(element, bounds);
            System.Windows.Controls.Panel.SetZIndex(element, (int)id.Level);

            if (!onPanel)
            {
                panel.Children.Add(element);
                panel.InvalidateMeasure();
            }
        }

        protected virtual DataRect Transform(DataRect visibleRect)
        {
            return visibleRect;
        }

        protected virtual DataRect TransformRegion(DataRect visibleRect)
        {
            return visibleRect;
        }

        protected List<VisibleTileInfo> GetVisibleTiles()
        {
            var transform = plotter.Viewport.Transform;
            Rect output = plotter.Viewport.Output;
            DataRect visible = plotter.Viewport.Visible;

            var tileInfos = (from tile in tileProvider.GetTilesForRegion(TransformRegion(visible), tileProvider.Level)
                             let visibleRect = Transform(tileProvider.GetTileBounds(tile))
                             let screenRect = visibleRect.ViewportToScreen(transform)
                             where output.IntersectsWith(screenRect)
                             select new VisibleTileInfo { Tile = tile, ScreenBounds = screenRect, VisibleBounds = visibleRect }).ToList();

            if (tileInfos.Count > MapTileProvider.GetTotalTilesCount(tileProvider.Level))
            {
                //Debugger.Break();
            }

            return tileInfos;
        }

        protected virtual IEnumerable<TileIndex> GetLoadedLowerTiles(IEnumerable<VisibleTileInfo> visibleTiles)
        {
            Set<TileIndex> result = new Set<TileIndex>();

            foreach (var tileInfo in visibleTiles)
            {
                if (!server.IsLoaded(tileInfo.Tile))
                {
                    bool found = false;
                    var tile = tileInfo.Tile;
                    do
                    {
                        if (tile.HasLowerTile)
                        {
                            tile = tile.GetLowerTile();
                            if (server.IsLoaded(tile) || tile.Level == 1)
                            {
                                found = true;
                                result.TryAdd(tile);
                            }
                        }
                        else
                        {
                            found = true;
                        }
                    }
                    while (!found);
                }
            }

            return result.OrderBy(id => id.Level);
        }

        protected Rect EnlargeRect(Rect rect)
        {
            double coeff = 1 + 1.0 / tileWidth;
            return EnlargeRect(rect, coeff);
        }

        protected Rect EnlargeRect(Rect rect, double rectZoomCoeff)
        {
            Rect res = rect;
            res = res.Zoom(res.GetCenter(), rectZoomCoeff);
            return res;
        }

        private FormattedText CreateText(TileIndex tileIndex)
        {
            FormattedText text = new FormattedText(tileIndex.ToString(),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Arial"), 8, Brushes.Red);
            return text;
        }

        #region ChangesTextFormat property

        public bool ChangesTextFormat
        {
            get { return (bool)GetValue(ChangesTextFormatProperty); }
            set { SetValue(ChangesTextFormatProperty, value); }
        }

        public static readonly DependencyProperty ChangesTextFormatProperty = DependencyProperty.Register(
          "ChangesTextFormat",
          typeof(bool),
          typeof(NewMap),
          new FrameworkPropertyMetadata(true, OnChangesTextFormatChanged));

        private static void OnChangesTextFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NewMap map = (NewMap)d;
            map.OnChangesTextFormatChanged();
        }

        private void OnChangesTextFormatChanged()
        {
            // do nothing if disconnected
            if (plotter == null)
                return;

            if (ChangesTextFormat)
            {
                plotter.Children.CollectionChanged += PlotterChildren_CollectionChanged;
                ChangeTextFormat();
            }
            else
            {
                plotter.Children.CollectionChanged -= PlotterChildren_CollectionChanged;
                RevertTextFormat();
            }
        }

        #endregion

        protected virtual void BeginInvalidateVisual()
        {
            if (!rendering)
            {
                invalidatePending = true;
                OnRender();
            }
            else
            {
                Dispatcher.BeginInvoke(((Action)(() => { OnRender(); })));
            }
        }

        #region IPlotterElement Members

        Func<double, string> prevXMapping;
        Func<double, string> prevYMapping;
        PhysicalProportionsRestriction proportionsRestriction = new PhysicalProportionsRestriction(2.0);
        protected PhysicalProportionsRestriction ProportionsRestriction
        {
            get { return proportionsRestriction; }
        }

        MaximalSizeRestriction maxSizeRestriction = new MaximalSizeRestriction();
        protected MaximalSizeRestriction MaxSizeRestriction
        {
            get { return maxSizeRestriction; }
        }

        void IPlotterElement.OnPlotterAttached(Plotter plotter)
        {
            this.plotter = (Plotter2D)plotter;
            this.plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;

            this.plotter.Viewport.Restrictions.Add(maxSizeRestriction);
            this.plotter.Viewport.Restrictions.Add(proportionsRestriction);

            this.plotter.KeyDown += new KeyEventHandler(plotter_KeyDown);

            if (ChangesTextFormat)
            {
                plotter.Children.CollectionChanged += PlotterChildren_CollectionChanged;
                // changing text mappings of CursorCoordinateGraph, if it exists,
                // to display text labels with degrees.
                ChangeTextFormat();
            }

            plotter.Children.BeginAdd(panel);

            OnPlotterAttached(plotter);

            BeginInvalidateVisual();
        }

        protected virtual void OnPlotterAttached(Plotter plotter)
        {
        }

        private void plotter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                tileProvider.IncreaseLevel();
                BeginInvalidateVisual();
            }
            else if (e.Key == Key.W)
            {
                tileProvider.DecreaseLevel();
                BeginInvalidateVisual();
            }
        }

        void PlotterChildren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ChangesTextFormat)
            {
                ChangeTextFormat();
            }
        }

        bool changedTextFormat = false;
        private void ChangeTextFormat()
        {
            if (changedTextFormat)
                return;


            // todo discover why sometimes we arrive here from PlotterChildren_CollectionChanged when we have removed this handler from
            // plotter.Children.CollectionChanged invocation list.
            if (plotter == null)
                return;

            var cursorGraph = plotter.Children.OfType<CursorCoordinateGraph>().FirstOrDefault<CursorCoordinateGraph>();
            if (cursorGraph != null)
            {
                changedTextFormat = true;

                // saving previous text mappings
                prevXMapping = cursorGraph.XTextMapping;
                prevYMapping = cursorGraph.YTextMapping;


                // setting new text mappings
                cursorGraph.XTextMapping = value =>
                {
                    if (Double.IsNaN(value))
                        return "";

                    if (-180 <= value && value <= 180)
                    {
                        Degree degree = Degree.CreateLongitude(value);
                        return degree.ToString();
                    }
                    else return null;
                };

                cursorGraph.YTextMapping = value =>
                {
                    if (Double.IsNaN(value))
                        return "";

                    var mapTileProvider = tileProvider as MapTileProvider;
                    if (mapTileProvider != null)
                    {
                        if (mapTileProvider.MinLatitude <= value && value <= mapTileProvider.MaxLatitude)
                        {
                            Degree degree = Degree.CreateLatitude(value);
                            return degree.ToString();
                        }
                    }
                    return null;
                };
            }
        }


        private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
        {
            var transform = plotter.Viewport.Transform;

            double prevLevel = tileProvider.Level;
            UpdateLevel(transform);

            bool shouldRedraw = prevLevel != tileProvider.Level;

            if (e.PropertyName == "Visible")
            {
                DataRect currVisible = (DataRect)e.NewValue;
                shouldRedraw |= !(drawnBounds.Contains(currVisible) || showsWholeMap);
            }
            else if (e.PropertyName == "Transform") { }

            if (shouldRedraw)
            {
                BeginInvalidateVisual();
            }
        }

        protected virtual void UpdateLevel(CoordinateTransform transform)
        {
            bool ok = false;
            do
            {
                double width = tileProvider.GetTileWidth(tileProvider.Level);
                double height = tileProvider.GetTileHeight(tileProvider.Level);

                DataRect size = new DataRect(new Size(width, height));
                Rect onScreen = size.ViewportToScreen(transform);

                // todo написать нормально
                if (onScreen.Width > tileWidth * 1.45)
                {
                    if (tileProvider.IncreaseLevel())
                    {
                        continue;
                    }
                }
                else if (onScreen.Width < tileWidth / 1.45)
                {
                    if (tileProvider.DecreaseLevel())
                    {
                        continue;
                    }
                }
                ok = true;
            } while (!ok);
        }

        protected virtual void OnPlotterDetaching(Plotter plotter) { }

        void IPlotterElement.OnPlotterDetaching(Plotter plotter)
        {
            OnPlotterDetaching(plotter);

            visibleBounds = new Rect();

            this.plotter.Children.BeginRemove(panel);
            this.plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;

            this.plotter.Viewport.Restrictions.Remove(proportionsRestriction);
            this.plotter.Viewport.Restrictions.Remove(maxSizeRestriction);

            this.plotter.Children.CollectionChanged -= PlotterChildren_CollectionChanged;

            RevertTextFormat();

            this.plotter = null;
        }

        private void RevertTextFormat()
        {
            if (changedTextFormat)
            {
                // revert test mappings of CursorCoordinateGraph, if it exists.
                var cursorGraph = plotter.Children.OfType<CursorCoordinateGraph>().FirstOrDefault<CursorCoordinateGraph>();
                if (cursorGraph != null)
                {
                    cursorGraph.XTextMapping = prevXMapping;
                    cursorGraph.YTextMapping = prevYMapping;
                }
                changedTextFormat = false;
            }
        }

        private Plotter2D plotter;
        public Plotter2D Plotter
        {
            get { return plotter; }
        }

        Plotter IPlotterElement.Plotter
        {
            get { return plotter; }
        }

        #endregion
    }
}
