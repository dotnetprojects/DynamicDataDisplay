using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Markers;
using System.Windows;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers
{
	public class VectorFieldChart : DevMarkerChart
	{
		static VectorFieldChart()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(VectorFieldChart), new FrameworkPropertyMetadata(typeof(VectorFieldChart)));
		}

		public VectorFieldChart()
		{
			MarkerBuilder = new VectorFieldItemGenerator();
		}

		public string LocationPath { get; set; }
		public string DirectionPath { get; set; }

		public override void EndInit()
		{
			VectorFieldItemGenerator generator = (VectorFieldItemGenerator)MarkerBuilder;
			generator.LocationPath = LocationPath;
			generator.DirectionPath = DirectionPath;
			generator.EndInit();
	
			base.EndInit();
		}
	}
}
