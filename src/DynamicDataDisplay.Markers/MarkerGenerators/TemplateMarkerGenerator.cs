using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Markers;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using DynamicDataDisplay.Markers.MarkerGenerators;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using System.Windows.Shapes;
using System.Diagnostics;

namespace DynamicDataDisplay.Markers
{
	[ContentProperty("MarkerTemplate")]
	public class TemplateMarkerGenerator : MarkerGenerator
	{
		public TemplateMarkerGenerator()
		{
		}

		public TemplateMarkerGenerator(DataTemplate markerTemplate)
		{
			MarkerTemplate = markerTemplate;
		}

		public DataTemplate MarkerTemplate
		{
			get { return (DataTemplate)GetValue(MarkerTemplateProperty); }
			set { SetValue(MarkerTemplateProperty, value); }
		}

		public static readonly DependencyProperty MarkerTemplateProperty = DependencyProperty.Register(
		  "MarkerTemplate",
		  typeof(DataTemplate),
		  typeof(TemplateMarkerGenerator),
		  new FrameworkPropertyMetadata(null, OnMarkerTemplateChanged));

		private static void OnMarkerTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TemplateMarkerGenerator owner = (TemplateMarkerGenerator)d;
			// todo 
		}

		protected override FrameworkElement CreateMarkerCore(object dataItem)
		{
			DataTemplate markerTemplate = MarkerTemplate;
			if (markerTemplate == null)
				throw new InvalidOperationException("Cannot create a marker while MarkerTemplate is null.");

			var marker = (FrameworkElement)markerTemplate.LoadContent();
			marker.DataContext = dataItem;

			return marker;
		}
	}
}
