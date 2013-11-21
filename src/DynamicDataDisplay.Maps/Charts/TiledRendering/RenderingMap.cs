using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers;
using System.Collections;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.TiledRendering
{
	/// <summary>
	/// Represents a sort of map that renders contents of some visual element to tiles and the displays this tile pyramid.
	/// </summary>
	public class RenderingMap : Map
	{
		private readonly RenderTileServer renderTileServer = new RenderTileServer();
		public RenderTileServer RenderTileServer
		{
			get { return renderTileServer; }
		} 

		/// <summary>
		/// Initializes a new instance of the <see cref="RenderingMap"/> class.
		/// </summary>
		public RenderingMap()
		{
			TileProvider = new RenderTileProvider();
			SourceTileServer = renderTileServer;
			FileTileServer = new AutoDisposableFileServer();

			renderTileServer.RenderingPlotter.SetBinding(FrameworkElement.DataContextProperty, new Binding("DataContext") { Source = this });
		}

		protected override void OnSourceServerChanged(object sender, ValueChangedEventArgs<SourceTileServer> e)
		{
			base.OnSourceServerChanged(sender, e);

			UpdateContentBounds();
		}

		private void UpdateContentBounds()
		{
			IRenderingTileServer renderingServer = SourceTileServer as IRenderingTileServer;
			if (renderingServer != null)
			{
				Viewport2D.SetContentBounds(this, renderingServer.ContentBounds);
			}
		}

		protected override void OnPlotterAttached(Plotter plotter)
		{
			// removing restrictions previously added by base class Map, as in case of rendering to texture they
			// are not relevant
			Plotter.Viewport.Restrictions.Remove(base.ProportionsRestriction);
			Plotter.Viewport.Restrictions.Remove(base.MaxSizeRestriction);
		}

		HashSet<double> levels = new HashSet<double>();
		private double prevLevel;
		protected override void UpdateLevel(CoordinateTransform transform)
		{
			var visible = Plotter.Viewport.Visible;
			var output = Plotter.Viewport.Output;

			prevLevel = TileProvider.Level;

			var num = output.Height / 512;
			double tileHeight = visible.Height / num;
			double level = -Math.Log(tileHeight, 2);

			level = RoundLevel(level);

			levels.Add(level);
			TileProvider.Level = level;

			if (level != prevLevel)
			{
				TileSystem.SourceServer.CancelRunningOperations();
			}
		}

		private double RoundLevel(double level)
		{
			double result = Math.Round(level * 4) / 4;
			return result;
		}

		protected override IEnumerable<LowerTileInfo> GetLoadedLowerTiles(IEnumerable<VisibleTileInfo> visibleTiles)
		{
			var memoryCache = (LRUMemoryCache)TileSystem.MemoryServer;
			var sourceServer = TileSystem.SourceServer;

			var actualVisibleTiles = visibleTiles.Where(tile => sourceServer.Contains(tile.Tile)).ToList();

			var currLevel = TileProvider.Level;
			var upperLevel = GetAvailableUpperLevel(currLevel);
			var cache = (ICollection<TileIndex>)memoryCache.GetCachedIndexes();

			var region = GetRegion(actualVisibleTiles);

			var server = TileSystem;
			var lowerTiles = TileProvider.GetTilesForRegion(region, upperLevel).Where(tile => server.IsLoaded(tile));

			var plotterTransform = Plotter.Transform;

			var lowerTileList = new List<LowerTileInfo>();
			foreach (var lowerTile in lowerTiles)
			{
				bool addToLowerTiles = false;
				var bounds = TileProvider.GetTileBounds(lowerTile);
				var shiftedScreenBounds = bounds.ViewportToScreen(plotterTransform);

				Geometry clip = new RectangleGeometry(shiftedScreenBounds);
				foreach (var tile in actualVisibleTiles)
				{
					if (tile.VisibleBounds.IntersectsWith(bounds))
					{
						if (!cache.Contains(tile.Tile))
						{
							addToLowerTiles = true;
						}
						else
						{
							var screenBounds = tile.VisibleBounds.ViewportToScreen(plotterTransform);
							clip = new CombinedGeometry(GeometryCombineMode.Exclude, clip, new RectangleGeometry(screenBounds));
						}
					}
				}

				if (addToLowerTiles)
					lowerTileList.Add(new LowerTileInfo { Id = lowerTile, Clip = clip });
			}

			return lowerTileList;
		}

		private DataRect GetRegion(IEnumerable<VisibleTileInfo> visibleTiles)
		{
			var visibleBoundsSeq = visibleTiles.Select(t => t.VisibleBounds);
			if (visibleBoundsSeq.Count() == 0)
				return DataRect.Empty;
			else if (visibleBoundsSeq.Count() == 1)
				return visibleBoundsSeq.First();
			else return visibleBoundsSeq.Aggregate((r1, r2) => DataRect.Union(r1, r2));
		}

		private double GetAvailableUpperLevel(double currLevel)
		{
			var memoryCache = (LRUMemoryCache)TileSystem.MemoryServer;
			var cache = (ICollection<TileIndex>)memoryCache.GetCachedIndexes();

			return levels.OrderBy(d => -d).FirstOrDefault(level => level < currLevel && cache.Where(id => id.Level == level).Count() >= 3);
		}
	}
}
