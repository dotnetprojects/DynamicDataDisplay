using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	public sealed class ReadonlyTileCache
	{
		private readonly Dictionary<TileIndex, ulong> cache = new Dictionary<TileIndex, ulong>(new TileIndex.TileIndexEqualityComparer());

		public ReadonlyTileCache() { }

		public ReadonlyTileCache(Dictionary<TileIndex, ulong> dictionary)
		{
			this.cache = dictionary;

			CalcMinMaxLevels();
		}

		private const int levelDelta = 3;
		private const int levelSize = 8; // 3 << 2

		public void CalcMinMaxLevels()
		{
			minLevel = Int32.MaxValue;
			maxLevel = Int32.MinValue;

			foreach (var key in cache.Keys)
			{
				int level = (int)key.Level + levelDelta;

				if (minLevel > level)
					minLevel = level;
				if (maxLevel < level)
					maxLevel = level;
			}
		}

		private int minLevel = 0;
		public int MinLevel
		{
			get { return minLevel; }
		}

		private int maxLevel = Int32.MaxValue;
		public int MaxLevel
		{
			get { return maxLevel; }
		}

		public void Add(TileIndex id, bool exists)
		{
			ulong cacheLine = 0;
			TileIndex parent = id.GetLowerTile(levelDelta);

			cache.TryGetValue(parent, out cacheLine);

			int x = id.X - parent.X * levelSize;
			int y = id.Y - parent.Y * levelSize;

			int position = x * levelSize + y;
			if (exists)
			{
				cacheLine |= ((ulong)1) << position;
			}
			else
			{
				cacheLine &= ~(((ulong)1) << position);
			}

			cache[parent] = cacheLine;
		}

		public bool Contains(TileIndex id)
		{
			int level = (int)id.Level;
			if (level < minLevel || level > maxLevel)
				return false;

			TileIndex parent = id.GetLowerTile(levelDelta);

			ulong cacheLine;
			if (cache.TryGetValue(parent, out cacheLine))
			{
				int x = id.X - parent.X * levelSize;
				int y = id.Y - parent.Y * levelSize;

				int position = x * levelSize + y;

				return (cacheLine & ((ulong)1 << position)) != 0;
			}
			return false;
		}

		public int Count
		{
			get { return cache.Count; }
		}
	}
}
