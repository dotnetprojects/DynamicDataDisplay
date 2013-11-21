using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay;

namespace NewMarkersSample.Pages
{
	public sealed class DateTimeRect : INotifyPropertyChanged
	{
		private DateTime start;
		public DateTime Start
		{
			get { return start; }
			set
			{
				start = value;
				PropertyChanged.Raise(this, "Start");
			}
		}

		private TimeSpan duration;
		public TimeSpan Duration
		{
			get { return duration; }
			set
			{
				duration = value;
				PropertyChanged.Raise(this, "Duration");
			}
		}

		private double y;
		public double Y
		{
			get { return y; }
			set
			{
				y = value;
				PropertyChanged.Raise(this, "Y");
			}
		}

		private double height;
		public double Height
		{
			get { return height; }
			set
			{
				height = value;
				PropertyChanged.Raise(this, "Height");
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
