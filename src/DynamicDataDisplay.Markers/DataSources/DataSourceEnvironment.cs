using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Markers.DataSources
{
	internal class DataSourceEnvironment : IDataSourceEnvironment
	{
		#region IDataSourceEnvironment Members

		private Plotter2D plotter;
		public Plotter2D Plotter
		{
			get { return plotter; }
			internal set { this.plotter = value; }
		}

		private bool firstDraw = false;
		public bool FirstDraw
		{
			get { return firstDraw; }
			internal set { firstDraw = value; }
		}

		#endregion
	}
}
