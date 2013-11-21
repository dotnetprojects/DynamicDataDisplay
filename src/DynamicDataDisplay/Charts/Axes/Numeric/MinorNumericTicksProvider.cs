using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public sealed class MinorNumericTicksProvider : ITicksProvider<double>
	{
		private readonly ITicksProvider<double> parent;
		private Range<double>[] ranges;
		internal void SetRanges(IEnumerable<Range<double>> ranges)
		{
			this.ranges = ranges.ToArray();
		}

		private double[] coeffs;
		public double[] Coeffs
		{
			get { return coeffs; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				coeffs = value;
				Changed.Raise(this);
			}
		}

		internal MinorNumericTicksProvider(ITicksProvider<double> parent)
		{
			this.parent = parent;
			Coeffs = new double[] { 0.3, 0.3, 0.3, 0.3, 0.6, 0.3, 0.3, 0.3, 0.3 };
		}

		#region ITicksProvider<double> Members

		public event EventHandler Changed;

		public ITicksInfo<double> GetTicks(Range<double> range, int ticksCount)
		{
			if (Coeffs.Length == 0)
				return new TicksInfo<double>();

			var minorTicks = ranges.Select(r => CreateTicks(r)).SelectMany(m => m);
			var res = new TicksInfo<double>();
			res.TickSizes = minorTicks.Select(m => m.Value).ToArray();
			res.Ticks = minorTicks.Select(m => m.Tick).ToArray();

			return res;
		}

		public MinorTickInfo<double>[] CreateTicks(Range<double> range)
		{
			double step = (range.Max - range.Min) / (Coeffs.Length + 1);

			MinorTickInfo<double>[] res = new MinorTickInfo<double>[Coeffs.Length];
			for (int i = 0; i < Coeffs.Length; i++)
			{
				res[i] = new MinorTickInfo<double>(Coeffs[i], range.Min + step * (i + 1));
			}
			return res;
		}

		public int DecreaseTickCount(int ticksCount)
		{
			return ticksCount;
		}

		public int IncreaseTickCount(int ticksCount)
		{
			return ticksCount;
		}

		public ITicksProvider<double> MinorProvider
		{
			get { return null; }
		}

		public ITicksProvider<double> MajorProvider
		{
			get { return parent; }
		}

		#endregion
	}
}
