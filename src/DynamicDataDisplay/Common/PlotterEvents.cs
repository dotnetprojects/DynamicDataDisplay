using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	public static class PlotterEvents
	{
		internal static void Notify(FrameworkElement target, PlotterChangedEventArgs args)
		{
			plotterAttachedEvent.Notify(target, args);
			plotterChangedEvent.Notify(target, args);
			plotterDetachingEvent.Notify(target, args);
		}

		private static readonly PlotterEventHelper plotterAttachedEvent = new PlotterEventHelper(Plotter.PlotterAttachedEvent);
		public static PlotterEventHelper PlotterAttachedEvent
		{
			get { return plotterAttachedEvent; }
		}

		private static readonly PlotterEventHelper plotterDetachingEvent = new PlotterEventHelper(Plotter.PlotterDetachingEvent);
		public static PlotterEventHelper PlotterDetachingEvent
		{
			get { return plotterDetachingEvent; }
		}

		private static readonly PlotterEventHelper plotterChangedEvent = new PlotterEventHelper(Plotter.PlotterChangedEvent);
		public static PlotterEventHelper PlotterChangedEvent
		{
			get { return plotterChangedEvent; }
		}
	}
}
