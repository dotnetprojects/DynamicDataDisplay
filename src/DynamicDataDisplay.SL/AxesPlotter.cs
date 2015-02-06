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
using Microsoft.Research.DynamicDataDisplaySilverLight.Charts;
using Microsoft.Research.DynamicDataDisplaySilverLight.Charts.Axes.Numeric;

namespace Microsoft.Research.DynamicDataDisplaySilverLight
{
    public class AxesPlotter : Plotter
    {
        private HorizontalNumericAxis HorizontalAxis;
        private VerticalNumericAxis VerticalAxis;
        private AxisGrid axisGrid = new AxisGrid();

        
        public AxesPlotter() {
            HorizontalAxis = new HorizontalNumericAxis();
            HorizontalAxis.TicksChanged += new EventHandler(HorizontalAxis_TicksChanged);

            VerticalAxis = new VerticalNumericAxis();
            VerticalAxis.TicksChanged += new EventHandler(VerticalAxis_TicksChanged);

            HorizontalAxis.Placement = AxisPlacement.Bottom;
            VerticalAxis.Range = new Range<double>(-3.4, 3.2);
            HorizontalAxis.Range = new Range<double>(0.5,69.3);
            VerticalAxis.Placement = AxisPlacement.Left;
            Loaded += new RoutedEventHandler(AxesPlotter_Loaded);

        }

        void VerticalAxis_TicksChanged(object sender, EventArgs e)
        {
            IAxis axis = sender as IAxis;
            UpdateVerticalTicks(axis);
        }

        void HorizontalAxis_TicksChanged(object sender, EventArgs e)
        {
            IAxis axis = sender as IAxis;
            UpdateHorizontalTicks(axis);
        }

        void AxesPlotter_Loaded(object sender, RoutedEventArgs e)
        {
            Children.Add(HorizontalAxis);
            Children.Add(VerticalAxis);
            Children.Add(axisGrid);
        }

        private void UpdateHorizontalTicks(IAxis axis)
        {
            axisGrid.BeginTicksUpdate();

            if (axis != null)
            {
                axisGrid.HorizontalTicks = axis.ScreenTicks;
                axisGrid.MinorHorizontalTicks = axis.MinorScreenTicks;
            }
            else
            {
                axisGrid.HorizontalTicks = null;
                axisGrid.MinorHorizontalTicks = null;
            }

            axisGrid.EndTicksUpdate();
        }

        private void UpdateVerticalTicks(IAxis axis)
        {
            axisGrid.BeginTicksUpdate();

            if (axis != null)
            {
                axisGrid.VerticalTicks = axis.ScreenTicks;
                axisGrid.MinorVerticalTicks = axis.MinorScreenTicks;
            }
            else
            {
                axisGrid.VerticalTicks = null;
                axisGrid.MinorHorizontalTicks = null;
            }

            axisGrid.EndTicksUpdate();
        }
    }
}
