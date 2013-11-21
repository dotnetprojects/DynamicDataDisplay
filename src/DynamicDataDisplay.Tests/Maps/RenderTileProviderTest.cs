using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.TiledRendering;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;

namespace DynamicDataDisplay.Tests.Maps
{
	[TestClass]
	public class RenderTileProviderTest
	{
		[TestMethod]
		public void TestGetTiles()
		{
			RenderTileProvider provider = new RenderTileProvider();
			var tiles = provider.GetTilesForRegion(new DataRect(0, 0, 1, 1), provider.Level).ToArray();
		}
	}
}
