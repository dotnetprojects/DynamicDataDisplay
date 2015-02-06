using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay
{
    public class HorizontalAxisTitle : ContentControl,IPlotterElement
    {
        private Plotter plotter = null;

        public HorizontalAxisTitle()
        {
            FontSize = 16;
            HorizontalAlignment = HorizontalAlignment.Center;
        }

        #region IPlotterElement Members

        public void OnPlotterAttached(Plotter plotter)
        {
            if (this.plotter == null)
            {
                this.plotter = plotter;
                plotter.BottomPanel.Children.Add(this);
            }
        }

        public void OnPlotterDetaching(Plotter plotter)
        {
            if (this.plotter != null)
            {
                this.plotter = null;
                plotter.BottomPanel.Children.Remove(this);
            }
        }

        public Plotter Plotter
        {
            get { return plotter; }
        }

        #endregion
    }
}
