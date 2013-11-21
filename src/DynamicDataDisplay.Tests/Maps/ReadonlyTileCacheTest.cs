using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DynamicDataDisplay.Test
{
	/// <summary>
	///This is a test class for ReadonlyTileCacheTest and is intended
	///to contain all ReadonlyTileCacheTest Unit Tests
	///</summary>
	[TestClass]
	public class ReadonlyTileCacheTest
	{
		public TestContext TestContext { get; set; }

		/// <summary>
		///A test for Add
		///</summary>
		[TestMethod]
		public void AddTest()
		{
			ReadonlyTileCache target = new ReadonlyTileCache();

			for (int z = 0; z < 16; z++)
			{
				target.Add(new TileIndex(0, 0, z), true);
				Assert.IsTrue(target.Contains(new TileIndex(0, 0, z)));
			}

			for (int z = 0; z < 16; z++)
			{
				target.Add(new TileIndex(0, 0, z), true);
				Assert.IsFalse(target.Contains(new TileIndex(1, 0, z)));
			}

			var id = new TileIndex(7, 7, 1);
			target.Add(id, true);
			Assert.IsTrue(target.Contains(id));

			target.Add(id, false);
			Assert.IsFalse(target.Contains(id));
		}
	}
}
