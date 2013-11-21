using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Markup;
using Microsoft.Research.DynamicDataDisplay.Navigation;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace DynamicDataDisplay.Test.D3
{
	[TestClass]
	public class PlotterXamlSerializationTest
	{
		public TestContext TestContext { get; set; }

		// this method now tests wrongly, as ChartPlotter.CreateEmpty() was changed;
		//[TestMethod]
		//public void TestXamlAreEqual()
		//{
		//    ChartPlotter plotter = new ChartPlotter();
		//    plotter.Children.Clear();

		//    string plotterXaml = XamlWriter.Save(plotter);

		//    ChartPlotter empty = ChartPlotter.CreateEmpty();
		//    empty.Children.Clear();

		//    string emptyXaml = XamlWriter.Save(empty);

		//    Assert.AreEqual(plotterXaml, emptyXaml);
		//}

		[TestMethod]
		public void TestDeserialization()
		{
			ChartPlotter plotter = new ChartPlotter();

			string xaml = XamlWriter.Save(plotter);

			ChartPlotter plotter2 = (ChartPlotter)XamlReader.Parse(xaml);
		}

		[TestMethod]
		public void TestNonEmptySerialization()
		{
			ChartPlotter plotter = new ChartPlotter();

			string xaml = XamlWriter.Save(plotter);
		}
	}
}
