using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.Navigation;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>Chart plotter is a plotter that renders axis and grid</summary>
	public class ChartPlotter : Plotter2D
	{
		private GeneralAxis horizontalAxis = new HorizontalAxis();
		private GeneralAxis verticalAxis = new VerticalAxis();
		private AxisGrid axisGrid = new AxisGrid();

		private readonly Legend legend = new Legend();

		private NewLegend newLegend = new NewLegend();
		public NewLegend NewLegend
		{
			get { return newLegend; }
			set { newLegend = value; }
		}

		public ItemsPanelTemplate LegendPanelTemplate
		{
			get { return newLegend.ItemsPanel; }
			set { newLegend.ItemsPanel = value; }
		}

		public Style LegendStyle
		{
			get { return newLegend.Style; }
			set { newLegend.Style = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChartPlotter"/> class.
		/// </summary>
		public ChartPlotter()
			: base()
		{
			horizontalAxis.TicksChanged += OnHorizontalAxisTicksChanged;
			verticalAxis.TicksChanged += OnVerticalAxisTicksChanged;

			SetIsDefaultAxis(horizontalAxis as DependencyObject, true);
			SetIsDefaultAxis(verticalAxis as DependencyObject, true);

			mouseNavigation = new MouseNavigation();
			keyboardNavigation = new KeyboardNavigation();
			defaultContextMenu = new DefaultContextMenu();
			horizontalAxisNavigation = new AxisNavigation { Placement = AxisPlacement.Bottom };
			verticalAxisNavigation = new AxisNavigation { Placement = AxisPlacement.Left };

			Children.AddMany(
				horizontalAxis,
				verticalAxis,
				axisGrid,
				mouseNavigation,
				keyboardNavigation,
				defaultContextMenu,
				horizontalAxisNavigation,
				legend,
				verticalAxisNavigation,
				new LongOperationsIndicator(),
				newLegend
				);

#if DEBUG
			Children.Add(new DebugMenu());
#endif

			SetAllChildrenAsDefault();
		}

		/// <summary>
		/// Creates generic plotter from this ChartPlotter.
		/// </summary>
		/// <returns></returns>
		public GenericChartPlotter<double, double> GetGenericPlotter()
		{
			return new GenericChartPlotter<double, double>(this);
		}

		/// <summary>
		/// Creates generic plotter from this ChartPlotter.
		/// Horizontal and Vertical types of GenericPlotter should correspond to ChartPlotter's actual main axes types.
		/// </summary>
		/// <typeparam name="THorizontal">The type of horizontal values.</typeparam>
		/// <typeparam name="TVertical">The type of vertical values.</typeparam>
		/// <returns>GenericChartPlotter, associated to this ChartPlotter.</returns>
		public GenericChartPlotter<THorizontal, TVertical> GetGenericPlotter<THorizontal, TVertical>()
		{
			return new GenericChartPlotter<THorizontal, TVertical>(this);
		}

		/// <summary>
		/// Creates generic plotter from this ChartPlotter.
		/// </summary>
		/// <typeparam name="THorizontal">The type of the horizontal axis.</typeparam>
		/// <typeparam name="TVertical">The type of the vertical axis.</typeparam>
		/// <param name="horizontalAxis">The horizontal axis to use as data conversion source.</param>
		/// <param name="verticalAxis">The vertical axis to use as data conversion source.</param>
		/// <returns>GenericChartPlotter, associated to this ChartPlotter</returns>
		public GenericChartPlotter<THorizontal, TVertical> GetGenericPlotter<THorizontal, TVertical>(AxisBase<THorizontal> horizontalAxis, AxisBase<TVertical> verticalAxis)
		{
			return new GenericChartPlotter<THorizontal, TVertical>(this, horizontalAxis, verticalAxis);
		}

		protected ChartPlotter(PlotterLoadMode loadMode) : base(loadMode) { }

		/// <summary>
		/// Creates empty plotter without any axes, navigation, etc.
		/// </summary>
		/// <returns>Empty plotter without any axes, navigation, etc.</returns>
		public static ChartPlotter CreateEmpty()
		{
			return new ChartPlotter(PlotterLoadMode.OnlyViewport);
		}

		public void BeginLongOperation()
		{
			LongOperationsIndicator.BeginLongOperation(this);
		}

		public void EndLongOperation()
		{
			LongOperationsIndicator.EndLongOperation(this);
		}

		#region Default charts

		private MouseNavigation mouseNavigation;
		/// <summary>
		/// Gets the default mouse navigation of ChartPlotter.
		/// </summary>
		/// <value>The mouse navigation.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MouseNavigation MouseNavigation
		{
			get { return mouseNavigation; }
		}

		private KeyboardNavigation keyboardNavigation;
		/// <summary>
		/// Gets the default keyboard navigation of ChartPlotter.
		/// </summary>
		/// <value>The keyboard navigation.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public KeyboardNavigation KeyboardNavigation
		{
			get { return keyboardNavigation; }
		}

		private DefaultContextMenu defaultContextMenu;
		/// <summary>
		/// Gets the default context menu of ChartPlotter.
		/// </summary>
		/// <value>The default context menu.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DefaultContextMenu DefaultContextMenu
		{
			get { return defaultContextMenu; }
		}

		private AxisNavigation horizontalAxisNavigation;
		/// <summary>
		/// Gets the default horizontal axis navigation of ChartPlotter.
		/// </summary>
		/// <value>The horizontal axis navigation.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AxisNavigation HorizontalAxisNavigation
		{
			get { return horizontalAxisNavigation; }
		}

		private AxisNavigation verticalAxisNavigation;
		/// <summary>
		/// Gets the default vertical axis navigation of ChartPlotter.
		/// </summary>
		/// <value>The vertical axis navigation.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AxisNavigation VerticalAxisNavigation
		{
			get { return verticalAxisNavigation; }
		}

		/// <summary>
		/// Gets the default axis grid of ChartPlotter.
		/// </summary>
		/// <value>The axis grid.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AxisGrid AxisGrid
		{
			get { return axisGrid; }
		}

		#endregion

		private void OnHorizontalAxisTicksChanged(object sender, EventArgs e)
		{
			GeneralAxis axis = (GeneralAxis)sender;
			UpdateHorizontalTicks(axis);
		}

		private void UpdateHorizontalTicks(GeneralAxis axis)
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

		private void OnVerticalAxisTicksChanged(object sender, EventArgs e)
		{
			GeneralAxis axis = (GeneralAxis)sender;
			UpdateVerticalTicks(axis);
		}

		private void UpdateVerticalTicks(GeneralAxis axis)
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
				axisGrid.MinorVerticalTicks = null;
			}

			axisGrid.EndTicksUpdate();
		}

		bool keepOldAxis = false;
		bool updatingAxis = false;

		/// <summary>
		/// Gets or sets the main vertical axis of ChartPlotter.
		/// Main vertical axis of ChartPlotter is axis which ticks are used to draw horizontal lines on AxisGrid.
		/// Value can be set to null to completely remove main vertical axis.
		/// </summary>
		/// <value>The main vertical axis.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GeneralAxis MainVerticalAxis
		{
			get { return verticalAxis; }
			set
			{
				if (updatingAxis)
					return;

				if (value == null && verticalAxis != null)
				{
					if (!keepOldAxis)
					{
						Children.Remove(verticalAxis);
					}
					verticalAxis.TicksChanged -= OnVerticalAxisTicksChanged;
					verticalAxis = null;

					UpdateVerticalTicks(verticalAxis);

					return;
				}

				VerifyAxisType(value.Placement, AxisType.Vertical);

				if (value != verticalAxis)
				{
					ValidateVerticalAxis(value);

					updatingAxis = true;

					if (verticalAxis != null)
					{
						verticalAxis.TicksChanged -= OnVerticalAxisTicksChanged;
						SetIsDefaultAxis(verticalAxis, false);
						if (!keepOldAxis)
						{
							Children.Remove(verticalAxis);
						}
					}
					SetIsDefaultAxis(value, true);

					verticalAxis = value;
					verticalAxis.TicksChanged += OnVerticalAxisTicksChanged;

					if (!Children.Contains(value))
					{
						Children.Add(value);
					}

					UpdateVerticalTicks(value);
					OnVerticalAxisChanged();

					updatingAxis = false;
				}
			}
		}

		protected virtual void OnVerticalAxisChanged() { }
		protected virtual void ValidateVerticalAxis(GeneralAxis axis) { }

		/// <summary>
		/// Gets or sets the main horizontal axis visibility.
		/// </summary>
		/// <value>The main horizontal axis visibility.</value>
		public Visibility MainHorizontalAxisVisibility
		{
			get { return MainHorizontalAxis != null ? ((UIElement)MainHorizontalAxis).Visibility : Visibility.Hidden; }
			set
			{
				if (MainHorizontalAxis != null)
				{
					((UIElement)MainHorizontalAxis).Visibility = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the main vertical axis visibility.
		/// </summary>
		/// <value>The main vertical axis visibility.</value>
		public Visibility MainVerticalAxisVisibility
		{
			get { return MainVerticalAxis != null ? ((UIElement)MainVerticalAxis).Visibility : Visibility.Hidden; }
			set
			{
				if (MainVerticalAxis != null)
				{
					((UIElement)MainVerticalAxis).Visibility = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the main horizontal axis of ChartPlotter.
		/// Main horizontal axis of ChartPlotter is axis which ticks are used to draw vertical lines on AxisGrid.
		/// Value can be set to null to completely remove main horizontal axis.
		/// </summary>
		/// <value>The main horizontal axis.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GeneralAxis MainHorizontalAxis
		{
			get { return horizontalAxis; }
			set
			{
				if (updatingAxis)
					return;

				if (value == null && horizontalAxis != null)
				{
					Children.Remove(horizontalAxis);
					horizontalAxis.TicksChanged -= OnHorizontalAxisTicksChanged;
					horizontalAxis = null;

					UpdateHorizontalTicks(horizontalAxis);

					return;
				}

				VerifyAxisType(value.Placement, AxisType.Horizontal);

				if (value != horizontalAxis)
				{
					ValidateHorizontalAxis(value);

					updatingAxis = true;

					if (horizontalAxis != null)
					{
						horizontalAxis.TicksChanged -= OnHorizontalAxisTicksChanged;
						SetIsDefaultAxis(horizontalAxis, false);
						if (!keepOldAxis)
						{
							Children.Remove(horizontalAxis);
						}
					}
					SetIsDefaultAxis(value, true);

					horizontalAxis = value;
					horizontalAxis.TicksChanged += OnHorizontalAxisTicksChanged;

					if (!Children.Contains(value))
					{
						Children.Add(value);
					}

					UpdateHorizontalTicks(value);
					OnHorizontalAxisChanged();

					updatingAxis = false;
				}
			}
		}

		protected virtual void OnHorizontalAxisChanged() { }
		protected virtual void ValidateHorizontalAxis(GeneralAxis axis) { }

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
				throw new ArgumentException(Strings.Exceptions.InvalidAxisPlacement);
		}

		protected override void OnIsDefaultAxisChangedCore(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GeneralAxis axis = d as GeneralAxis;
			if (axis != null)
			{
				bool value = (bool)e.NewValue;
				bool oldKeepOldAxis = keepOldAxis;

				bool horizontal = axis.Placement == AxisPlacement.Bottom || axis.Placement == AxisPlacement.Top;
				keepOldAxis = true;

				if (value && horizontal)
				{
					MainHorizontalAxis = axis;
				}
				else if (value && !horizontal)
				{
					MainVerticalAxis = axis;
				}
				else if (!value && horizontal)
				{
					MainHorizontalAxis = null;
				}
				else if (!value && !horizontal)
				{
					MainVerticalAxis = null;
				}

				keepOldAxis = oldKeepOldAxis;
			}
		}

		/// <summary>
		/// Gets the default legend of ChartPlotter.
		/// </summary>
		/// <value>The legend.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Legend Legend
		{
			get { return legend; }
		}

		/// <summary>
		/// Gets or sets the visibility of legend.
		/// </summary>
		/// <value>The legend visibility.</value>
		public Visibility LegendVisibility
		{
			get { return legend.Visibility; }
			set { legend.Visibility = value; }
		}

		public bool NewLegendVisible
		{
			get { return newLegend.LegendVisible; }
			set { newLegend.LegendVisible = value; }
		}

		private enum AxisType
		{
			Horizontal,
			Vertical
		}
	}
}