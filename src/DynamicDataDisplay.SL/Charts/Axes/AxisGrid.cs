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
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay
{
    /// <summary>This element draws grid over viewport. Number of 
    /// grid lines depends on horizontal and vertical axes</summary>
    public class AxisGrid : ContentControl, IPlotterElement
    {
        bool shouldUpdate = true;
        internal void BeginTicksUpdate()
        {
            shouldUpdate = false;
        }
        internal void EndTicksUpdate()
        {
            shouldUpdate = true;
            UpdateUIRepresentation();
        }

        private MinorTickInfo<double>[] minorHorizontalTicks;
        public MinorTickInfo<double>[] MinorHorizontalTicks
        {
            get { return minorHorizontalTicks; }
            set
            {
                minorHorizontalTicks = value;
                if (drawHorizontals && shouldUpdate)
                {
                    UpdateUIRepresentation();
                }
            }
        }

        private MinorTickInfo<double>[] minorVerticalTicks;
        public MinorTickInfo<double>[] MinorVerticalTicks
        {
            get { return minorVerticalTicks; }
            set
            {
                minorVerticalTicks = value;
                if (drawVerticals && shouldUpdate)
                {
                    UpdateUIRepresentation();
                }
            }
        }

        private double[] horizontalTicks;
        public double[] HorizontalTicks
        {
            get { return horizontalTicks; }
            set
            {
                horizontalTicks = value;
                if (drawHorizontals && shouldUpdate)
                {
                    UpdateUIRepresentation();
                }
            }
        }

        private double[] verticalTicks;
        public double[] VerticalTicks
        {
            get { return verticalTicks; }
            set
            {
                verticalTicks = value;
                if (drawVerticals && shouldUpdate)
                {
                    UpdateUIRepresentation();
                }
            }
        }

        private bool drawVerticals = true;
        public bool DrawVerticals
        {
            get { return drawVerticals; }
            set
            {
                if (drawVerticals != value)
                {
                    drawVerticals = value;
                    UpdateUIRepresentation();
                }
            }
        }

        private bool drawHorizontals = true;
        public bool DrawHorizontals
        {
            get { return drawHorizontals; }
            set
            {
                if (drawHorizontals != value)
                {
                    drawHorizontals = value;
                    UpdateUIRepresentation();
                }
            }
        }

        private double gridBrushThickness = 1;

        private Path path = new Path();

        public AxisGrid()
        {
            Canvas panel = new Canvas();
            //panel.ClipToBounds = true;
            panel.Background = new SolidColorBrush(Colors.Cyan);
            panel.Children.Add(path);

            path.Stroke = new SolidColorBrush(Colors.LightGray);
            path.StrokeThickness = gridBrushThickness;

            Content = panel;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //return base.MeasureOverride(availableSize);
            //if (double.IsInfinity(availableSize.Height) || double.IsInfinity(availableSize.Width))
            //{
            //    return new Size(600,400);
            //}
            //else 
                return availableSize;
            
        }

        private void UpdateUIRepresentation()
        {
            GeometryGroup group = new GeometryGroup();

            Size size = new Size(ActualWidth,ActualHeight);
            PerformanceCounter.startStopwatch("AG UI rep: horizontan ticks");
            if (horizontalTicks != null)
            {
                double minY = 0;
                double maxY = size.Height;

                for (int i = 0; i < horizontalTicks.Length; i++)
                {
                    double screenX = horizontalTicks[i];
                    LineGeometry line = new LineGeometry();
                    line.StartPoint= new Point(screenX, minY);
                    line.EndPoint = new Point(screenX, maxY);
                    group.Children.Add(line);
                }
            }
            PerformanceCounter.stopStopwatch("AG UI rep: horizontan ticks");

            PerformanceCounter.startStopwatch("AG UI rep: vertical ticks");
            
            if (verticalTicks != null)
            {
                double minX = 0;
                double maxX = size.Width;

                for (int i = 0; i < verticalTicks.Length; i++)
                {
                    double screenY = verticalTicks[i];
                    LineGeometry line = new LineGeometry();
                    line.StartPoint =new Point(minX, screenY);
                    line.EndPoint =new Point(maxX, screenY);
                    group.Children.Add(line);
                }
            }
            PerformanceCounter.stopStopwatch("AG UI rep: vertical ticks");
            

            path.Data = group;
            }

        #region IPlotterElement Members
        public void OnPlotterAttached(Plotter plotter)
        {

            if (this.plotter == null)
            {
                this.plotter = plotter;
                plotter.CentralGrid.Children.Insert(0, this);
            }
        }

        public void OnPlotterDetaching(Plotter plotter)
        {
            if (this.plotter!=null)
            {
                plotter.CentralGrid.Children.Remove(this);
                this.plotter = null;
                }
        }

        private Plotter plotter = null;
        public Plotter Plotter
        {
            get { return plotter; }
        }

        #endregion
    }
}
