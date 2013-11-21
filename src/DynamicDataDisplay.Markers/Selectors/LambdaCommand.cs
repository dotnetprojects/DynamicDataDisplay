using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	public class LambdaCommand : ICommand
	{
		private readonly Func<object, bool> canExecuteHandler;
		private readonly Action<object> executeHandler;

		public LambdaCommand(Action<object> executeHandler, Func<object, bool> canExecuteHandler)
		{
			if (canExecuteHandler == null)
				throw new ArgumentNullException("canExecuteHandler");
			if (executeHandler == null)
				throw new ArgumentNullException("executeHandler");

			this.executeHandler = executeHandler;
			this.canExecuteHandler = canExecuteHandler;
		}

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			bool result = canExecuteHandler(parameter);
			return result;
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged.Raise(this);
		}
		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			executeHandler(parameter);
		}

		#endregion
	}
}
