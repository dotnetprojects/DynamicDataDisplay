using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Threading;

namespace Microsoft.Research.DynamicDataDisplay
{
	public class PopupTip : Popup
	{
		private TimeSpan showDurationInerval = new TimeSpan(0, 0, 10);
		private Timer timer;

		public void ShowDelayed(TimeSpan delay)
		{
			if (timer != null)
				timer.Change((int)delay.TotalMilliseconds, System.Threading.Timeout.Infinite);
			else
				timer = new Timer(OnTimerFinished, null, (int)delay.TotalMilliseconds, System.Threading.Timeout.Infinite);
		}

		public void HideDelayed(TimeSpan delay)
		{
			if (timer != null)
			{
				timer.Change((int)delay.TotalMilliseconds, System.Threading.Timeout.Infinite);
			}
			else
				timer = new Timer(OnTimerFinished, null, (int)delay.TotalMilliseconds, System.Threading.Timeout.Infinite);
		}

		public void Hide()
		{
			if (timer != null)
			{
				timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
			}
			this.IsOpen = false;
		}

		private void OnTimerFinished(object state)
		{
			this.Dispatcher.BeginInvoke(new Action(() =>
				{
					bool show = !this.IsOpen;
					this.IsOpen = show;
					if (show)
						HideDelayed(showDurationInerval);
				}));
		}
	}
}
