using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.Research.DynamicDataDisplay.Controls;

namespace DynamicDataDisplay.Test
{


	/// <summary>
	///This is a test class for PaletteControlTest and is intended
	///to contain all PaletteControlTest Unit Tests
	///</summary>
	[TestClass()]
	public class PaletteControlTest
	{

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		/// <summary>
		///A test for Palette
		///</summary>
		[TestMethod()]
		public void PaletteTest()
		{
			PaletteControl target = new PaletteControl();
			target.Palette = null;
		}
	}
}
