using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class TaskExtensions
	{
		/// <summary>
		/// Logs exceptions that occur during task execution.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns></returns>
		public static Task WithExceptionLogging(this Task task)
		{
			return task.ContinueWith(t =>
			{
				var exception = t.Exception;
				if (exception != null)
				{
					if (exception.InnerException != null)
						exception = exception.InnerException;

					Debug.WriteLine("Failure in async task: " + exception.Message);
				}
			}, TaskContinuationKind.OnFailed);
		}

		/// <summary>
		/// Rethrows exceptions thrown during task execution in thespecified dispatcher thread.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="dispatcher">The dispatcher.</param>
		/// <returns></returns>
		public static Task WithExceptionThrowingInDispatcher(this Task task, Dispatcher dispatcher)
		{
			return task.ContinueWith(t =>
			{
				dispatcher.BeginInvoke(() =>
				{
					throw t.Exception;
				}, DispatcherPriority.Send);
			}, TaskContinuationKind.OnFailed);
		}
	}
}
