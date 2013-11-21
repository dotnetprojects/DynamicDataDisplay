using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Markers;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Media;

namespace DynamicDataDisplay.Markers
{
	public class BarChart : DevMarkerChart
	{
		static BarChart()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarChart), new FrameworkPropertyMetadata(typeof(BarChart)));
		}

		public BarChart()
		{
			PropertyMappings[DependentValuePathProperty] = ViewportPanel.ViewportWidthProperty;
			MarkerBuilder = new TemplateMarkerGenerator();
		}
	}
}
