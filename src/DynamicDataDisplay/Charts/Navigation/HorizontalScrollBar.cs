using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Navigation
{
	/// <summary>
	/// Represents a horizontal scroll bar on the borrom of <see cref="Plotter"/>.
	/// Uses ChartPlotter.Plotter.Viewport.DataDomain property as a source of data about current position and position limits.
	/// </summary>
	public sealed class HorizontalScrollBar : PlotterScrollBar
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalScrollBar"/> class.
		/// </summary>
		public HorizontalScrollBar()
		{
			ScrollBar.Orientation = Orientation.Horizontal;
		}

		protected override void UpdateScrollBar(Viewport2D viewport)
		{
			if (viewport != null && !viewport.Domain.IsEmpty)
			{
				var visibleRange = new Range<double>(viewport.Visible.XMin, viewport.Visible.XMax);

				double size = visibleRange.Max - visibleRange.Min;
				ScrollBar.ViewportSize = size;

				var domainRange = new Range<double>(viewport.Domain.XMin, viewport.Domain.XMax);
				ScrollBar.Minimum = domainRange.Min;
				ScrollBar.Maximum = domainRange.Max - size;

				ScrollBar.Value = visibleRange.Min;

				ScrollBar.Visibility = Visibility.Visible;
			}
			else
			{
				ScrollBar.Visibility = Visibility.Collapsed;
			}
		}

		protected override DataRect CreateVisibleRect(DataRect rect, double value)
		{
			rect.XMin = value;
			return rect;
		}

		protected override Panel GetHostPanel(Plotter plotter)
		{
			return plotter.BottomPanel;
		}
	}
}
