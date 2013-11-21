using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace PerfCounterChart
{
	public class PerformanceInfo
	{
		public DateTime Time { get; set; }
		public double Value { get; set; }
	}

	public class PerformanceData : RingArray<PerformanceInfo>
	{
		private readonly PerformanceCounter counter;
		public PerformanceData(string categoryName, string counterName) : this(new PerformanceCounter(categoryName, counterName)) { }

		public PerformanceData(PerformanceCounter counter)
			: base(200)
		{
			if (counter == null)
				throw new ArgumentNullException("counter");

			this.counter = counter;
			timer.Tick += OnTimerTick;
			timer.Start();
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			var newInfo = new PerformanceInfo { Time = DateTime.Now, Value = counter.NextValue() };
			this.Add(newInfo);

			Debug.WriteLine(String.Format("{0}.{1}: {2}", newInfo.Time.Second, newInfo.Time.Millisecond, newInfo.Value));
		}

		private TimeSpan updateInterval = TimeSpan.FromMilliseconds(500);
		public TimeSpan UpdateInterval
		{
			get { return updateInterval; }
			set
			{
				updateInterval = value;
				timer.Interval = updateInterval;
			}
		}

		private readonly DispatcherTimer timer = new DispatcherTimer();

		public void Run()
		{
			timer.Interval = updateInterval;
			timer.IsEnabled = true;
		}

		public void Pause()
		{
			timer.IsEnabled = false;
		}
	}
}
