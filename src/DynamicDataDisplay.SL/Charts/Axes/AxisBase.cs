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
using System.Windows.Data;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    /// <summary>
    /// Represents a base class for all axes in ChartPlotter.
    /// Contains a real UI representation of axis - AxisControl, and means to adjust number of ticks, algorythms of their generating and 
    /// look of ticks' labels.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AxisBase<T> : ContentControl, ITypedAxis<T>, IValueConversion<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AxisBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="axisControl">The axis control.</param>
        /// <param name="convertFromDouble">The convert from double.</param>
        /// <param name="convertToDouble">The convert to double.</param>
        protected AxisBase(AxisControl<T> axisControl, Func<double, T> convertFromDouble, Func<T, double> convertToDouble)
        {
            if (axisControl == null)
                throw new ArgumentNullException("axisControl");
            if (convertFromDouble == null)
                throw new ArgumentNullException("convertFromDouble");
            if (convertToDouble == null)
                throw new ArgumentNullException("convertToDouble");

            this.convertToDouble = convertToDouble;
            this.convertFromDouble = convertFromDouble;

            this.axisControl = axisControl;
            axisControl.ConvertToDouble = convertToDouble;

            Content = axisControl;
            Binding binding = new Binding("Background");
            binding.Source = this;
            axisControl.SetBinding(Control.BackgroundProperty, binding);
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            RaiseTicksChanged();
        }

        private AxisPlacement placement = AxisPlacement.Bottom;
        /// <summary>
        /// Gets or sets the placement of axis - place in ChartPlotter where it should be placed.
        /// </summary>
        /// <value>The placement.</value>
        public AxisPlacement Placement
        {
            get { return placement; }
            set
            {
                if (placement != value)
                {
                    ValidatePlacement(value);
                    AxisPlacement oldPlacement = placement;
                    placement = value;
                    OnPlacementChanged(oldPlacement, placement);
                }
            }
        }

        protected virtual void ValidatePlacement(AxisPlacement newPlacement) { }

        private void OnPlacementChanged(AxisPlacement oldPlacement, AxisPlacement newPlacement)
        {
            axisControl.Placement = placement;
            if (plotter != null)
            {
                Panel panel = GetPanelByPlacement(oldPlacement);
                panel.Children.Remove(this);

                Panel newPanel = GetPanelByPlacement(placement);
                newPanel.Children.Add(this);
            }
        }

        private Panel GetPanelByPlacement(AxisPlacement oldPlacement)
        {
            Panel panel = null;
            switch (oldPlacement)
            {
                case AxisPlacement.Left:
                    panel = Plotter.LeftPanel;
                    break;
                case AxisPlacement.Right:
                    panel = Plotter.RightPanel;
                    break;
                case AxisPlacement.Top:
                    panel = Plotter.TopPanel;
                    break;
                case AxisPlacement.Bottom:
                    panel = Plotter.BottomPanel;
                    break;
                default:
                    break;
            }
            return panel;
        }

        private void RaiseTicksChanged()
        {
            if (TicksChanged != null)
            {
                TicksChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Occurs when ticks changes.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler TicksChanged;

        /// <summary>
        /// Gets the screen coordinates of axis ticks.
        /// </summary>
        /// <value>The screen ticks.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double[] ScreenTicks
        {
            get { return axisControl.ScreenTicks; }
        }

        /// <summary>
        /// Gets the screen coordinates of minor ticks.
        /// </summary>
        /// <value>The minor screen ticks.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MinorTickInfo<double>[] MinorScreenTicks
        {
            get { return axisControl.MinorScreenTicks; }
        }

        private AxisControl<T> axisControl;
        /// <summary>
        /// Gets the axis control - actual UI representation of axis.
        /// </summary>
        /// <value>The axis control.</value>
        public AxisControl<T> AxisControl
        {
            get { return axisControl; }
        }

        /// <summary>
        /// Gets the ticks provider, which is used to generate ticks in given range.
        /// </summary>
        /// <value>The ticks provider.</value>
        public ITicksProvider<T> TicksProvider
        {
            get { return axisControl.TicksProvider; }
        }

        /// <summary>
        /// Gets or sets the label provider, used to create UI look of axis ticks.
        /// 
        /// Should not be null.
        /// </summary>
        /// <value>The label provider.</value>
        public LabelProviderBase<T> LabelProvider
        {
            get { return axisControl.LabelProvider; }
            set { axisControl.LabelProvider = value; }
        }

        /// <summary>
        /// Gets or sets the label string format, used to create simple formats of each tick's label, such as
        /// changing tick label from "1.2" to "$1.2".
        /// Should be in format "*{0}*", where '*' is any number of any chars.
        /// 
        /// If value is null, format string will not be used.
        /// </summary>
        /// <value>The label string format.</value>
        public string LabelStringFormat
        {
            get { return LabelProvider.LabelStringFormat; }
            set { LabelProvider.LabelStringFormat = value; }
        }

        private Plotter plotter = null;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Plotter Plotter
        {
            get { return plotter; }
        }

        void IPlotterElement.OnPlotterAttached(Plotter plotter)
        {
            if (this.plotter == null)
            {
                this.plotter = plotter;

                Plotter2D plotter2d = plotter as Plotter2D;
                plotter2d.Viewport.VisibleChanged += OnViewportConfigurationChanged;
                plotter2d.Viewport.OutputChanged += OnViewportConfigurationChanged;
                plotter2d.Viewport.TransformChanged += OnViewportConfigurationChanged;

                Panel panel = GetPanelByPlacement(placement);
                //panel.Children.Add( this);
                if (placement == AxisPlacement.Right || placement == AxisPlacement.Left) panel.Children.Add(this);
                else panel.Children.Insert(0, this);

                using (axisControl.OpenUpdateRegion())
                {
                    axisControl.Transform = plotter2d.Viewport.Transform;
                    axisControl.Range = CreateRangeFromRect(plotter2d.Viewport.Visible.ViewportToData(plotter2d.Viewport.Transform));
                }
            }
        }

        private void OnViewportConfigurationChanged(object sender, EventArgs e)
        {
            Viewport2D viewport = (Viewport2D)sender;

            Rect visible = viewport.Visible;


            PerformanceCounter.startStopwatch("Creating new ranges");
                
            Range<T> range = CreateRangeFromRect(visible.ViewportToData(viewport.Transform));
            PerformanceCounter.stopStopwatch("Creating new ranges");
            using (axisControl.OpenUpdateRegion())
            {
                axisControl.Range = range;
                axisControl.Transform = viewport.Transform;
                
            }
            RaiseTicksChanged();
        }

        private Func<double, T> convertFromDouble;
        /// <summary>
        /// Gets or sets the delegate that is used to create each tick from double.
        /// Should not be null.
        /// </summary>
        /// <value>The convert from double.</value>
        public Func<double, T> ConvertFromDouble
        {
            get { return convertFromDouble; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                convertFromDouble = value;
            }
        }

        private Func<T, double> convertToDouble;
        /// <summary>
        /// Gets or sets the delegate that is used to convert each tick to double.
        /// Should not be null.
        /// </summary>
        /// <value>The convert to double.</value>
        public Func<T, double> ConvertToDouble
        {
            get { return convertToDouble; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                convertToDouble = value;
                axisControl.ConvertToDouble = value;
            }
        }

        private Range<T> CreateRangeFromRect(Rect visible)
        {
            T min, max;

            Range<T> range;
            switch (placement)
            {
                case AxisPlacement.Left:
                case AxisPlacement.Right:
                    min = ConvertFromDouble(visible.Top);
                    max = ConvertFromDouble(visible.Bottom);
                    break;
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:
                    min = ConvertFromDouble(visible.Left);
                    max = ConvertFromDouble(visible.Right);
                    break;
                default:
                    throw new NotSupportedException();
            }

            //TrySort(ref min, ref max);
            range = new Range<T>(min, max);
            return range;
        }

        private static void TrySort<TS>(ref TS min, ref TS max)
        {
            if (min is IComparable)
            {
                IComparable c1 = (IComparable)min;

                if (c1.CompareTo(max) > 0)
                {
                    TS temp = min;
                    min = max;
                    max = temp;
                }
            }
        }

        /// <summary>
        /// Called when item is being detached from parent plotter.
        /// Allows to remove added in OnPlotterAttached method UI parts or unsubscribe from events.
        /// This should be done as each chart can be added only one Plotter at one moment of time.
        /// </summary>
        /// <param name="plotter">The plotter.</param>
        void IPlotterElement.OnPlotterDetaching(Plotter plotter)
        {
            if (this.plotter != null)
            {
                Panel panel = GetPanelByPlacement(placement);
                panel.Children.Remove(this);

                Plotter2D plotter2d = plotter as Plotter2D;
                axisControl.Transform = null;

                plotter2d.Viewport.VisibleChanged -= OnViewportConfigurationChanged;
                plotter2d.Viewport.OutputChanged -= OnViewportConfigurationChanged;
                plotter2d.Viewport.TransformChanged -= OnViewportConfigurationChanged;

                this.plotter = null;
            }
        }
    }
}
