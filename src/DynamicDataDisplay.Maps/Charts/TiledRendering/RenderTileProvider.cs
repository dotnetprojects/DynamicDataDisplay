using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.TiledRendering
{
	public sealed class RenderTileProvider : TileProvider
	{
		public RenderTileProvider()
		{
			MinLevel = Int32.MinValue;
			MaxLevel = Int32.MaxValue;
		}

		public override DataRect GetTileBounds(TileIndex id)
		{
			double tileSide = GetTileSide(id.Level);
			DataRect result = new DataRect(id.X * tileSide, id.Y * tileSide, tileSide, tileSide);
			return result;
		}

		private double GetTileSide(double level)
		{
			// we are assuming that tile in level 0 is a square of 1*1 units in viewport coordinates
			return Math.Pow(2, -level);
		}

		public override double GetTileWidth(double level)
		{
			return GetTileSide(level);
		}

		public override double GetTileHeight(double level)
		{
			return GetTileSide(level);
		}

		public override IEnumerable<TileIndex> GetTilesForRegion(DataRect region, double level)
		{
			if (region.IsEmpty)
				yield break;

			double tileSide = GetTileSide(level);

			int minIx = (int)Math.Floor(region.XMin / tileSide);
			int maxIx = (int)Math.Ceiling(region.XMax / tileSide);

			int minIy = (int)Math.Floor(region.YMin / tileSide);
			int maxIy = (int)Math.Ceiling(region.YMax / tileSide);

			for (int ix = minIx; ix < maxIx; ix++)
			{
				for (int iy = minIy; iy < maxIy; iy++)
				{
					yield return new TileIndex(ix, iy, level);
				}
			}
		}
	}
}
