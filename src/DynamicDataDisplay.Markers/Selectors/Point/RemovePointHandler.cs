using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using Microsoft.Research.DynamicDataDisplay.Common.UndoSystem;
using System.Reflection;
using Microsoft.Research.DynamicDataDisplay.Charts.Markers;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	public class RemovePointHandler : PointSelectorModeHandler
	{
		private PointSelector selector;
		private Plotter2D plotter;

		protected override void AttachCore(PointSelector selector, Plotter plotter)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (plotter == null)
				throw new ArgumentNullException("plotter");

			this.selector = selector;
			this.plotter = (Plotter2D)plotter;

			foreach (FrameworkElement element in selector.MarkerChart.Items)
			{
				element.Cursor = Cursors.No;
				element.MouseLeftButtonUp += element_MouseLeftButtonUp;
			}
		}

		protected override void DetachCore()
		{
			foreach (FrameworkElement element in selector.MarkerChart.Items)
			{
				element.ClearValue(FrameworkElement.CursorProperty);
				element.MouseLeftButtonUp -= element_MouseLeftButtonUp;
			}
		}

		private void element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			FrameworkElement clickedElement = (FrameworkElement)sender;
			int index = DevMarkerChart.GetIndex(clickedElement);
			if (index >= 0)
			{
				Point point = selector.Points[index];
				selector.Points.RemoveAt(index);
				//plotter.UndoProvider.AddAction(new CollectionRemoveAction<Point>(selector.Points, point, index));
				e.Handled = true;
			}
		}
	}
}
