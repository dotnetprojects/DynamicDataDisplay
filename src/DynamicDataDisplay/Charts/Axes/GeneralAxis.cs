using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
	/// <summary>
	/// Represents a base class for all DynamicDataDisplay's axes.
	/// Has several axis-specific and all WPF-specific properties.
	/// </summary>
	public abstract class GeneralAxis : ContentControl, IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralAxis"/> class.
		/// </summary>
		protected GeneralAxis() { }

		#region Placement property

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

		protected virtual void OnPlacementChanged(AxisPlacement oldPlacement, AxisPlacement newPlacement) { }

		protected Panel GetPanelByPlacement(AxisPlacement placement)
		{
			Panel panel = null;
			switch (placement)
			{
				case AxisPlacement.Left:
					panel = ParentPlotter.LeftPanel;
					break;
				case AxisPlacement.Right:
					panel = ParentPlotter.RightPanel;
					break;
				case AxisPlacement.Top:
					panel = ParentPlotter.TopPanel;
					break;
				case AxisPlacement.Bottom:
					panel = ParentPlotter.BottomPanel;
					break;
				default:
					break;
			}
			return panel;
		}

		/// <summary>
		/// Validates the placement - e.g., vertical axis should not be placed from top or bottom, etc.
		/// If proposed placement is wrong, throws an ArgumentException.
		/// </summary>
		/// <param name="newPlacement">The new placement.</param>
		protected virtual void ValidatePlacement(AxisPlacement newPlacement) { }

		#endregion

		protected void RaiseTicksChanged()
		{
			TicksChanged.Raise(this);
		}

		public abstract void ForceUpdate();

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
		public abstract double[] ScreenTicks { get; }

		/// <summary>
		/// Gets the screen coordinates of minor ticks.
		/// </summary>
		/// <value>The minor screen ticks.</value>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract MinorTickInfo<double>[] MinorScreenTicks { get; }

		#region IPlotterElement Members

		private Plotter2D plotter;
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Plotter2D ParentPlotter
		{
			get { return plotter; }
		}

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			OnPlotterAttached(this.plotter);
		}

		protected virtual void OnPlotterAttached(Plotter2D plotter) { }

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			OnPlotterDetaching(this.plotter);
			this.plotter = null;
		}

		protected virtual void OnPlotterDetaching(Plotter2D plotter) { }

		public Plotter2D Plotter
		{
			get { return plotter; }
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
