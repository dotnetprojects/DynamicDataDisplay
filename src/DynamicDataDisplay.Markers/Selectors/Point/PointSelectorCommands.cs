using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Research.DynamicDataDisplay.Markers.Strings;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	public static class PointSelectorCommands
	{
		private static readonly RoutedUICommand removePoint = new RoutedUICommand(UIResources.PointSelector_RemovePoint, "RemovePoint", typeof(PointSelectorCommands));
		public static RoutedUICommand RemovePoint
		{
			get { return removePoint; }
		}

		private static readonly RoutedUICommand changeMode = new RoutedUICommand("Change mode", "ChangeMode", typeof(PointSelectorCommands));
		public static RoutedUICommand ChangeMode
		{
			get { return changeMode; }
		}

		private static readonly RoutedUICommand addPoint = new RoutedUICommand(UIResources.PointSelector_AddPoint, "AddPoint", typeof(PointSelectorCommands));
		public static RoutedUICommand AddPoint
		{
			get { return addPoint; }
		} 
	}
}
