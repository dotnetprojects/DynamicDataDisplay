using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DynamicDataDisplay.Test
{
	[TestClass]
	public class DegreeTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void DegreesTest()
		{
			Degree target = new Degree(20.5);
			Assert.AreEqual(20, target.Degrees);
			Assert.AreEqual(30, target.Minutes);
			Assert.AreEqual(0, target.Seconds);

			target = new Degree(-(10 + 21.0 / 60 + 34.22 / 3600));
			Assert.AreEqual(-10, target.Degrees);
			Assert.AreEqual(21, target.Minutes);
			Assert.AreEqual(34, target.Seconds);
			Assert.AreEqual(34.22, Math.Round(target.TotalSeconds, 3));

			target = new Degree(37.85);
			Assert.AreEqual(37, target.Degrees);
		}

		[TestMethod]
		public void DegreesFromComponents()
		{
			Degree degree = new Degree(10, 20, 30, CoordinateType.Latitude);
			Assert.AreEqual(degree.CoordinateType, CoordinateType.Latitude);
			Assert.AreEqual(degree.Degrees, 10);
			Assert.AreEqual(degree.Minutes, 20);
			Assert.AreEqual(degree.Seconds, 30);
		}
	}
}
