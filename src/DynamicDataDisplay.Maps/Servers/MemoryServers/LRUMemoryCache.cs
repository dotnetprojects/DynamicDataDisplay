using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers
{
	public class LRUMemoryCache : LRUMemoryCacheBase<BitmapSource>, ITileSystem
	{
		public LRUMemoryCache(string name)
			: base(name)
		{

		}
	}
}
