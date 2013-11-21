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
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
    [ContentProperty("SourceTileServer")]
    public partial class Map : Canvas, IPlotterElement
    {
        public Map()
        {
            server.ImageLoaded += OnTileLoaded;
            server.SourceServerChanged += OnSourceServerChanged;

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
                BeginInvalidateVisual();
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

		#region AspectRatio property

		/// <summary>
		/// Gets or sets map's aspect ratio. Default value is 2.0.
		/// </summary>
		/// <value>The proportion.</value>
		public double AspectRatio
		{
			get { return (double)GetValue(AspectRatioProperty); }
			set { SetValue(AspectRatioProperty, value); }
		}

		public static readonly DependencyProperty AspectRatioProperty = DependencyProperty.Register(
		  "AspectRatio",
		  typeof(double),
		  typeof(Map),
		  new FrameworkPropertyMetadata(2.0, OnAspectRatioChanged));

		private static void OnAspectRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Map owner = (Map)d;
			double aspectRatio = (double)e.NewValue;
			owner.proportionsRestriction.ProportionRatio = aspectRatio;
		}

		#endregion // end of Proportion property

		protected virtual void OnTileLoaded(object sender, TileLoadResultEventArgs e)
        {
            if (e.Result == TileLoadResult.Success)
            {
                DataRect tileBounds = tileProvider.GetTileBounds(e.ID);

                bool intersectsWithVisible = visibleBounds.IntersectsWith(tileBounds);
                if (intersectsWithVisible && !invalidatePending && e.ID.Level <= tileProvider.Level)
                {
                    BeginInvalidateVisual();
                }
            }
        }

        private int tileWidth = 256;
        private int tileHeight = 256;
        protected virtual void OnSourceServerChanged(object sender, ValueChangedEventArgs<SourceTileServer> e)
        {
            if (e.PreviousValue != null)
            {
                RemoveLogicalChild(e.PreviousValue);
            }
            if (e.CurrentValue != null)
            {
                AddLogicalChild(e.CurrentValue);
            }

            SourceTileServer sourceServer = server.SourceServer;
            if (sourceServer != null)
            {
                tileProvider.MinLevel = sourceServer.MinLevel;
                tileProvider.MaxLevel = sourceServer.MaxLevel;
                tileWidth = sourceServer.TileWidth;
                tileHeight = sourceServer.TileHeight;

                MapTileProvider provider = tileProvider as MapTileProvider;
                if (provider != null)
                    provider.MaxLatitude = sourceServer.MaxLatitude;
            }

            UpdateDebugMenuHeaders();

            BeginInvalidateVisual();
        }

        private void UpdateDebugMenuHeaders()
        {
            string name = SourceTileServer != null ? String.Format(" \"{0}\"", SourceTileServer.ServerName) : String.Empty;
            // supposing that if this menu item is not equal to null, others are not equal, too.
            if (openCacheItem != null)
            {
                openCacheItem.Header = "Open cache" + name;
                clearCacheItem.Header = "Clear cache" + name;
                clearMemoryCacheItem.Header = "Clear memory cache" + name;
            }
        }

        private bool drawDebugBounds = false;
        public bool DrawDebugBounds
        {
            get { return drawDebugBounds; }
            set
            {
                drawDebugBounds = value;
                BeginInvalidateVisual();
            }
        }

        private bool showLowerTiles = true;
        public bool ShowLowerTiles
        {
            get { return showLowerTiles; }
            set
            {
                showLowerTiles = value;
                BeginInvalidateVisual();
            }
        }

        private readonly Pen debugBoundsPen = new Pen(Brushes.Red.MakeTransparent(0.5), 1);

        DataRect visibleBounds;
        bool invalidatePending = false;
        bool rendering = false;
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            if (plotter == null)
                return;

            rendering = true;
            invalidatePending = false;

            base.OnRender(drawingContext);

            var transform = plotter.Viewport.Transform;
            Rect output = plotter.Viewport.Output;
            DataRect visible = plotter.Viewport.Visible;
            visibleBounds = visible;

            var tileInfos = GetVisibleTiles();

            //Debug.WriteLine(String.Format("OnRender: {0}", DateTime.Now.TimeOfDay.ToString()));

            var dc = drawingContext;
            var lowerTilesList = GetLoadedLowerTiles(tileInfos);
            // displaying lower tiles
            if (showLowerTiles)
            {
                foreach (var tileInfo in lowerTilesList)
                {
                    var tile = tileInfo.Id;

                    if (server.IsLoaded(tile))
                    {
                        var bmp = server[tile];
                        DataRect visibleRect = tileProvider.GetTileBounds(tile);
                        Rect screenRect = visibleRect.ViewportToScreen(transform);
                        Rect enlargedRect = EnlargeRect(screenRect);

                        dc.PushClip(tileInfo.Clip);
                        dc.DrawImage(bmp, enlargedRect);
                        dc.Pop();

                        //if (!visibleRect.IntersectsWith(visible))
                        //{
                        //    dc.DrawRectangle(Brushes.Red, null, output);
                        //}

                        if (drawDebugBounds)
                        {
                            DrawDebugInfo(dc, enlargedRect, tile);
                        }
                    }
                    else
                    {
                        server.BeginLoadImage(tile);
                    }
                }
            }

            foreach (var tileInfo in tileInfos)
            {
                if (server.IsLoaded(tileInfo.Tile))
                {
                    var bmp = server[tileInfo.Tile];

                    Rect enlargedRect = EnlargeRect(tileInfo.ScreenBounds);
                    drawingContext.DrawImage(bmp, enlargedRect);

                    // drawing debug bounds
                    if (drawDebugBounds)
                    {
                        DrawDebugInfo(drawingContext, tileInfo.ScreenBounds, tileInfo.Tile);
                    }
                }
                else
                {
                    server.BeginLoadImage(tileInfo.Tile);
                }
            }

            rendering = false;
        }

        private void DrawDebugInfo(DrawingContext dc, Rect screenBounds, TileIndex tile)
        {
            // bounds around the whole tile
            dc.DrawRectangle(null, debugBoundsPen, screenBounds);

            var text = CreateText(tile);
            // drawing text only if its size is less that the whole tile's screen size
            if (screenBounds.Width > text.Width && screenBounds.Height > text.Height)
            {
                Point position = screenBounds.Location;
                position.Offset(3, 3);
                // white rect around the text
                dc.DrawRectangle(Brushes.White, null, new Rect(position, new Size(text.Width, text.Height)));
                // the very text
                dc.DrawText(text, position);
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

            using (new DisposableTimer("GetVisibleTiles", false))
            {
                var tileInfos = (from tile in tileProvider.GetTilesForRegion(TransformRegion(visible), tileProvider.Level)
                                 let visibleRect = Transform(tileProvider.GetTileBounds(tile))
                                 let screenRect = visibleRect.ViewportToScreen(transform)
                                 where output.IntersectsWith(screenRect)
                                 select new VisibleTileInfo { Tile = tile, ScreenBounds = screenRect, VisibleBounds = visibleRect }).ToList();

                return tileInfos;
            }
        }

        protected virtual IEnumerable<LowerTileInfo> GetLoadedLowerTiles(IEnumerable<VisibleTileInfo> visibleTiles)
        {
            Set<TileIndex> result = new Set<TileIndex>();

            var minLevel = tileProvider.MinLevel;
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
                            if (server.IsLoaded(tile) || tile.Level == minLevel)
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

            return result.OrderBy(id => id.Level).Select(id => new LowerTileInfo { Id = id });
        }

        protected Rect EnlargeRect(Rect rect)
        {
            Size newSize = rect.Size;
            newSize.Width += 1;
            newSize.Height += 1;
            Rect result = RectExtensions.FromCenterSize(rect.GetCenter(), newSize);
            return result;

            //double coeff = 1 + 1.0 / tileWidth;
            //return EnlargeRect(rect, coeff);
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
          typeof(Map),
          new FrameworkPropertyMetadata(true, OnChangesTextFormatChanged));

        private static void OnChangesTextFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Map map = (Map)d;
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

        protected void BeginInvalidateVisual()
        {
            if (plotter == null) return;

            UpdateLevel(plotter.Transform);
            BeginInvalidateVisualCore();
        }

        protected virtual void BeginInvalidateVisualCore()
        {
            if (!rendering)
            {
                invalidatePending = true;
                InvalidateVisual();
            }
            else
            {
                //Dispatcher.BeginInvoke(((Action)(() => { InvalidateVisual(); })));
            }
        }

        #region IPlotterElement Members

        MenuItem mapsMenu;
        Func<double, string> prevXMapping;
        Func<double, string> prevYMapping;
        private PhysicalProportionsRestriction proportionsRestriction = new PhysicalProportionsRestriction(2.0);
        public PhysicalProportionsRestriction ProportionsRestriction
        {
            get { return proportionsRestriction; }
        }

        private MaximalSizeRestriction maxSizeRestriction = new MaximalSizeRestriction();
        public MaximalSizeRestriction MaxSizeRestriction
        {
            get { return maxSizeRestriction; }
        }

        MenuItem openCacheItem;
        MenuItem clearCacheItem;
        MenuItem clearMemoryCacheItem;
        void IPlotterElement.OnPlotterAttached(Plotter plotter)
        {
            this.plotter = (Plotter2D)plotter;
            this.plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;

            var debugMenu = plotter.Children.OfType<DebugMenu>().FirstOrDefault();
            if (debugMenu != null)
            {
                mapsMenu = debugMenu.TryFindMenuItem("Maps");
                if (mapsMenu == null)
                {
                    mapsMenu = new MenuItem { Header = "Maps" };
                    debugMenu.Menu.Items.Add(mapsMenu);
                }

                openCacheItem = new MenuItem { Header = "Open cache" };
                openCacheItem.Click += OnOpenCache;
                mapsMenu.Items.Add(openCacheItem);

                clearCacheItem = new MenuItem { Header = "Clear cache" };
                clearCacheItem.Click += OnClearCache;
                mapsMenu.Items.Add(clearCacheItem);

                clearMemoryCacheItem = new MenuItem { Header = "Clear memory cache" };
                clearMemoryCacheItem.Click += OnClearMemoryCache;
                mapsMenu.Items.Add(clearMemoryCacheItem);

                UpdateDebugMenuHeaders();

                var deleteCachesItem = mapsMenu.FindChildByHeader("Delete all caches");
                if (deleteCachesItem == null)
                {
                    deleteCachesItem = new MenuItem { Header = "Delete all caches" };
                    deleteCachesItem.Click += OnDeleteCaches;
                    mapsMenu.Items.Add(deleteCachesItem);
                }
            }

            // add default restrictions
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

            plotter.CentralGrid.Children.Add(this);

            OnPlotterAttached(plotter);
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

                    MapTileProvider provider = tileProvider as MapTileProvider;
                    if (provider != null && provider.MinLatitude <= value && value <= provider.MaxLatitude)
                    {
                        Degree degree = Degree.CreateLatitude(value);
                        return degree.ToString();
                    }
                    else return null;
                };
            }
        }


        private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
        {
            var transform = plotter.Viewport.Transform;

            UpdateLevel(transform);

            BeginInvalidateVisual();
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

            var debugMenu = plotter.Children.OfType<DebugMenu>().FirstOrDefault();
            if (debugMenu != null)
            {
                debugMenu.Menu.Items.Remove(mapsMenu);
            }

            visibleBounds = new Rect();

            this.plotter.CentralGrid.Children.Remove(this);
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

    /// <summary>
    /// Contains information about loaded lower tile - its Id and a Clip, that should be applied to 
    /// visual, that is rendering this tile.
    /// </summary>
    public sealed class LowerTileInfo
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public TileIndex Id { get; set; }
        /// <summary>
        /// Gets or sets the geometric clip.
        /// </summary>
        /// <value>The clip.</value>
        public Geometry Clip { get; set; }
    }
}
