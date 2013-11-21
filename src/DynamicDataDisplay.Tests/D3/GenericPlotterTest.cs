using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Tests.D3
{
	[TestClass]
	public class GenericPlotterTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void TestReversibility()
		{
			ChartPlotter plotter = new ChartPlotter();
			var genericPlotter = plotter.GetGenericPlotter<double, double>();
			genericPlotter.DataRect = new GenericRect<double, double>(0, 0, 2, 2);

			Assert.IsTrue(plotter.Visible == new DataRect(0, 0, 2, 2));
			Assert.IsTrue(genericPlotter.DataRect == new GenericRect<double, double>(0, 0, 2, 2));
		}
	}
}
