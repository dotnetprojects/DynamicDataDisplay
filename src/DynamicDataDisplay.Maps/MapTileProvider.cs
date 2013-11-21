using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	public class MapTileProvider : TileProvider
	{
		public MapTileProvider()
		{
			rect = DataRect.FromPoints(minX, minY, maxX, maxY);
		}

		private double minX = -180;
		private double maxX = 180;
		private double minY = -87;
		private double maxY = 87;
		private DataRect rect;

		public double MaxLatitude
		{
			get { return maxY; }
			set
			{
				maxY = value;
				minY = -value;

				rect = DataRect.FromPoints(minX, minY, maxX, maxY);
			}
		}

		private double maxShaderLatitude = 85;
		public double MaxShaderLatitude
		{
			get { return maxShaderLatitude; }
			set { maxShaderLatitude = value; }
		}

		public double MinLatitude
		{
			get { return minY; }
		}

		public double XSize { get { return maxX - minX; } }
		public double YSize { get { return maxY - minY; } }

		public override DataRect GetTileBounds(TileIndex tile)
		{
			if (tile.Level == 0)
				return rect;

			double width = GetTileWidth(tile.Level);
			double height = GetTileHeight(tile.Level);
			double x = 0 + tile.X * width;
			double y = /*minY*/0 + tile.Y * height;

			DataRect bounds = new DataRect(x, y, width, height);
			return bounds;
		}

		private bool cyclingX = true;
		public bool CyclingX
		{
			get { return cyclingX; }
			set { cyclingX = value; }
		}

		// todo rewrite
		public static IEnumerable<TileIndex> GetTilesForLevel(double level)
		{
			int size = GetSideTilesCount(level);
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					yield return new TileIndex(x, y, level);
				}
			}
		}

		public static DataRect GetTileBoundsGeneric(TileIndex tile)
		{
			double width = 360.0 / Math.Pow(2, tile.Level);
			double height = 174.0 / Math.Pow(2, tile.Level);
			double x = /*minX*/0 + tile.X * width;
			double y = /*minY*/0 + tile.Y * height;

			DataRect bounds = new DataRect(x, y, width, height);
			return bounds;
		}

		public static long GetTotalTilesCount(double level)
		{
			long size = GetSideTilesCount(level);
			return size * size;
		}

		public static int GetSideTilesCount(double level)
		{
			return (int)Math.Pow(2, level);
		}

		public double TileWidth { get { return GetTileWidth(Level); } }
		public double TileHeight { get { return GetTileHeight(Level); } }

		public override double GetTileWidth(double level)
		{
			return XSize / Math.Pow(2, level);
		}

		public override double GetTileHeight(double level)
		{
			return YSize / Math.Pow(2, level);
		}

		public override IEnumerable<TileIndex> GetTilesForRegion(DataRect region, double level)
		{
			region.Intersect(rect);
			//region.Intersect(new Rect(region.XMin, minY, region.Width, maxY - minY));
			if (region.IsEmpty)
				yield break;

			checked
			{
				double tileWidth = TileWidth;
				double tileHeight = TileHeight;

				int minIx = (int)Math.Floor(region.XMin / tileWidth);
				int maxIx = (int)Math.Ceiling(region.XMax / tileWidth);

				int minIy = (int)Math.Floor(region.YMin / tileHeight);
				int maxIy = (int)Math.Ceiling(region.YMax / tileHeight);

				var maxSideCount = GetSideTilesCount(Level);

				int maxIndex = maxSideCount / 2;
				if (maxIx > maxIndex)
					maxIx = maxIndex;
				if (maxIy > maxIndex)
					maxIy = maxIndex;
				if (minIx < -maxIndex)
					minIx = -maxIndex;
				if (minIy < -maxIndex)
					minIy = -maxIndex;

				if (level != 0)
				{
					maxIx--;
					maxIy--;
				}


				for (int ix = minIx; ix <= maxIx; ix++)
				{
					for (int iy = minIy; iy <= maxIy; iy++)
					{
						yield return new TileIndex(ix, iy, level);
					}
				}
			}
		}

		[Obsolete("Unready")]
		public static TileIndex Normalize(TileIndex id)
		{
			int actualX = id.X % GetSideTilesCount(id.Level);
			TileIndex res = new TileIndex(actualX, id.Y, id.Level);
			return res;
		}
	}
}
