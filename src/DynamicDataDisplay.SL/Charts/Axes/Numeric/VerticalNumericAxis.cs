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

namespace Microsoft.Research.DynamicDataDisplaySilverLight.Charts.Axes.Numeric
{
    public class VerticalNumericAxis : NumericAxisControl, IPlotterElement, IAxis
    {
        private Plotter parentPlotter;

        public VerticalNumericAxis() {
            Loaded += new RoutedEventHandler(VerticalNumericAxis_Loaded);
            ScreenTicksChanged += new EventHandler(VerticalNumericAxis_ScreenTicksChanged);
        }

        void VerticalNumericAxis_ScreenTicksChanged(object sender, EventArgs e)
        {
            RaiseTicksChanged();
        }

        void VerticalNumericAxis_Loaded(object sender, RoutedEventArgs e)
        {
            RaiseTicksChanged();
        }

        #region IPlotterElement Members

        public void OnPlotterAttached(Plotter plotter)
        {
            parentPlotter = plotter;
            plotter.LeftPanel.Children.Add(this);
        }

        public void OnPlotterDetaching(Plotter plotter)
        {
            plotter.LeftPanel.Children.Remove(this);
            parentPlotter = null;
        }

        public Plotter Plotter
        {
            get { return parentPlotter; }
        }

        #endregion

        private void RaiseTicksChanged()
        {
            if (TicksChanged != null)
            {
                TicksChanged(this, EventArgs.Empty);
            }
        }
        public event EventHandler TicksChanged;
    }
}
