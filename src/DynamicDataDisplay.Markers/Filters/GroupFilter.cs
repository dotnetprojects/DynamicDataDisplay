using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Filters;

namespace DynamicDataDisplay.Markers.Filters
{
	public abstract class GroupFilter : PointsFilter2d
	{
		private double markerSize = 10; // px
		public double MarkerSize
		{
			get { return markerSize; }
			set
			{
				markerSize = value;
				RaiseChanged();
			}
		}
	}
}
