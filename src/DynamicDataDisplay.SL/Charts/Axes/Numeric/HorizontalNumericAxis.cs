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
    public class HorizontalNumericAxis : NumericAxisControl, IPlotterElement,IAxis
    {
        private Plotter parentPlotter;

        public HorizontalNumericAxis() {
            Loaded += new RoutedEventHandler(HorizontalNumericAxis_Loaded);
            ScreenTicksChanged += new EventHandler(HorizontalNumericAxis_ScreenTicksChanged);
        }

        void HorizontalNumericAxis_ScreenTicksChanged(object sender, EventArgs e)
        {
            RaiseTicksChanged();
        }

        void HorizontalNumericAxis_Loaded(object sender, RoutedEventArgs e)
        {
            RaiseTicksChanged();
        }

        #region IPlotterElement Members

        public void OnPlotterAttached(Plotter plotter)
        {
            parentPlotter = plotter;
            plotter.BottomPanel.Children.Insert(0,this);
        }

        public void OnPlotterDetaching(Plotter plotter)
        {
            parentPlotter = null;
            plotter.BottomPanel.Children.Remove(this);
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
