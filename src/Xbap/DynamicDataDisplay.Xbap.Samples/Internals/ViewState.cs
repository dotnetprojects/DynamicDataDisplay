using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals
{
	public class ViewState : DependencyObject, INotifyPropertyChanged
	{
		private static readonly ViewState state = new ViewState();

		public static ViewState State { get { return state; } }

		private object value;
		public object SelectedValue
		{
			get { return value; }
			set
			{
				this.value = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("SelectedValue"));
				}
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
