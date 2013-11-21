using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay;

namespace NewMarkersSample
{
	public class Segment : INotifyPropertyChanged
	{
		private double yMin;
		public double YMin { get { return yMin; } set { yMin = value; PropertyChanged.Raise(this, "YMin"); } }

		private double yMax;
		public double YMax { get { return yMax; } set { yMax = value; PropertyChanged.Raise(this, "YMax"); } }

		private double x;
		public double X { get { return x; } set { x = value; PropertyChanged.Raise(this, "X"); } }

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public static Segment[] LoadSegments(int count)
		{
			Segment[] segments = new Segment[count];
			double coeff = 0.33;
			for (int i = 0; i < count; i++)
			{
				Segment seg = new Segment { X = i, YMin = Math.Sin(i * coeff), YMax = Math.Sin(i * coeff) + 3 };
				segments[i] = seg;
			}
			return segments;
		}
	}
}
