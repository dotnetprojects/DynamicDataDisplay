using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Maps;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
    public class MercatorShaderMap : Map
    {
        public MercatorShaderMap()
        {
            IsHitTestVisible = false;
        }

        private MercatorTransform mercatorTransform = new MercatorTransform();

        private Panel ContentPanel
        {
            get { return this; }
        }

        protected override void OnTileLoaded(object sender, TileLoadResultEventArgs e)
        {
            if (e.Result == TileLoadResult.Success)
            {
                BeginInvalidateVisual();
            }
        }

        protected override void OnSourceServerChanged(object sender, ValueChangedEventArgs<SourceTileServer> e)
        {
            base.OnSourceServerChanged(sender, e);

            var mapTileProvider = TileProvider as MapTileProvider;
            if (mapTileProvider == null)
                throw new ArgumentException("MercatorShaderMap's TileProvider should be only MapTileProvider.");

            mercatorTransform = new MercatorTransform(mapTileProvider.MaxShaderLatitude);
        }

        protected override DataRect Transform(DataRect viewportRect)
        {
            return viewportRect.ViewportToData(mercatorTransform);
        }

        protected override DataRect TransformRegion(DataRect dataRect)
        {
            return dataRect.DataToViewport(mercatorTransform);
        }

        private bool rendering = false;
        private void OnRender()
        {
            if (Plotter == null) return;

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            if (rendering && !renderingPending)
            {
                Dispatcher.BeginInvoke(OnRender);
                return;
            }
            else if (rendering)
                return;

            rendering = true;
            renderingPending = false;

            ContentPanel.Children.Clear();

            var transform = Plotter.Viewport.Transform;
            Rect output = Plotter.Viewport.Output;

            DataRect visible = Plotter.Viewport.Visible;
            var tileInfos = GetVisibleTiles();

            var lowerTiles = GetLoadedLowerTiles(tileInfos);

            foreach (var tile in lowerTiles)
            {
                var id = tile.Id;
                if (TileSystem.IsLoaded(id))
                {
                    var bmp = TileSystem[id];
                    DataRect visibleRect = Transform(TileProvider.GetTileBounds(id));
                    Rect screenRect = visibleRect.DataToScreen(transform);

                    DrawTile(bmp, screenRect, visibleRect, tile.Clip);
                }
                else
                {
                    TileSystem.BeginLoadImage(id);
                }
            }

            foreach (var tileInfo in tileInfos)
            {
                if (TileSystem.IsLoaded(tileInfo.Tile))
                {
                    var bmp = TileSystem[tileInfo.Tile];
                    DrawTile(bmp, tileInfo.ScreenBounds, tileInfo.VisibleBounds, null);
                }
                else
                {
                    TileSystem.BeginLoadImage(tileInfo.Tile);
                }
            }

            rendering = false;
        }

        private const double maxScreenSizeWhenShaderIsUsed = 2000;
        private void DrawTile(BitmapSource bmp, Rect screenBounds, DataRect visibleBounds, Geometry clip)
        {
            // enlarges tile with reverse proportion with its size,
            // so that difference with enlarged and original rectangle is less than one pixel
            double enlargeCoeff = 1 + 1.0 / screenBounds.Width;

            MapTileUIElement element = new MapTileUIElement
            {
                Bounds = EnlargeRect(screenBounds, enlargeCoeff),
                Tile = bmp,
                DrawDebugBounds = this.DrawDebugBounds,
                Clip = clip
            };

            // mercator shader will be used only if output bounds of this tile are less than 
            // 2000 pixels. Otherwise this can cause enormous video memory usage.
            if (screenBounds.Width < maxScreenSizeWhenShaderIsUsed && screenBounds.Height < maxScreenSizeWhenShaderIsUsed)
                element.Effect = CreateEffect(visibleBounds);

            ContentPanel.Children.Add(element);
        }

        private Effect CreateEffect(DataRect bounds)
        {
            MercatorShader effect = new MercatorShader();

            effect.YMax = Math.Max(bounds.YMin, bounds.YMax);
            effect.YDiff = bounds.Height;

            double latMax = mercatorTransform.DataToViewport(new Point(0, Math.Max(bounds.YMin, bounds.YMax))).Y;
            double latMin = mercatorTransform.DataToViewport(new Point(0, Math.Min(bounds.YMin, bounds.YMax))).Y;

            effect.YLatMax = latMax;
            effect.YLatDiff = 1.0 / Math.Abs(latMax - latMin);
            effect.Scale = mercatorTransform.Scale;

            return effect;
        }

        bool renderingPending = false;
        protected override void BeginInvalidateVisualCore()
        {
            OnRender();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
        }
    }
}
