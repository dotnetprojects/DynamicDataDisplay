using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherSample
{
	public class WeatherData
	{
		public WeatherType WeatherType { get; set; }
		public double Day { get; set; }
		public double Temperature { get; set; }
	}

	public enum WeatherType
	{
		Sun,
		Rain,
		Cloud,
		Thunderstorm
	}
}
