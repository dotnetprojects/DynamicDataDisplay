using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Represents a title of vertical axis. Can be placed from left or right of Plotter.
	/// </summary>
	public class VerticalAxisTitle : ContentControl, IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VerticalAxisTitle"/> class.
		/// </summary>
		public VerticalAxisTitle()
		{
			ChangeLayoutTransform();
			VerticalAlignment = VerticalAlignment.Center;
			FontSize = 16;
		}

		private void ChangeLayoutTransform()
		{
			if (placement == AxisPlacement.Left)
				LayoutTransform = new RotateTransform(-90);
			else
				LayoutTransform = new RotateTransform(90);
		}

		private Plotter plotter;
		public Plotter Plotter
		{
			get { return plotter; }
		}

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;

			var hostPanel = GetHostPanel(plotter);
			var index = GetInsertPosition(hostPanel);

			hostPanel.Children.Insert(index, this);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter = null;

			var hostPanel = GetHostPanel(plotter);

			hostPanel.Children.Remove(this);
		}

		private Panel GetHostPanel(Plotter plotter)
		{
			if (placement == AxisPlacement.Left)
				return plotter.LeftPanel;
			else
				return plotter.RightPanel;
		}

		private int GetInsertPosition(Panel panel)
		{
			if (placement == AxisPlacement.Left)
				return 0;
			else
				return panel.Children.Count;
		}

		private AxisPlacement placement = AxisPlacement.Left;
		/// <summary>
		/// Gets or sets the placement of axis title.
		/// </summary>
		/// <value>The placement.</value>
		public AxisPlacement Placement
		{
			get { return placement; }
			set
			{
				if (value.IsBottomOrTop())
					throw new ArgumentException(String.Format("VerticalAxisTitle only supports Left and Right values of AxisPlacement, you passed '{0}'", value), "Placement");

				if (placement != value)
				{
					if (plotter != null)
					{
						var oldPanel = GetHostPanel(plotter);
						oldPanel.Children.Remove(this);
					}

					placement = value;

					ChangeLayoutTransform();

					if (plotter != null)
					{
						var hostPanel = GetHostPanel(plotter);
						var index = GetInsertPosition(hostPanel);
						hostPanel.Children.Insert(index, this);
					}
				}
			}
		}
	}
}