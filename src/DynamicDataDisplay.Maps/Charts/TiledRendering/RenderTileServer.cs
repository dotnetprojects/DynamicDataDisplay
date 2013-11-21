using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using System.Windows;
using System.Windows.Markup;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.TiledRendering
{
    [ContentProperty("VisualToRender")]
    public class RenderTileServer : SourceTileServer
    {
        private CapturingPlotter plotter = new CapturingPlotter();
        internal CapturingPlotter RenderingPlotter
        {
            get { return plotter; }
        }

        private FrameworkElement child;

        public RenderTileServer()
        {
            plotter.PerformLoad();
            plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;
            plotter.ViewportClipToBoundsEnlargeFactor = 1;

            plotter.Measure(tileSize);
            plotter.Arrange(new Rect(tileSize));

            TileWidth = (int)tileSize.Width;
            TileHeight = (int)tileSize.Height;
            ServerName = "Render";

            DeleteFileCacheOnUpdate = true;

            MinLevel = Int32.MinValue;
            MaxLevel = Int32.MaxValue;
        }

        public FrameworkElement VisualToRender
        {
            get { return child; }
            set
            {
                if (child != null)
                {
                    child.RemoveHandler(Viewport2D.ContentBoundsChangedEvent, new RoutedEventHandler(OnChildContentBoundsChanged));
                    child.RemoveHandler(BackgroundRenderer.RenderingFinished, new RoutedEventHandler(OnChildRenderingFinished));
                    child.RemoveHandler(BackgroundRenderer.UpdateRequested, new RoutedEventHandler(OnChildUpdateRequested));
                }
                child = value;
                if (child != null)
                {
                    child.AddHandler(Viewport2D.ContentBoundsChangedEvent, new RoutedEventHandler(OnChildContentBoundsChanged));
                    child.AddHandler(BackgroundRenderer.RenderingFinished, new RoutedEventHandler(OnChildRenderingFinished));
                    child.AddHandler(BackgroundRenderer.UpdateRequested, new RoutedEventHandler(OnChildUpdateRequested));
                }

				RaiseChangedEvent();
            }
        }

        private void OnChildUpdateRequested(object sender, RoutedEventArgs e)
        {
            RaiseChangedEvent();
        }

        private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Output")
            {
                Debug.WriteLine("Output = " + e.NewValue);
                Rect currOutput = (Rect)e.NewValue;
                // not a default value of output, which is (x=0,y=0,w=1,h=1)
                if (currOutput.Width > 1 && child != null)
                {
                    plotter.Children.Add((IPlotterElement)child);
                }
            }
        }

        private TileIndex renderingID;
        private void OnChildRenderingFinished(object sender, RoutedEventArgs e)
        {
            plotter.Dispatcher.Invoke(() => { }, DispatcherPriority.Input);

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)tileSize.Width, (int)tileSize.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(plotter);
            bmp.Freeze();
            rendering = false;
            ReportSuccessAsync(null, bmp, renderingID);

            Debug.WriteLine(String.Format("Finished rendering ID={0}", renderingID));
        }

        protected void OnChildContentBoundsChanged(object sender, RoutedEventArgs e)
        {
            DependencyObject dependencySender = sender as DependencyObject;
            var contentBounds = Viewport2D.GetContentBounds(dependencySender);
            Debug.WriteLine("ContentBounds = " + contentBounds);

            RaiseChangedEvent();
        }

        private readonly Queue<TileIndex> waitingIndexes = new Queue<TileIndex>();

        bool rendering = false;
        private void RenderChildToBitmap(TileIndex id)
        {
            if (rendering)
            {
                waitingIndexes.Enqueue(id);
                return;
            }

            renderingID = id;

            if (!BackgroundRenderer.GetUsesBackgroundRendering(child))
            {
                plotter.Dispatcher.BeginInvoke(() =>
                {
                    RenderToBitmapCore(id);
                }, DispatcherPriority.Background);
            }
            else
            {
                RenderToBitmapCore(id);
            }
        }

        private void RenderToBitmapCore(TileIndex id)
        {
            rendering = true;
            var visible = GetTileBounds(id);

            Debug.WriteLine(String.Format("Visible is {0} for id={1}", visible.ToString(), id.ToString()));

            plotter.Visible = visible;
            //plotter.InvalidateVisual();

            if (!BackgroundRenderer.GetUsesBackgroundRendering(child))
            {
                // this is done to make all inside plotter to perform measure and arrange procedures
                plotter.Dispatcher.Invoke(() => { }, DispatcherPriority.Input);
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)tileSize.Width, (int)tileSize.Height, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(plotter);
                bmp.Freeze();
                ReportSuccessAsync(null, bmp, id);
                rendering = false;
            }
        }

        protected override void ReportSuccess(Stream stream, BitmapSource bmp, TileIndex id)
        {
            base.ReportSuccess(stream, bmp, id);

            if (waitingIndexes.Count > 0)
            {
                var nextId = waitingIndexes.Dequeue();
                RenderChildToBitmap(nextId);
            }
        }

        private Size tileSize = new Size(256, 512);

        public override bool Contains(TileIndex id)
        {
            if (child == null) return false;

            var childBounds = Viewport2D.GetContentBounds(child);
            var tileBounds = GetTileBounds(id);

            bool contains = childBounds.IntersectsWith(tileBounds);
            return contains;
        }

        public override void BeginLoadImage(TileIndex id)
        {
            RenderChildToBitmap(id);
        }

        private DataRect GetTileBounds(TileIndex id)
        {
            double tileSide = GetTileSide(id.Level);
            DataRect result = new DataRect(id.X * tileSide, id.Y * tileSide, tileSide, tileSide);
            return result;
        }

        private double GetTileSide(double level)
        {
            // we are assuming that tile in level 0 is a square of 1*1 units in viewport coordinates
            return 1.0 * Math.Pow(2, -level);
        }

        public override void CancelRunningOperations()
        {
            waitingIndexes.Clear();
        }
    }
}
