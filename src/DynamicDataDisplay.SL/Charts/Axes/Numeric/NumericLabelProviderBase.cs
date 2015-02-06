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
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public abstract class NumericLabelProviderBase : LabelProviderBase<double>
    {
        bool shouldRound = true;
        private int rounding;
        protected void Init(double[] ticks)
        {
            if (ticks.Length == 0)
                return;

            double start = ticks[0];
            double finish = ticks[ticks.Length - 1];

            if (start == finish)
            {
                shouldRound = false;
                return;
            }

            double delta = finish - start;

            rounding = (int)Math.Round(Math.Log10(delta));

            double newStart = RoundHelper.Round(start, rounding);
            double newFinish = RoundHelper.Round(finish, rounding);
            if (newStart == newFinish)
                rounding--;
        }

        protected override string GetStringCore(LabelTickInfo<double> tickInfo)
        {
            string res;
            if (!shouldRound)
            {
                res = tickInfo.Tick.ToString();
            }
            else
            {
                int round = Math.Min(15, Math.Max(-15, rounding - 2));
                res = RoundHelper.Round(tickInfo.Tick, round).ToString();
            }

            return res;
        }
    }
}
