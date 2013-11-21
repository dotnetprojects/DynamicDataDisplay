using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	[Obsolete("Unready")]
	public class MultipleSelectHandler : PointSelectorModeHandler
	{
		private PointSelector selector;
		private Plotter2D plotter;
		private InkCanvas inkCanvas = new InkCanvas
		{
			Background = Brushes.Blue.MakeTransparent(0.1),
			EditingMode = InkCanvasEditingMode.Select,
			ResizeEnabled = false
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="MultipleSelectHandler"/> class.
		/// </summary>
		public MultipleSelectHandler()
		{
			inkCanvas.SelectionChanged += new EventHandler(InkCanvas_SelectionChanged);
			inkCanvas.SelectionChanging += new InkCanvasSelectionChangingEventHandler(InkCanvas_SelectionChanging);
			inkCanvas.StrokeErased += new RoutedEventHandler(inkCanvas_StrokeErased);
		}

		private void inkCanvas_StrokeErased(object sender, RoutedEventArgs e)
		{
		}

		private void InkCanvas_SelectionChanging(object sender, InkCanvasSelectionChangingEventArgs e)
		{
		}

		private void InkCanvas_SelectionChanged(object sender, EventArgs e)
		{
		}

		protected override void AttachCore(PointSelector selector, Plotter plotter)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (plotter == null)
				throw new ArgumentNullException("plotter");

			this.selector = selector;
			this.plotter = (Plotter2D)plotter;
			var transform = this.plotter.Transform;

			// copying all markers from marker chart to InkCanvas to enable their selection
			foreach (FrameworkElement marker in selector.MarkerChart.Items)
			{
				var xamlString = XamlWriter.Save(marker);
				var markerCopy = (FrameworkElement)XamlReader.Parse(xamlString);

				var x = ViewportPanel.GetX(marker);
				var y = ViewportPanel.GetY(marker);
				Point position = new Point(x, y);
				var positionInScreen = position.ViewportToScreen(transform);

				if (!marker.IsMeasureValid)
					marker.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

				InkCanvas.SetLeft(markerCopy, positionInScreen.X - marker.Width / 2);
				InkCanvas.SetTop(markerCopy, positionInScreen.Y - marker.Height / 2);
				markerCopy.DataContext = marker.DataContext;

				inkCanvas.Children.Add(markerCopy);
			}

			plotter.CentralGrid.Children.Add(inkCanvas);
		}

		protected override void DetachCore()
		{
			plotter.CentralGrid.Children.Remove(inkCanvas);
		}
	}
}
