using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace DynamicDataDisplay.Tests.D3
{
    [TestClass]
    public class FrequencyFilterTest
    {
        [TestMethod]
        public void TestFilteringLastPoint()
        {
            List<Point> unfiltered = new List<Point>();
            unfiltered.Add(new Point(0, 0));
            unfiltered.Add(new Point(1, 0.1));
            unfiltered.Add(new Point(-1, 0.2));
            unfiltered.Add(new Point(0.5, 0.22));
            unfiltered.Add(new Point(0.4, 0.23));
            unfiltered.Add(new Point(2, 0));

            FrequencyFilter filter = new FrequencyFilter();
            filter.SetScreenRect(new Rect(0, 0, 2, 10));
            var filtered = filter.Filter(unfiltered);

            Assert.IsTrue(filtered[filtered.Count - 1] == new Point(2, 0));
        }
    }
}
