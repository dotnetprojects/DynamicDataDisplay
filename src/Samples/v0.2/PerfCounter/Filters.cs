using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerfCounterChart
{
	public class MaxSizeFilter : IFilter<PerformanceInfo>
	{
		TimeSpan length = TimeSpan.FromSeconds(10);
		public IList<PerformanceInfo> Filter(IList<PerformanceInfo> c)
		{
			if (c.Count == 0)
				return new List<PerformanceInfo>();

			DateTime end = c[c.Count - 1].Time;

			int startIndex = 0;
			for (int i = 0; i < c.Count; i++)
			{
				if (end - c[i].Time <= length)
				{
					startIndex = i;
					break;
				}
			}

			List<PerformanceInfo> res = new List<PerformanceInfo>(c.Count - startIndex);
			for (int i = startIndex; i < c.Count; i++)
			{
				res.Add(c[i]);
			}
			return res;
		}

	}

	public class FilterChain : IFilter<PerformanceInfo>
	{
		private readonly IFilter<PerformanceInfo>[] filters;
		public FilterChain(params IFilter<PerformanceInfo>[] filters)
		{
			this.filters = filters;
		}

		#region IFilter<PerformanceInfo> Members

		public IList<PerformanceInfo> Filter(IList<PerformanceInfo> c)
		{
			foreach (var filter in filters)
			{
				c = filter.Filter(c);
			}
			return c;
		}

		#endregion
	}

	public class AverageFilter : IFilter<PerformanceInfo>
	{
		private int number = 2;
		public int Number
		{
			get { return number; }
			set { number = value; }
		}

		public IList<PerformanceInfo> Filter(IList<PerformanceInfo> c)
		{
			int num = number - 1;
			if (c.Count - num <= 0)
				return c;

			List<PerformanceInfo> res = new List<PerformanceInfo>(c.Count - num);
			for (int i = 0; i < c.Count - num; i++)
			{
				double doubleSum = 0;
				long ticksSum = 0;
				for (int j = i; j < i + number; j++)
				{
					doubleSum += c[j].Value;
					ticksSum += c[j].Time.Ticks / number;
				}
				doubleSum /= number;
				res.Add(new PerformanceInfo { Time = new DateTime(ticksSum), Value = doubleSum });
			}
			return res;
		}
	}
}
