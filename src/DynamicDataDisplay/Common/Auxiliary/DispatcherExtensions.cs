using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class DispatcherExtensions
	{
		public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action)
		{
			return dispatcher.BeginInvoke((Delegate)action);
		}

		public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
		{
			return dispatcher.BeginInvoke(action, priority);
		}

		public static void Invoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
		{
			dispatcher.Invoke(action, priority);
		}
	}
}
