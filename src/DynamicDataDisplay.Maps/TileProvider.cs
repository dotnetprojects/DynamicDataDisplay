using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	public abstract class TileProvider
	{
		public abstract DataRect GetTileBounds(TileIndex id);

		public abstract double GetTileWidth(double level);

		public abstract double GetTileHeight(double level);

		public abstract IEnumerable<TileIndex> GetTilesForRegion(DataRect region, double level);


		private double level = 1;
		public double Level
		{
			get { return level; }
			set { level = value; }
		}

		public bool DecreaseLevel()
		{
			if (level > minLevel)
			{
				level--;
				return true;
			}
			return false;
		}

		public bool IncreaseLevel()
		{
			if (level < maxLevel)
			{
				level++;
				return true;
			}
			return false;
		}

		private int minLevel = 1;
		public int MinLevel
		{
			get { return minLevel; }
			set { minLevel = value; }
		}

		private int maxLevel = 17;
		public int MaxLevel
		{
			get { return maxLevel; }
			set { maxLevel = value; }
		}
	}
}
