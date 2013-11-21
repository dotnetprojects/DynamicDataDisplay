using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Represents a title of horizontal axis. Can be placed from top or bottom of Plotter.
	/// </summary>
	public class HorizontalAxisTitle : ContentControl, IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalAxisTitle"/> class.
		/// </summary>
		public HorizontalAxisTitle()
		{
			FontSize = 16;
			HorizontalAlignment = HorizontalAlignment.Center;
		}

		private Plotter plotter;
		public Plotter Plotter
		{
			get { return plotter; }
		}

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;
			AddToPlotter();
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			RemoveFromPlotter();
			this.plotter = null;
		}

		private Panel GetHostPanel(Plotter plotter)
		{
			if (placement == AxisPlacement.Bottom)
				return plotter.BottomPanel;
			else
				return plotter.TopPanel;
		}

		private int GetInsertPosition(Panel panel)
		{
			if (placement == AxisPlacement.Bottom)
				return panel.Children.Count;
			else
				return 0;
		}

		private AxisPlacement placement = AxisPlacement.Bottom;
		/// <summary>
		/// Gets or sets the placement of axis title.
		/// </summary>
		/// <value>The placement.</value>
		public AxisPlacement Placement
		{
			get { return placement; }
			set
			{
				if (!value.IsBottomOrTop())
					throw new ArgumentException(String.Format("HorizontalAxisTitle only supports Top and Bottom values of AxisPlacement, you passed '{0}'", value), "Placement");

				if (placement != value)
				{
					if (plotter != null)
					{
						RemoveFromPlotter();
					}

					placement = value;

					if (plotter != null)
					{
						AddToPlotter();
					}
				}
			}
		}

		private void RemoveFromPlotter()
		{
			var oldPanel = GetHostPanel(plotter);
			oldPanel.Children.Remove(this);
		}

		private void AddToPlotter()
		{
			var hostPanel = GetHostPanel(plotter);
			var index = GetInsertPosition(hostPanel);
			hostPanel.Children.Insert(index, this);
		}
	}
}