using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents a base class for all axes in ChartPlotter.
	/// Contains a real UI representation of axis - AxisControl, and means to adjust number of ticks, algorythms of their generating and 
	/// look of ticks' labels.
	/// </summary>
	/// <typeparam name="T">Type of each tick's value</typeparam>
	public abstract class AxisBase<T> : GeneralAxis, ITypedAxis<T>, IValueConversion<T>
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
			axisControl.MakeDependent();
			axisControl.ConvertToDouble = convertToDouble;
			axisControl.ScreenTicksChanged += axisControl_ScreenTicksChanged;

			Content = axisControl;
			axisControl.SetBinding(Control.BackgroundProperty, new Binding("Background") { Source = this });

			Focusable = false;

			Loaded += OnLoaded;
		}

		public override void ForceUpdate()
		{
			axisControl.UpdateUI();
		}

		private void axisControl_ScreenTicksChanged(object sender, EventArgs e)
		{
			RaiseTicksChanged();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this axis is default axis.
		/// ChartPlotter's AxisGrid gets axis ticks to display from two default axes - horizontal and vertical.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is default axis; otherwise, <c>false</c>.
		/// </value>
		public bool IsDefaultAxis
		{
			get { return Microsoft.Research.DynamicDataDisplay.Plotter.GetIsDefaultAxis(this); }
			set { Microsoft.Research.DynamicDataDisplay.Plotter.SetIsDefaultAxis(this, value); }
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			RaiseTicksChanged();
		}

		/// <summary>
		/// Gets the screen coordinates of axis ticks.
		/// </summary>
		/// <value>The screen ticks.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override double[] ScreenTicks
		{
			get { return axisControl.ScreenTicks; }
		}

		/// <summary>
		/// Gets the screen coordinates of minor ticks.
		/// </summary>
		/// <value>The minor screen ticks.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override MinorTickInfo<double>[] MinorScreenTicks
		{
			get { return axisControl.MinorScreenTicks; }
		}

		private AxisControl<T> axisControl;
		/// <summary>
		/// Gets the axis control - actual UI representation of axis.
		/// </summary>
		/// <value>The axis control.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AxisControl<T> AxisControl
		{
			get { return axisControl; }
		}

		/// <summary>
		/// Gets or sets the ticks provider, which is used to generate ticks in given range.
		/// </summary>
		/// <value>The ticks provider.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ITicksProvider<T> TicksProvider
		{
			get { return axisControl.TicksProvider; }
			set { axisControl.TicksProvider = value; }
		}

		/// <summary>
		/// Gets or sets the label provider, that is used to create UI look of axis ticks.
		/// 
		/// Should not be null.
		/// </summary>
		/// <value>The label provider.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[NotNull]
		public LabelProviderBase<T> LabelProvider
		{
			get { return axisControl.LabelProvider; }
			set { axisControl.LabelProvider = value; }
		}

		/// <summary>
		/// Gets or sets the major label provider, which creates labels for major ticks.
		/// If null, major labels will not be shown.
		/// </summary>
		/// <value>The major label provider.</value>
		public LabelProviderBase<T> MajorLabelProvider
		{
			get { return axisControl.MajorLabelProvider; }
			set { axisControl.MajorLabelProvider = value; }
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

		/// <summary>
		/// Gets or sets a value indicating whether to show minor ticks.
		/// </summary>
		/// <value><c>true</c> if show minor ticks; otherwise, <c>false</c>.</value>
		public bool ShowMinorTicks
		{
			get { return axisControl.DrawMinorTicks; }
			set { axisControl.DrawMinorTicks = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to show major labels.
		/// </summary>
		/// <value><c>true</c> if show major labels; otherwise, <c>false</c>.</value>
		public bool ShowMajorLabels
		{
			get { return axisControl.DrawMajorLabels; }
			set { axisControl.DrawMajorLabels = value; }
		}

		protected override void OnPlotterAttached(Plotter2D plotter)
		{
			plotter.Viewport.PropertyChanged += OnViewportPropertyChanged;

			Panel panel = GetPanelByPlacement(Placement);
			if (panel != null)
			{
				int index = GetInsertionIndexByPlacement(Placement, panel);
				panel.Children.Insert(index, this);
			}

			using (axisControl.OpenUpdateRegion(true))
			{
				UpdateAxisControl(plotter);
			}
		}

		private void UpdateAxisControl(Plotter2D plotter2d)
		{
			axisControl.Transform = plotter2d.Viewport.Transform;
			axisControl.Range = CreateRangeFromRect(plotter2d.Visible.ViewportToData(plotter2d.Viewport.Transform));
		}

		private int GetInsertionIndexByPlacement(AxisPlacement placement, Panel panel)
		{
			int index = panel.Children.Count;

			switch (placement)
			{
				case AxisPlacement.Left:
					index = 0;
					break;
				case AxisPlacement.Top:
					index = 0;
					break;
				default:
					break;
			}

			return index;
		}

		ExtendedPropertyChangedEventArgs visibleChangedEventArgs;
		int viewportPropertyChangedEnters = 0;
		DataRect prevDataRect = DataRect.Empty;
		private void OnViewportPropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (viewportPropertyChangedEnters > 4)
			{
				if (e.PropertyName == "Visible")
				{
					visibleChangedEventArgs = e;
				}
				return;
			}

			viewportPropertyChangedEnters++;

			Viewport2D viewport = (Viewport2D)sender;

			DataRect visible = viewport.Visible;

			DataRect dataRect = visible.ViewportToData(viewport.Transform);
			bool forceUpdate = dataRect != prevDataRect;
			prevDataRect = dataRect;

			Range<T> range = CreateRangeFromRect(dataRect);

			using (axisControl.OpenUpdateRegion(false))	// todo was forceUpdate
			{
				axisControl.Range = range;
				axisControl.Transform = viewport.Transform;
			}

			Dispatcher.BeginInvoke(() =>
			{
				viewportPropertyChangedEnters--;
				if (visibleChangedEventArgs != null)
				{
					OnViewportPropertyChanged(Plotter.Viewport, visibleChangedEventArgs);
				}
				visibleChangedEventArgs = null;
			}, DispatcherPriority.Render);
		}

		private Func<double, T> convertFromDouble;
		/// <summary>
		/// Gets or sets the delegate that is used to create each tick from double.
		/// Is used to create typed range to display for internal AxisControl.
		/// If changed, ConvertToDouble should be changed appropriately, too.
		/// Should not be null.
		/// </summary>
		/// <value>The convert from double.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[NotNull]
		public Func<double, T> ConvertFromDouble
		{
			get { return convertFromDouble; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (convertFromDouble != value)
				{
					convertFromDouble = value;
					if (ParentPlotter != null)
					{
						UpdateAxisControl(ParentPlotter);
					}
				}
			}
		}

		private Func<T, double> convertToDouble;
		/// <summary>
		/// Gets or sets the delegate that is used to convert each tick to double.
		/// Is used by internal AxisControl to convert tick to double to get tick's coordinates inside of viewport.
		/// If changed, ConvertFromDouble should be changed appropriately, too.
		/// Should not be null.
		/// </summary>
		/// <value>The convert to double.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[NotNull]
		public Func<T, double> ConvertToDouble
		{
			get { return convertToDouble; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (convertToDouble != value)
				{
					convertToDouble = value;
					axisControl.ConvertToDouble = value;
				}
			}
		}

		/// <summary>
		/// Sets conversions of axis - functions used to convert values of axis type to and from double values of viewport.
		/// Sets both ConvertToDouble and ConvertFromDouble properties.
		/// </summary>
		/// <param name="min">The minimal viewport value.</param>
		/// <param name="minValue">The value of axis type, corresponding to minimal viewport value.</param>
		/// <param name="max">The maximal viewport value.</param>
		/// <param name="maxValue">The value of axis type, corresponding to maximal viewport value.</param>
		public virtual void SetConversion(double min, T minValue, double max, T maxValue)
		{
			throw new NotImplementedException();
		}

		private Range<T> CreateRangeFromRect(DataRect visible)
		{
			T min, max;

			Range<T> range;
			switch (Placement)
			{
				case AxisPlacement.Left:
				case AxisPlacement.Right:
					min = ConvertFromDouble(visible.YMin);
					max = ConvertFromDouble(visible.YMax);
					break;
				case AxisPlacement.Top:
				case AxisPlacement.Bottom:
					min = ConvertFromDouble(visible.XMin);
					max = ConvertFromDouble(visible.XMax);
					break;
				default:
					throw new NotSupportedException();
			}

			TrySort(ref min, ref max);
			range = new Range<T>(min, max);
			return range;
		}

		private static void TrySort<TS>(ref TS min, ref TS max)
		{
			if (min is IComparable)
			{
				IComparable c1 = (IComparable)min;
				// if min > max
				if (c1.CompareTo(max) > 0)
				{
					TS temp = min;
					min = max;
					max = temp;
				}
			}
		}

		protected override void OnPlacementChanged(AxisPlacement oldPlacement, AxisPlacement newPlacement)
		{
			axisControl.Placement = Placement;
			if (ParentPlotter != null)
			{
				Panel panel = GetPanelByPlacement(oldPlacement);
				panel.Children.Remove(this);

				Panel newPanel = GetPanelByPlacement(newPlacement);
				int index = GetInsertionIndexByPlacement(newPlacement, newPanel);
				newPanel.Children.Insert(index, this);
			}
		}

		protected override void OnPlotterDetaching(Plotter2D plotter)
		{
			if (plotter == null)
				return;

			Panel panel = GetPanelByPlacement(Placement);
			if (panel != null)
			{
				panel.Children.Remove(this);
			}

			plotter.Viewport.PropertyChanged -= OnViewportPropertyChanged;
			axisControl.Transform = null;
		}
	}
}
