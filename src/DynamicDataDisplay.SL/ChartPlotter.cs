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
using System.Linq;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric;
using Microsoft.Research.DynamicDataDisplay.Navigation;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay
{
    public class ChartPlotter : Plotter2D
    {
        private bool alreadyLoaded = false;
        private IAxis horizontalAxis = null;
        private IAxis verticalAxis = null;
        private AxisGrid axisGrid = new AxisGrid();
        private buttonsNavigation buttonsNavigation;
        private MouseNavigation mouseNavigation = new MouseNavigation();
        private Legend legend = null;
        private ToolTip tooltip = new ToolTip();
        private ChartPlotterSettings settings;
        
        public IAxis HorizontalAxis
        {
            get { return horizontalAxis; }
            set
            {
                if (horizontalAxis == value)
                    return;

                if (horizontalAxis != null)
                {
                    Children.Remove(horizontalAxis);
                    horizontalAxis.TicksChanged -= OnHorizontalAxisTicksChanged;
                    horizontalAxis = null;

                    //UpdateHorizontalTicks(horizontalAxis);
                }

                VerifyAxisType(value.Placement, AxisType.Horizontal);



               // updatingAxis = true;
                horizontalAxis = value;
                horizontalAxis.TicksChanged += OnHorizontalAxisTicksChanged;

                if (!Children.Contains(value))
                {
                    Children.Add(value);
                }

                UpdateHorizontalTicks(value);

                //updatingAxis = false;
            }
        }

        public IAxis VerticalAxis
        {
            get { return verticalAxis; }
            set
            {
                if (value == verticalAxis)
                    return;

                if (verticalAxis != null)
                {
                    Children.Remove(verticalAxis);
                    verticalAxis.TicksChanged -= OnVerticalAxisTicksChanged;
                }

                //VerifyAxisType(value.Placement, AxisType.Horizontal);

               

                   // updatingAxis = true;

                    

                    verticalAxis = value;
                    verticalAxis.TicksChanged += OnVerticalAxisTicksChanged;

                    if (!Children.Contains(value))
                    {
                        Children.Add(value);
                    }

                    UpdateVerticalTicks(value);

                    //updatingAxis = false;
                
            }
        }

        private static void VerifyAxisType(AxisPlacement axisPlacement, AxisType axisType)
        {
            bool result = false;
            switch (axisPlacement)
            {
                case AxisPlacement.Left:
                case AxisPlacement.Right:
                    result = axisType == AxisType.Vertical;
                    break;
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:
                    result = axisType == AxisType.Horizontal;
                    break;
                default:
                    break;
            }

            if (!result)
                throw new ArgumentException();
        }


        public ChartPlotter()
        {
            settings = new ChartPlotterSettings();
            Loaded += new RoutedEventHandler(ChartPlotter_Loaded);
        }

        public ChartPlotter(ChartPlotterSettings settings) {
            this.settings = settings;
            Loaded += new RoutedEventHandler(ChartPlotter_Loaded);
        }

        /// <summary>
        /// Remove all graphs which are present on the plotter
        /// </summary>
        public void RemoveAllGraphs() {
            List<IPlotterElement> graphs = new List<IPlotterElement>(from IPlotterElement elem in Children where elem is ILegendable select elem);
            foreach (IPlotterElement graph in graphs) Children.Remove(graph);
        }

        private void OnVerticalAxisTicksChanged(object sender, EventArgs e)
        {
            IAxis axis = sender as IAxis;
            UpdateVerticalTicks(axis);
        }

        private void OnHorizontalAxisTicksChanged(object sender, EventArgs e)
        {
            IAxis axis = sender as IAxis;
            UpdateHorizontalTicks(axis);
        }

        private void ChartPlotter_Loaded(object sender, RoutedEventArgs e)
        {
            if (!alreadyLoaded)
            {
                VerticalAxis = new VerticalAxis();

                #region Different initialization
                if (settings.HorizontalAxisType == ChartPlotterSettings.AxisType.NumericAxis)
                {
                    HorizontalAxis = new HorizontalAxis();
                }
                else HorizontalAxis = new HorizontalDateTimeAxis();

                if (settings.IsButtonNavigationPresents)
                {
                    buttonsNavigation = new buttonsNavigation(this);
                    HoveringStackPanel.Children.Add(buttonsNavigation);
                }

                if(settings.IsLegendPresents)
                {
                    legend = new Legend(this);
                    ScrollWraper legendWraper = new ScrollWraper(legend);
                    legendWraper.Margin = new Thickness(5, 10, 10, 10);
                    HoveringStackPanel.Children.Add(legendWraper);
                }
                #endregion

                Children.Add(axisGrid);
                Children.Add(mouseNavigation);


                
                MainCanvas.SizeChanged += new SizeChangedEventHandler(MainCanvas_SizeChanged);
                HoveringStackPanel.SizeChanged += new SizeChangedEventHandler(hoveringPanel_SizeChanged);
                Viewport.FitToView();
                alreadyLoaded = true;
            }
        }

        private bool isHoveringPanelCliped = false;

        private void hoveringPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StackPanel h = sender as StackPanel;
            if ((!isHoveringPanelCliped)&&( e.NewSize.Height > CentralGrid.ActualHeight - 10))
            {
                h.Height = CentralGrid.ActualHeight - 15;
                isHoveringPanelCliped=true;
                return;
            }

            if ((isHoveringPanelCliped) && (e.NewSize.Height < CentralGrid.ActualHeight - 20))
            {
                h.Height = double.NaN;
                isHoveringPanelCliped = false;
            }
        }

        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateClippings();
            //HoveringStackPanel.Height = 20;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size toMeasure = new Size();
            toMeasure.Width = double.IsInfinity(availableSize.Width) ? (800) : (availableSize.Width);
            toMeasure.Height = double.IsInfinity(availableSize.Height)? (400):(availableSize.Height);
            return base.MeasureOverride(toMeasure);
        }

        private void UpdateClippings() {
            RectangleGeometry bottomClip = new RectangleGeometry();
            RectangleGeometry centralClip = new RectangleGeometry();
            RectangleGeometry headerClip = new RectangleGeometry();
            RectangleGeometry leftPanelClip = new RectangleGeometry();
            RectangleGeometry rightPanelClip = new RectangleGeometry();
            bottomClip.Rect = new Rect(new Point(0, 0), new Size(CentralGrid.ActualWidth, ActualHeight - CentralGrid.ActualHeight - HeaderPanel.ActualHeight));
            centralClip.Rect = new Rect(new Point(0, 0), new Size(CentralGrid.ActualWidth, CentralGrid.ActualHeight));
            headerClip.Rect = new Rect(new Point(0, 0), new Size(CentralGrid.ActualWidth, ActualHeight - CentralGrid.ActualHeight - BottomPanel.ActualHeight));
            leftPanelClip.Rect = new Rect(new Point(0, 0), new Size(ActualWidth - CentralGrid.ActualWidth - RightPanel.ActualWidth, CentralGrid.ActualHeight));
            rightPanelClip.Rect = new Rect(new Point(0, 0), new Size(ActualWidth - CentralGrid.ActualWidth - LeftPanel.ActualWidth, CentralGrid.ActualHeight));
            axisGrid.Clip = centralClip;
            BottomPanel.Clip = bottomClip;
            HeaderPanel.Clip = headerClip;
            LeftPanel.Clip = leftPanelClip;
            RightPanel.Clip = rightPanelClip;
            }

        private void UpdateHorizontalTicks(IAxis axis)
        {   
            if (axis != null)
            {
                axisGrid.HorizontalTicks = axis.ScreenTicks;
             }
            else
            {
                axisGrid.HorizontalTicks = null;
            }
        }

        private void UpdateVerticalTicks(IAxis axis)
        {
            if (axis != null)
            {
                axisGrid.VerticalTicks = axis.ScreenTicks;
            }
            else
            {
                axisGrid.VerticalTicks = null;
            }
        }

        private enum AxisType
        {
            Horizontal,
            Vertical
        }
    }

    public class ChartPlotterSettings
    {
        public enum AxisType { NumericAxis, DateTimeAxis };

        public AxisType HorizontalAxisType { get; set; }

        public bool IsLegendPresents { get; set; }

        public bool IsButtonNavigationPresents { get; set; }

        public ChartPlotterSettings() {
            HorizontalAxisType = AxisType.NumericAxis;
            IsLegendPresents = true;
            IsButtonNavigationPresents = true;
        }
    }
}
