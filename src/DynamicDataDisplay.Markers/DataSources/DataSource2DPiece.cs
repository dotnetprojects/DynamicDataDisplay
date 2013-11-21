using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DynamicDataDisplay.Markers.DataSources
{
	internal sealed class DataSource2DPiece<T>
	{
		public Point Position { get; set; }
		public T Data { get; set; }
	}
}
