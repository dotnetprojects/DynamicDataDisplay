using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Markers
{
	public class OldBarChart : MarkerChart
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BarChart"/> class.
		/// </summary>
		public OldBarChart() { }

		protected override void OnDataSourceChanged()
		{
		}

		BarFromValueConverter converter = new BarFromValueConverter();
		List<double> xValues = new List<double>();
		protected void OnMarkerBind(BindMarkerEventArgs e)
		{
			var marker = e.Marker;

			//marker.SetBinding(ViewportRectPanel.ViewportVerticalAlignmentProperty, new Binding { Path = new PropertyPath("Value"), Converter = converter });

			xValues.Add(ViewportPanel.GetX(marker));
		}

		protected void RebuildMarkers(bool shouldReleaseMarkers)
		{
			xValues.Clear();

			base.RebuildMarkers(shouldReleaseMarkers);

			double width = 0;
			for (int i = 0; i < xValues.Count - 1; i++)
			{
				double currX = xValues[i];
				double nextX = xValues[i + 1];
				width = (nextX - currX);

				ViewportPanel.SetViewportWidth(ItemsPanel.Children[i], width);
			}

			if (ItemsPanel.Children.Count > 0)
			{
				ViewportPanel.SetViewportWidth(ItemsPanel.Children[ItemsPanel.Children.Count - 1], width);
			}
		}
	}
}
