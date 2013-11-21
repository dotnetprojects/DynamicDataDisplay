using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
	/// <summary>
	/// Represents a ticks provider for intefer values.
	/// </summary>
	public class IntegerTicksProvider : ITicksProvider<int>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IntegerTicksProvider"/> class.
		/// </summary>
		public IntegerTicksProvider() { }

		private int minStep = 0;
		/// <summary>
		/// Gets or sets the minimal step between ticks.
		/// </summary>
		/// <value>The min step.</value>
		public int MinStep
		{
			get { return minStep; }
			set
			{
				Verify.IsTrue(value >= 0, "value");
				if (minStep != value)
				{
					minStep = value;
					RaiseChangedEvent();
				}
			}
		}

		private int maxStep = Int32.MaxValue;
		/// <summary>
		/// Gets or sets the maximal step between ticks.
		/// </summary>
		/// <value>The max step.</value>
		public int MaxStep
		{
			get { return maxStep; }
			set
			{
				if (maxStep != value)
				{
					if (value < 0)
						throw new ArgumentOutOfRangeException("value", Strings.Exceptions.ParameterShouldBePositive);

					maxStep = value;
					RaiseChangedEvent();
				}
			}
		}

		#region ITicksProvider<int> Members

		/// <summary>
		/// Generates ticks for given range and preferred ticks count.
		/// </summary>
		/// <param name="range">The range.</param>
		/// <param name="ticksCount">The ticks count.</param>
		/// <returns></returns>
		public ITicksInfo<int> GetTicks(Range<int> range, int ticksCount)
		{
			double start = range.Min;
			double finish = range.Max;

			double delta = finish - start;

			int log = (int)Math.Round(Math.Log10(delta));

			double newStart = RoundingHelper.Round(start, log);
			double newFinish = RoundingHelper.Round(finish, log);
			if (newStart == newFinish)
			{
				log--;
				newStart = RoundingHelper.Round(start, log);
				newFinish = RoundingHelper.Round(finish, log);
			}

			// calculating step between ticks
			double unroundedStep = (newFinish - newStart) / ticksCount;
			int stepLog = log;
			// trying to round step
			int step = (int)RoundingHelper.Round(unroundedStep, stepLog);
			if (step == 0)
			{
				stepLog--;
				step = (int)RoundingHelper.Round(unroundedStep, stepLog);
				if (step == 0)
				{
					// step will not be rounded if attempts to be rounded to zero.
					step = (int)unroundedStep;
				}
			}

			if (step < minStep)
				step = minStep;
			if (step > maxStep)
				step = maxStep;

			if (step <= 0)
				step = 1;

			int[] ticks = CreateTicks(start, finish, step);

			TicksInfo<int> res = new TicksInfo<int> { Info = log, Ticks = ticks };

			return res;
		}

		private static int[] CreateTicks(double start, double finish, int step)
		{
			DebugVerify.Is(step != 0);

			int x = (int)(step * Math.Floor(start / (double)step));
			List<int> res = new List<int>();

			checked
			{
				double increasedFinish = finish + step * 1.05;
				while (x <= increasedFinish)
				{
					res.Add(x);
					x += step;
				}
			}
			return res.ToArray();
		}

		private static int[] tickCounts = new int[] { 20, 10, 5, 4, 2, 1 };

		/// <summary>
		/// Decreases the tick count.
		/// Returned value should be later passed as ticksCount parameter to GetTicks method.
		/// </summary>
		/// <param name="ticksCount">The ticks count.</param>
		/// <returns>Decreased ticks count.</returns>
		public int DecreaseTickCount(int ticksCount)
		{
			return tickCounts.FirstOrDefault(tick => tick < ticksCount);
		}

		/// <summary>
		/// Increases the tick count.
		/// Returned value should be later passed as ticksCount parameter to GetTicks method.
		/// </summary>
		/// <param name="ticksCount">The ticks count.</param>
		/// <returns>Increased ticks count.</returns>
		public int IncreaseTickCount(int ticksCount)
		{
			int newTickCount = tickCounts.Reverse().FirstOrDefault(tick => tick > ticksCount);
			if (newTickCount == 0)
				newTickCount = tickCounts[0];

			return newTickCount;
		}

		/// <summary>
		/// Gets the minor ticks provider, used to generate ticks between each two adjacent ticks.
		/// </summary>
		/// <value>The minor provider.</value>
		public ITicksProvider<int> MinorProvider
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the major provider, used to generate major ticks - for example, years for common ticks as months.
		/// </summary>
		/// <value>The major provider.</value>
		public ITicksProvider<int> MajorProvider
		{
			get { return null; }
		}

		protected void RaiseChangedEvent()
		{
			Changed.Raise(this);
		}
		public event EventHandler Changed;

		#endregion
	}
}
