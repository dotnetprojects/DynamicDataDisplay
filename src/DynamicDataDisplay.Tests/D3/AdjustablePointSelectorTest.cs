using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Charts.Selectors;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Tests.D3
{
	[TestClass]
	public class AdjustablePointSelectorTest
	{
		[TestMethod]
		public void TestMaxPointsCount()
		{
			AdjustablePointSelector selector = new AdjustablePointSelector();

			selector.Points.Add(new Point());
			selector.Points.Add(new Point());
			selector.Points.Add(new Point());

			selector.MaxPointsCount = 3;
			Assert.IsTrue(selector.Points.Count == 3);

			selector.MaxPointsCount = 2;
			Assert.IsTrue(selector.Points.Count == 2);

			selector.MaxPointsCount = 1;
			Assert.IsTrue(selector.Points.Count == 1);
			Assert.IsFalse(selector.AddPointCommand.CanExecute(new Point()));

			selector.MaxPointsCount = 2;
			Assert.IsTrue(selector.Points.Count == 1);
			Assert.IsTrue(selector.AddPointCommand.CanExecute(new Point()));

			selector.MaxPointsCount = 0;
			Assert.IsTrue(selector.Points.Count == 0);
			Assert.IsFalse(selector.AddPointCommand.CanExecute(new Point()));

			bool thrown = false;
			try
			{
				selector.MaxPointsCount = -1;
			}
			catch (ArgumentException)
			{
				thrown = true;
			}

			Assert.IsTrue(thrown);
		}
	}
}
