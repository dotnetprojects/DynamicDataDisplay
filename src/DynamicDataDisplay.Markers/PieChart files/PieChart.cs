using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Markers;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using d3 = Microsoft.Research.DynamicDataDisplay;
using System.Collections.Specialized;
using Microsoft.Research.DynamicDataDisplay.Charts;
using DynamicDataDisplay.Markers.DataSources;

namespace DynamicDataDisplay.Markers
{
	public class PieChart : DevMarkerChart
	{
		static PieChart()
		{
			Type thisType = typeof(PieChart);
			DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
		}

		private static void OnPlotterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PieChart chart = (PieChart)d;
			ChartPlotter currPlotter = e.NewValue as ChartPlotter;
			if (currPlotter != null)
			{
				chart.OnPlotterAttached(currPlotter);
			}
			else
			{
				ChartPlotter prevPlotter = e.OldValue as ChartPlotter;
				chart.OnPlotterDetaching(prevPlotter);
			}
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
		}

		protected override void OnPlotterDetaching(Plotter2D plotter)
		{
		}

		public PieChart()
		{
			MarkerBuilder = new TemplateMarkerGenerator();
			PropertyMappings.Clear();
		}

		protected override void AddCommonBindings(FrameworkElement marker)
		{
			base.AddCommonBindings(marker);

			marker.SetBinding(PieChartItem.AngleProperty, IndependentValueBinding);
		}

		#region Properties

		public double StartAngle
		{
			get { return (double)GetValue(StartAngleProperty); }
			set { SetValue(StartAngleProperty, value); }
		}

		public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
		  "StartAngle",
		  typeof(double),
		  typeof(PieChart),
		  new FrameworkPropertyMetadata(0.0));

		#endregion // end of Properties

		#region API

		public void AddPieItem(string caption, double value)
		{
			if (String.IsNullOrEmpty(caption))
				throw new ArgumentNullException("caption");

			Items.Add(new PieChartItem { Caption = caption, Angle = value });
		}

		public void AddPieItem(string caption, double value, Brush fill)
		{
			if (String.IsNullOrEmpty(caption))
				throw new ArgumentNullException("caption");

			Items.Add(new PieChartItem { Caption = caption, Angle = value, Background = fill });
		}

		#endregion // end of API
	}
}
