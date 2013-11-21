using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Markers;
using System.Windows;

namespace DynamicDataDisplay.Markers
{
	public class AcceptableRangeChart : DevMarkerChart
	{
		#region DataSource

		#region YMinMapping property

		public string YMinMapping
		{
			get { return (string)GetValue(YMinMappingProperty); }
			set { SetValue(YMinMappingProperty, value); }
		}

		public static readonly DependencyProperty YMinMappingProperty = DependencyProperty.Register(
			"YMinMapping",
			typeof(string),
			typeof(AcceptableRangeChart),
			new FrameworkPropertyMetadata(null, OnYMappingChanged));

		private static void OnYMappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AcceptableRangeChart chart = (AcceptableRangeChart)d;
		}

		#endregion // end of YMinMapping

		#endregion

	}
}
