using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public sealed class NumericTicksProvider : ITicksProvider<double>
    {
        public NumericTicksProvider()
        {
            minorProvider = new MinorNumericTicksProvider(this);
            minorProvider.Coeffs = new double[] { 0.3, 0.3, 0.3, 0.3, 0.6, 0.3, 0.3, 0.3, 0.3 };
        }

        public event EventHandler Changed;
        private void RaiseChangedEvent()
        {
            Changed.Raise(this);
        }

        private double minStep = 0.0;
        public double MinStep
        {
            get { return minStep; }
            set
            {
                Verify.IsTrue(value >= 0.0, "value");
                if (minStep != value)
                {
                    minStep = value;
                    RaiseChangedEvent();
                }
            }
        }

        private double[] ticks;
        public ITicksInfo<double> GetTicks(Range<double> range, int ticksCount)
        {
            double start = range.Min;
            double finish = range.Max;

            double delta = finish - start;

            int log = (int)Math.Round(Math.Log10(delta));

            double newStart = RoundHelper.Round(start, log);
            double newFinish = RoundHelper.Round(finish, log);
            if (newStart == newFinish)
            {
                log--;
                newStart = RoundHelper.Round(start, log);
                newFinish = RoundHelper.Round(finish, log);
            }

            // calculating step between ticks
            double unroundedStep = (newFinish - newStart) / ticksCount;
            int stepLog = log;
            // trying to round step
            double step = RoundHelper.Round(unroundedStep, stepLog);
            if (step == 0)
            {
                stepLog--;
                step = RoundHelper.Round(unroundedStep, stepLog);
                if (step == 0)
                {
                    // step will not be rounded if attempts to be rounded to zero.
                    step = unroundedStep;
                }
            }

            if (step < minStep)
                step = minStep;

            if (step != 0.0)
            {
                ticks = CreateTicks(start, finish, step);
            }
            else
            {
                ticks = new double[] { };
            }

            TicksInfo<double> res = new TicksInfo<double> { Info = log, Ticks = ticks };

            return res;
        }

        private static double[] CreateTicks(double start, double finish, double step)
        {
            DebugVerify.Is(step != 0.0);

            double x = step * Math.Floor(start / step);
            List<double> res = new List<double>();

            double increasedFinish = finish + step * 1.05;
            //double increasedFinish = finish;
            
            while (x <= increasedFinish)
            {
                res.Add(x);
                x += step;
            }
            return res.ToArray();
        }

        private static int[] tickCounts = new int[] { 20, 10, 5, 4, 2, 1 };

        public const int DefaultPreferredTicksCount = 10;

        public int DecreaseTickCount(int ticksCount)
        {
            return tickCounts.FirstOrDefault(tick => tick < ticksCount);
        }

        public int IncreaseTickCount(int ticksCount)
        {
            int newTickCount = tickCounts.Reverse().FirstOrDefault(tick => tick > ticksCount);
            if (newTickCount == 0)
                newTickCount = tickCounts[0];

            return newTickCount;
        }

        #region ITicksProvider<double> Members

        private readonly MinorNumericTicksProvider minorProvider;
        public ITicksProvider<double> MinorProvider
        {
            get
            {
                minorProvider.SetRanges(ticks.GetPairs());

                return minorProvider;
            }
        }

        public ITicksProvider<double> MayorProvider
        {
            get { return null; }
        }

        #endregion
    }
}
