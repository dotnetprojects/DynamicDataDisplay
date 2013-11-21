using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace DynamicDataDisplay.Markers
{
	public class PieChartItem : Control
	{
		static PieChartItem()
		{
			Type thisType = typeof(PieChartItem);
			DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
			//BackgroundProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(Brushes.Yellow));
		}

		public PieChartItem()
		{
			Background = ColorHelper.RandomBrush;
		}

		#region Properties

		#region Angle property

		public double Angle
		{
			get { return (double)GetValue(AngleProperty); }
			set { SetValue(AngleProperty, value); }
		}

		public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
		  "Angle",
		  typeof(double),
		  typeof(PieChartItem),
		  new FrameworkPropertyMetadata(45.0));

		#endregion

		#region AngleInChart

		internal double AngleInChart
		{
			get { return (double)GetValue(AngleInChartProperty); }
			set { SetValue(AngleInChartProperty, value); }
		}

		internal static readonly DependencyProperty AngleInChartProperty = DependencyProperty.Register(
		  "AngleInChart",
		  typeof(double),
		  typeof(PieChartItem),
		  new FrameworkPropertyMetadata(0.0));

		#endregion // end of AngleInChart

		#region StrokeThickness

		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
		  "StrokeThickness",
		  typeof(double),
		  typeof(PieChartItem),
		  new FrameworkPropertyMetadata(1.0));

		#endregion // end of StrokeThickness

		#region RadiusStartRatio

		public double RadiusStartRatio
		{
			get { return (double)GetValue(RadiusStartRatioProperty); }
			set { SetValue(RadiusStartRatioProperty, value); }
		}

		public static readonly DependencyProperty RadiusStartRatioProperty = DependencyProperty.Register(
		  "RadiusStartRatio",
		  typeof(double),
		  typeof(PieChartItem),
		  new FrameworkPropertyMetadata(0.5));

		#endregion // end of RadiusStartRatio

		#region Caption

		public object Caption
		{
			get { return NewLegend.GetDescription(this); }
			set { NewLegend.SetDescription(this, value); }
		}

		#endregion // end of Caption

		#endregion // end of Properties
	}
}
