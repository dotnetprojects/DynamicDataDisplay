using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay;

namespace NewMarkersSample.Pages
{
	public sealed class SellInfo : INotifyPropertyChanged
	{
		private double income;
		public double Income
		{
			get { return income; }
			set { income = value; PropertyChanged.Raise(this, "Income"); }
		}

		private string cityName;
		public string CityName
		{
			get { return cityName; }
			set { cityName = value; PropertyChanged.Raise(this, "CityName"); }
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
