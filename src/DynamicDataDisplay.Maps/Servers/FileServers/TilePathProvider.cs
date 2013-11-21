using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.IO;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers.FileServers
{
	public abstract class TilePathProvider
	{
		public abstract string GetTilePath(TileIndex id);

		public List<FoundTileInfo> GetTiles(string directoryName, string fileExtension)
		{
			DirectoryInfo directory = new DirectoryInfo(directoryName);
			var files = directory.GetFiles("*" + fileExtension);

			List<FoundTileInfo> foundIndexes = new List<FoundTileInfo>();
			foreach (var file in files)
			{
				TileIndex id;
				if (TryParse(file.Name, out id))
				{
					FoundTileInfo info = new FoundTileInfo { ID = id, Path = file.Name };
					foundIndexes.Add(info);
				}
			}

			return foundIndexes;
		}

		public bool TryParse(string fileName, out TileIndex index)
		{
			string name = fileName.Substring(0, fileName.IndexOf('.'));

			int x = 0;
			int y = 0;
			int level = 0;
			foreach (char ch in name)
			{
				switch (ch)
				{
					case '0':
						break;
					case '1':
						x++;
						break;
					case '2':
						y++;
						break;
					case '3':
						x++;
						y++;
						break;
					default:
						index = new TileIndex();
						return false;
				}

				level++;
				x *= 2;
				y *= 2;
			}

			y = MapTileProvider.GetSideTilesCount(level) - y;
			index = new TileIndex(x, y, level);

			return true;
		}
	}

	public sealed class FoundTileInfo
	{
		public TileIndex ID { get; set; }
		public string Path { get; set; }
	}
}
