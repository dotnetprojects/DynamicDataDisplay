using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Charts.Selectors;

namespace DynamicDataDisplay.Tests.D3
{
	[TestClass]
	public class PointSelectorTest
	{
		[TestMethod]
		public void TestModeChangeWhileDisconnected()
		{
			PointSelector selector = new PointSelector();
			selector.Mode = PointSelectorMode.Add;
			selector.Mode = PointSelectorMode.MultipleSelect;
			selector.Mode = PointSelectorMode.Remove;
		}
	}
}
