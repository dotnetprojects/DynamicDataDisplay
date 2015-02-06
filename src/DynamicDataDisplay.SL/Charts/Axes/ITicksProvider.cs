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
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    [DebuggerDisplay("{Value} @ {Tick}")]
    public struct MinorTickInfo<T>
    {
        public MinorTickInfo(double value, T tick)
        {
            this.value = value;
            this.tick = tick;
        }

        private readonly double value;
        private readonly T tick;

        public double Value { get { return value; } }
        public T Tick { get { return tick; } }

        public override string ToString()
        {
            return String.Format("{0} @ {1}", value, tick);
        }
    }

    public interface ITicksInfo<T>
    {
        T[] Ticks { get; }
        double[] TickSizes { get; }
        object Info { get; }
    }

    internal class TicksInfo<T> : ITicksInfo<T>
    {
        private T[] ticks = { };
        public T[] Ticks
        {
            get { return ticks; }
            internal set { ticks = value; }
        }

        private double[] tickSizes = { };
        public double[] TickSizes
        {
            get
            {
                if (tickSizes.Length != ticks.Length)
                    tickSizes = ArrayExtensions.CreateArray(ticks.Length, 1.0);

                return tickSizes;
            }
            internal set { tickSizes = value; }
        }

        private object info = null;
        public object Info
        {
            get { return info; }
            internal set { info = value; }
        }
    }

    public interface ITicksProvider<T>
    {
        ITicksInfo<T> GetTicks(Range<T> range, int ticksCount);
        int DecreaseTickCount(int ticksCount);
        int IncreaseTickCount(int ticksCount);

        ITicksProvider<T> MinorProvider { get; }
        ITicksProvider<T> MayorProvider { get; }

        event EventHandler Changed;
    }
}
