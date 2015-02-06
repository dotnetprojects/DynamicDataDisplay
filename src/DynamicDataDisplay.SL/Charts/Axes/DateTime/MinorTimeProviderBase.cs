using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Collections.Generic;

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

        private T[] mayorTicks = new T[] { };
        internal void SetTicks(T[] ticks)
        {
            this.mayorTicks = ticks;
        }

        private double ticksSize = 0.5;
        public ITicksInfo<T> GetTicks(Range<T> range, int ticksCount)
        {
            if (mayorTicks.Length == 0)
                return new TicksInfo<T>();

            ticksCount /= mayorTicks.Length;
            if (ticksCount == 0)
                ticksCount = 2;

            var ticks = mayorTicks.GetPairs().Select(r => Clip(provider.GetTicks(r, ticksCount), r)).
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
            if (mayorTicks.Length > 0)
                ticksCount /= mayorTicks.Length;

            int minorTicksCount = provider.DecreaseTickCount(ticksCount);

            if (mayorTicks.Length > 0)
                minorTicksCount *= mayorTicks.Length;

            return minorTicksCount;
        }

        public int IncreaseTickCount(int ticksCount)
        {
            if (mayorTicks.Length > 0)
                ticksCount /= mayorTicks.Length;

            int minorTicksCount = provider.IncreaseTickCount(ticksCount);

            if (mayorTicks.Length > 0)
                minorTicksCount *= mayorTicks.Length;

            return minorTicksCount;
        }

        public ITicksProvider<T> MinorProvider
        {
            get { return null; }
        }

        public ITicksProvider<T> MayorProvider
        {
            get { return null; }
        }
    }
}
