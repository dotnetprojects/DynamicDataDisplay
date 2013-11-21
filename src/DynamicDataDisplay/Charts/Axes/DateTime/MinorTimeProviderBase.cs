using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	internal abstract class MinorTimeProviderBase<T> : ITicksProvider<T>
	{
		public event EventHandler Changed;
		protected void RaiseChanged()
		{
			if (Changed != null)
			{
				Changed(this, EventArgs.Empty);
			}
		}

		private readonly ITicksProvider<T> provider;
		public MinorTimeProviderBase(ITicksProvider<T> provider)
		{
			this.provider = provider;
		}

		private T[] majorTicks = new T[] { };
		internal void SetTicks(T[] ticks)
		{
			this.majorTicks = ticks;
		}

		private double ticksSize = 0.5;
		public ITicksInfo<T> GetTicks(Range<T> range, int ticksCount)
		{
			if (majorTicks.Length == 0)
				return new TicksInfo<T>();

			ticksCount /= majorTicks.Length;
			if (ticksCount == 0)
				ticksCount = 2;

			var ticks = majorTicks.GetPairs().Select(r => Clip(provider.GetTicks(r, ticksCount), r)).
				SelectMany(t => t.Ticks).ToArray();

			var res = new TicksInfo<T>
			{
				Ticks = ticks,
				TickSizes = ArrayExtensions.CreateArray(ticks.Length, ticksSize)
			};
			return res;
		}

		private ITicksInfo<T> Clip(ITicksInfo<T> ticks, Range<T> range)
		{
			var newTicks = new List<T>(ticks.Ticks.Length);
			var newSizes = new List<double>(ticks.TickSizes.Length);

			for (int i = 0; i < ticks.Ticks.Length; i++)
			{
				T tick = ticks.Ticks[i];
				if (IsInside(tick, range))
				{
					newTicks.Add(tick);
					newSizes.Add(ticks.TickSizes[i]);
				}
			}

			return new TicksInfo<T>
			{
				Ticks = newTicks.ToArray(),
				TickSizes = newSizes.ToArray(),
				Info = ticks.Info
			};
		}

		protected abstract bool IsInside(T value, Range<T> range);

		public int DecreaseTickCount(int ticksCount)
		{
			if (majorTicks.Length > 0)
				ticksCount /= majorTicks.Length;

			int minorTicksCount = provider.DecreaseTickCount(ticksCount);

			if (majorTicks.Length > 0)
				minorTicksCount *= majorTicks.Length;

			return minorTicksCount;
		}

		public int IncreaseTickCount(int ticksCount)
		{
			if (majorTicks.Length > 0)
				ticksCount /= majorTicks.Length;

			int minorTicksCount = provider.IncreaseTickCount(ticksCount);

			if (majorTicks.Length > 0)
				minorTicksCount *= majorTicks.Length;

			return minorTicksCount;
		}

		public ITicksProvider<T> MinorProvider
		{
			get { return null; }
		}

		public ITicksProvider<T> MajorProvider
		{
			get { return null; }
		}
	}
}
