using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Media;
using System.Windows.Threading;
using System.Media;

namespace TrafficLightsSample
{
	public class TrafficLight : IEnumerable<OneTrafficLight>
	{
		private OneTrafficLight redLight = new OneTrafficLight { X = 0.5, Y = 0.3, Fill = Brushes.Red };
		private OneTrafficLight yellowLight = new OneTrafficLight { X = 0.5, Y = 0.2, Fill = Brushes.Yellow };
		private OneTrafficLight greenLight = new OneTrafficLight { X = 0.5, Y = 0.1, Fill = Brushes.Green };

		private List<LightStep> steps;
		int currentIndex = 0;
		DispatcherTimer timer = new DispatcherTimer();

		public TrafficLight()
		{
			lights.Add(redLight);
			lights.Add(yellowLight);
			lights.Add(greenLight);

			const int greenFlash = 500;
			steps = new List<LightStep>
			{
				new LightStep{ Duration = TimeSpan.FromSeconds(5), Red = Brushes.Red },
				new LightStep{ Duration = TimeSpan.FromSeconds(1), Red = Brushes.Red, Yellow = Brushes.Yellow },
				new LightStep{ Duration = TimeSpan.FromSeconds(5), Green = Brushes.Green },
				new LightStep{ Duration = TimeSpan.FromMilliseconds(greenFlash) },
				new LightStep{ Duration = TimeSpan.FromMilliseconds(greenFlash), Green = Brushes.Green },
				new LightStep{ Duration = TimeSpan.FromMilliseconds(greenFlash) },
				new LightStep{ Duration = TimeSpan.FromMilliseconds(greenFlash), Green = Brushes.Green },
				new LightStep{ Duration = TimeSpan.FromMilliseconds(greenFlash) },
				new LightStep{ Duration = TimeSpan.FromMilliseconds(greenFlash), Green = Brushes.Green },
				new LightStep{ Duration = TimeSpan.FromSeconds(1), Yellow = Brushes.Yellow }
			};

			ApplyStep();
			timer.Interval = steps[0].Duration;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			currentIndex++;
			if (currentIndex >= steps.Count)
				currentIndex = 0;

			ApplyStep();
		}

		private void ApplyStep()
		{
			var step = steps[currentIndex];

			redLight.Fill = step.Red;
			yellowLight.Fill = step.Yellow;
			greenLight.Fill = step.Green;

			timer.Interval = step.Duration;
		}

		private readonly List<OneTrafficLight> lights = new List<OneTrafficLight>();

		#region IEnumerable<OneTrafficLight> Members

		public IEnumerator<OneTrafficLight> GetEnumerator()
		{
			return lights.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		class LightStep
		{
			public LightStep()
			{
				Red = Green = Yellow = Brushes.LightGray;
			}

			public TimeSpan Duration { get; set; }
			public Brush Red { get; set; }
			public Brush Yellow { get; set; }
			public Brush Green { get; set; }
		}
	}
}
