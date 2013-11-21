using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	public sealed class PlotterEventHelper
	{
		private RoutedEvent @event;
		internal PlotterEventHelper(RoutedEvent @event)
		{
			this.@event = @event;
		}

		// todo use a weakReference here
		private readonly Dictionary<DependencyObject, EventHandler<PlotterChangedEventArgs>> handlers = new Dictionary<DependencyObject, EventHandler<PlotterChangedEventArgs>>();

		public void Subscribe(DependencyObject target, EventHandler<PlotterChangedEventArgs> handler)
		{
			if (target == null)
				throw new ArgumentNullException("target");
			if (handler == null)
				throw new ArgumentNullException("handler");

			handlers.Add(target, handler);
		}

		internal void Notify(FrameworkElement target, PlotterChangedEventArgs args)
		{
			if (args.RoutedEvent == @event && handlers.ContainsKey(target))
			{
				var handler = handlers[target];
				handler(target, args);
			}
		}
	}
}
