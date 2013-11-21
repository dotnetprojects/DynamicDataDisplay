using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows;

namespace DynamicDataDisplay.Test.D3
{
    [TestClass]
    public class ViewportTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestScreenRect()
        {
            ChartPlotter plotter = new ChartPlotter { Width = 200, Height = 100 };
            var img = plotter.CreateScreenshot();

            Rect screenRect = plotter.Viewport.Output;
            Assert.IsTrue(screenRect.Width > 0 && screenRect.Height > 0);
        }

        [TestMethod]
        public void ShouldThrowOnNanInVisible()
        {
            ChartPlotter plotter = new ChartPlotter();
            bool thrown = false;
            try
            {
                plotter.Visible = new Rect(Double.NaN, 0, 1, 1);
            }
            catch (ArgumentException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void TestViewportContentBounds()
        {
            ChartPlotter plotter = new ChartPlotter();
            TempIPlotterElement element = new TempIPlotterElement();
            plotter.Children.Add(element);
            plotter.PerformLoad();
            plotter.Viewport.ClipToBoundsEnlargeFactor = 1.0;

            Assert.AreEqual(new DataRect(0, 0, 1, 1), plotter.Visible);

            Viewport2D.SetContentBounds(element, new DataRect(0, 0, 2, 2));

            Assert.AreEqual(new DataRect(0, 0, 2, 2), plotter.Visible);

            plotter.Children.Remove(element);

            Assert.AreEqual(new DataRect(0, 0, 1, 1), plotter.Visible);

            plotter.Children.Add(element);
            Assert.AreEqual(new DataRect(0, 0, 2, 2), plotter.Visible);

            Viewport2D.SetIsContentBoundsHost(element, false);
            Assert.AreEqual(new DataRect(0, 0, 1, 1), plotter.Visible);
        }

        private class TempIPlotterElement : DependencyObject, IPlotterElement
        {
            public TempIPlotterElement()
            {
            }

            #region IPlotterElement Members

            private Plotter plotter;
            public void OnPlotterAttached(Plotter plotter)
            {
                this.plotter = plotter;
            }

            public void OnPlotterDetaching(Plotter plotter)
            {
                this.plotter = null;
            }

            public Plotter Plotter
            {
                get { return plotter; }
            }

            #endregion
        }
    }
}
