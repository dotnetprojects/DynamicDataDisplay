using System;
using System.Net;
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
    public abstract class TimeTicksProviderBase<T> : ITicksProvider<T>
    {
        public event EventHandler Changed;
        protected void RaiseChanged()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }

        private static readonly Dictionary<DifferenceIn, ITicksProvider<T>> providers =
            new Dictionary<DifferenceIn, ITicksProvider<T>>();

        protected static Dictionary<DifferenceIn, ITicksProvider<T>> Providers
        {
            get { return TimeTicksProviderBase<T>.providers; }
        }

        private static readonly Dictionary<DifferenceIn, ITicksProvider<T>> minorProviders =
            new Dictionary<DifferenceIn, ITicksProvider<T>>();

        protected static Dictionary<DifferenceIn, ITicksProvider<T>> MinorProviders
        {
            get { return TimeTicksProviderBase<T>.minorProviders; }
        }

        protected abstract TimeSpan GetDifference(T start, T end);

        #region ITicksProvider<T> Members

        private IDateTimeTicksStrategy strategy = new DefaultDateTimeTicksStrategy();
        public IDateTimeTicksStrategy Strategy
        {
            get { return strategy; }
            set
            {
                if (strategy != value)
                {
                    strategy = value;
                    RaiseChanged();
                }
            }
        }

        private ITicksInfo<T> result;
        private DifferenceIn diff;

        public ITicksInfo<T> GetTicks(Range<T> range, int ticksCount)
        {
            Verify.IsTrue(ticksCount > 0);

            T start = range.Min;
            T end = range.Max;
            TimeSpan length = GetDifference(start, end);

            diff = strategy.GetDifference(length);

            TicksInfo<T> res = new TicksInfo<T> { Info = diff };
            if (providers.ContainsKey(diff))
            {
                ITicksInfo<T> result = providers[diff].GetTicks(range, ticksCount);
                T[] mayorTicks = result.Ticks;

                res.Ticks = mayorTicks;
                this.result = res;
                return res;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Decreases the tick count.
        /// </summary>
        /// <param name="tickCount">The tick count.</param>
        /// <returns></returns>
        public int DecreaseTickCount(int ticksCount)
        {
            if (providers.ContainsKey(diff))
                return providers[diff].DecreaseTickCount(ticksCount);

            int res = ticksCount / 2;
            if (res < 2) res = 2;
            return res;
        }

        /// <summary>
        /// Increases the tick count.
        /// </summary>
        /// <param name="tickCount">The tick count.</param>
        /// <returns></returns>
        public int IncreaseTickCount(int ticksCount)
        {
            DebugVerify.Is(ticksCount < 2000);

            if (providers.ContainsKey(diff))
                return providers[diff].IncreaseTickCount(ticksCount);

            return ticksCount * 2;
        }

        public ITicksProvider<T> MinorProvider
        {
            get
            {
                DifferenceIn smallerDiff = DifferenceIn.Smallest;
                if (strategy.TryGetLowerDiff(diff, out smallerDiff) && minorProviders.ContainsKey(smallerDiff))
                {
                    var minorProvider = (MinorTimeProviderBase<T>)minorProviders[smallerDiff];
                    minorProvider.SetTicks(result.Ticks);
                    return minorProvider;
                }

                return null;
                // todo What to do if this already is the smallest provider?
            }
        }

        public ITicksProvider<T> MayorProvider
        {
            get
            {
                DifferenceIn biggerDiff = DifferenceIn.Smallest;
                if (strategy.TryGetBiggerDiff(diff, out biggerDiff))
                {
                    return providers[biggerDiff];
                }

                return null;
                // todo What to do if this already is the biggest provider?
            }
        }

        #endregion
    }
}
