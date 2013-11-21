using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using System.Windows.Input;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	public sealed class SelectorAxisNavigation : AxisNavigation
	{
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			Point mousePosition = e.GetPosition(ListeningPanel);
			if (mousePosition == LmbInitialPosition)
			{
				RaiseMouseLeftButtonClick(e);
			}
			base.OnMouseLeftButtonUp(e);
		}

		private void RaiseMouseLeftButtonClick(MouseButtonEventArgs e)
		{
			if (MouseLeftButtonClick != null)
			{
				MouseLeftButtonClick(this, e);
			}
		}

		public event MouseButtonEventHandler MouseLeftButtonClick;
	}
}
