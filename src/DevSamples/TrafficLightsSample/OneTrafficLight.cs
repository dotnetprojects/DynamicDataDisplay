using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace TrafficLightsSample
{
	public class OneTrafficLight : INotifyPropertyChanged
	{
		public double X { get; set; }
		public double Y { get; set; }

		private Brush fill;
		public Brush Fill
		{
			get { return fill; }
			set
			{
				fill = value;
				RaisePropertyChanged("Fill");
			}
		}

		#region INotifyPropertyChanged Members

		private void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
