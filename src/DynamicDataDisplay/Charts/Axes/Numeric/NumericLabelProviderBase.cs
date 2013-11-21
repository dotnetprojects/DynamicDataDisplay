using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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

			double newStart = RoundingHelper.Round(start, rounding);
			double newFinish = RoundingHelper.Round(finish, rounding);
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
				int round = Math.Min(15, Math.Max(-15, rounding - 3)); // was rounding - 2
				res = RoundingHelper.Round(tickInfo.Tick, round).ToString();
			}

			return res;
		}
	}
}
