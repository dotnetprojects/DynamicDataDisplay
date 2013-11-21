using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Navigation
{
    public sealed class LongOperationsIndicator : IPlotterElement
    {
        public LongOperationsIndicator()
        {
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateWaitIndicator();
            timer.Stop();
        }

        #region IPlotterElement Members

        void IPlotterElement.OnPlotterAttached(Plotter plotter)
        {
            this.plotter = plotter;
        }

        void IPlotterElement.OnPlotterDetaching(Plotter plotter)
        {
            this.plotter = null;
        }

        private Plotter plotter;
        Plotter IPlotterElement.Plotter
        {
            get { return plotter; }
        }

        #endregion

        #region LongOperationRunning

        public static void BeginLongOperation(DependencyObject obj)
        {
            obj.SetValue(LongOperationRunningProperty, true);
        }

        public static void EndLongOperation(DependencyObject obj)
        {
            obj.SetValue(LongOperationRunningProperty, false);
        }

        public static bool GetLongOperationRunning(DependencyObject obj)
        {
            return (bool)obj.GetValue(LongOperationRunningProperty);
        }

        public static void SetLongOperationRunning(DependencyObject obj, bool value)
        {
            obj.SetValue(LongOperationRunningProperty, value);
        }

        public static readonly DependencyProperty LongOperationRunningProperty = DependencyProperty.RegisterAttached(
          "LongOperationRunning",
          typeof(bool),
          typeof(LongOperationsIndicator),
          new FrameworkPropertyMetadata(false, OnLongOperationRunningChanged));

        private static void OnLongOperationRunningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Plotter plotter = null;
            IPlotterElement element = d as IPlotterElement;
            if (element == null)
            {
                plotter = Plotter.GetPlotter(d);
            }
            else
            {
                plotter = element.Plotter;
            }

            if (plotter != null)
            {
                var indicator = plotter.Children.OfType<LongOperationsIndicator>().FirstOrDefault();
                if (indicator != null)
                {
                    indicator.OnLongOperationRunningChanged(element, (bool)e.NewValue);
                }
            }
        }

        UIElement indicator = LoadIndicator();

        private static UIElement LoadIndicator()
        {
            var resources = (ResourceDictionary)Application.LoadComponent(new Uri("/DynamicDataDisplay;component/Charts/Navigation/LongOperationsIndicatorResources.xaml", UriKind.Relative));
            UIElement indicator = (UIElement)resources["Indicator"];
            return indicator;
        }

        private readonly DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        private int operationsCounter = 0;
        private void OnLongOperationRunningChanged(IPlotterElement element, bool longOperationRunning)
        {
            int change = longOperationRunning ? +1 : -1;
            operationsCounter += change;

            if (plotter == null) return;

            if (operationsCounter == 1)
            {
                timer.Start();
            }
            else if (operationsCounter == 0)
            {
                timer.Stop();
                UpdateWaitIndicator();
            }
        }

        private void UpdateWaitIndicator()
        {
            if (operationsCounter == 1)
            {
                if (!plotter.MainCanvas.Children.Contains(indicator))
                {
                    plotter.MainCanvas.Children.Add(indicator);
                }
                plotter.Cursor = Cursors.Wait;
            }
            else if (operationsCounter == 0)
            {
                plotter.MainCanvas.Children.Remove(indicator);
                plotter.ClearValue(Plotter.CursorProperty);
            }
        }

        #endregion // end of LongOperationRunning
    }
}
