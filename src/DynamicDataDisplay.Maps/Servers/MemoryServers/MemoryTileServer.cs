using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	public sealed class MemoryTileServer : TileServerBase, ITileSystem	{
		public MemoryTileServer(string name)
		{
			ServerName = name;
		}

		private readonly Dictionary<TileIndex, BitmapSource> cache = new Dictionary<TileIndex, BitmapSource>(new TileIndex.TileIndexEqualityComparer());

		public override bool Contains(TileIndex id)
		{
			return cache.ContainsKey(id);
		}

		public override void BeginLoadImage(TileIndex id)
		{
			if (Contains(id))
				ReportSuccess(cache[id], id);
			else
				ReportFailure(id);
		}

		public BitmapSource this[TileIndex id]
		{
			get
			{
				return cache[id];
			}
		}

		#region ITileStore Members

		public void BeginSaveImage(TileIndex id, BitmapSource image, Stream stream)
		{
			cache[id] = image;
		}

		#endregion

        #region ITileStore Members


        public void Clear()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
