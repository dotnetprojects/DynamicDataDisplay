using Microsoft.Research.DynamicDataDisplay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;
using System.Threading;
using System.Security.Permissions;
using System;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace DynamicDataDisplay.Test
{
	[TestClass]
	public class AxisTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void HorizontalAxisTest()
		{
			ChartPlotter target = new ChartPlotter();
			var expected = new HorizontalAxis();
			GeneralAxis actual;
			target.MainHorizontalAxis = expected;
			actual = target.MainHorizontalAxis;
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void VerticalAxisTest()
		{
			ChartPlotter plotter = new ChartPlotter();
			var expected = new VerticalAxis();
			GeneralAxis actual;
			plotter.MainVerticalAxis = expected;
			actual = plotter.MainVerticalAxis;
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void HorizontalAxisIsDefaultTest()
		{
			ChartPlotter plotter = new ChartPlotter();

			HorizontalAxis axis = (HorizontalAxis)plotter.MainHorizontalAxis;
			HorizontalAxis axis2 = new HorizontalAxis();
			plotter.Children.Add(axis2);

			Assert.AreEqual(plotter.MainHorizontalAxis, axis);
			Assert.IsTrue(axis.IsDefaultAxis);

			axis2.IsDefaultAxis = true;
			Assert.AreEqual(plotter.MainHorizontalAxis, axis2);
			Assert.IsFalse(axis.IsDefaultAxis);

			axis.IsDefaultAxis = true;
			Assert.AreEqual(plotter.MainHorizontalAxis, axis);
			Assert.IsFalse(axis2.IsDefaultAxis);
		}

		[TestMethod]
		public void VerticalAxisIsDefaultTest()
		{
			ChartPlotter plotter = new ChartPlotter();

			VerticalAxis axis = (VerticalAxis)plotter.MainVerticalAxis;
			VerticalAxis axis2 = new VerticalAxis();
			plotter.Children.Add(axis2);

			Assert.AreEqual(plotter.MainVerticalAxis, axis);
			Assert.IsTrue(axis.IsDefaultAxis);

			axis2.IsDefaultAxis = true;
			Assert.AreEqual(plotter.MainVerticalAxis, axis2);
			Assert.IsFalse(axis.IsDefaultAxis);

			axis.IsDefaultAxis = true;
			Assert.AreEqual(plotter.MainVerticalAxis, axis);
			Assert.IsFalse(axis2.IsDefaultAxis);
		}
	}
}
