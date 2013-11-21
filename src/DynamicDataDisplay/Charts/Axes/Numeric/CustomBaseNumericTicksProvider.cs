using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric
{
	[ContentProperty("TicksProvider")]
	public class CustomBaseNumericTicksProvider : ITicksProvider<double>
	{
		private double customBase = 2;

		/// <summary>
		/// Gets or sets the custom base.
		/// </summary>
		/// <value>The custom base.</value>
		public double CustomBase
		{
			get { return customBase; }
			set
			{
				if (Double.IsNaN(value))
					throw new ArgumentException(Strings.Exceptions.CustomBaseTicksProviderBaseIsNaN);
				if (value <= 0)
					throw new ArgumentOutOfRangeException(Strings.Exceptions.CustomBaseTicksProviderBaseIsTooSmall);

				customBase = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomBaseNumericTicksProvider"/> class.
		/// </summary>
		public CustomBaseNumericTicksProvider() : this(2.0) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomBaseNumericTicksProvider"/> class.
		/// </summary>
		/// <param name="customBase">The custom base, e.g. Math.PI</param>
		public CustomBaseNumericTicksProvider(double customBase) : this(customBase, new NumericTicksProvider()) { }

		private CustomBaseNumericTicksProvider(double customBase, ITicksProvider<double> ticksProvider)
		{
			if (ticksProvider == null)
				throw new ArgumentNullException("ticksProvider");

			CustomBase = customBase;

			TicksProvider = ticksProvider;
		}

		private void ticksProvider_Changed(object sender, EventArgs e)
		{
			Changed.Raise(this);
		}

		private ITicksProvider<double> ticksProvider = null;
		public ITicksProvider<double> TicksProvider
		{
			get { return ticksProvider; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (ticksProvider != null)
					ticksProvider.Changed -= ticksProvider_Changed;
				ticksProvider = value;
				ticksProvider.Changed += ticksProvider_Changed;

				if (minorTicksProvider != null)
					minorTicksProvider.Changed -= minorTicksProvider_Changed;
				minorTicksProvider = new MinorProviderWrapper(this);
				minorTicksProvider.Changed += minorTicksProvider_Changed;

				Changed.Raise(this);
			}
		}

		void minorTicksProvider_Changed(object sender, EventArgs e)
		{
			Changed.Raise(this);
		}

		private Range<double> TransformRange(Range<double> range)
		{
			double min = range.Min / customBase;
			double max = range.Max / customBase;

			return new Range<double>(min, max);
		}

		#region ITicksProvider<double> Members

		private double[] tickMarks;
		public ITicksInfo<double> GetTicks(Range<double> range, int ticksCount)
		{
			var ticks = ticksProvider.GetTicks(TransformRange(range), ticksCount);

			TransformTicks(ticks);

			tickMarks = ticks.Ticks;

			return ticks;
		}

		private void TransformTicks(ITicksInfo<double> ticks)
		{
			for (int i = 0; i < ticks.Ticks.Length; i++)
			{
				ticks.Ticks[i] *= customBase;
			}
		}

		public int DecreaseTickCount(int ticksCount)
		{
			return ticksProvider.DecreaseTickCount(ticksCount);
		}

		public int IncreaseTickCount(int ticksCount)
		{
			return ticksProvider.IncreaseTickCount(ticksCount);
		}

		private ITicksProvider<double> minorTicksProvider;
		public ITicksProvider<double> MinorProvider
		{
			get { return minorTicksProvider; }
		}

		/// <summary>
		/// Gets the major provider, used to generate major ticks - for example, years for common ticks as months.
		/// </summary>
		/// <value>The major provider.</value>
		public ITicksProvider<double> MajorProvider
		{
			get { return null; }
		}

		public event EventHandler Changed;

		#endregion

		private sealed class MinorProviderWrapper : ITicksProvider<double>
		{
			private readonly CustomBaseNumericTicksProvider owner;

			public MinorProviderWrapper(CustomBaseNumericTicksProvider owner)
			{
				this.owner = owner;

				MinorTicksProvider.Changed += MinorTicksProvider_Changed;
			}

			private void MinorTicksProvider_Changed(object sender, EventArgs e)
			{
				Changed.Raise(this);
			}

			private ITicksProvider<double> MinorTicksProvider
			{
				get { return owner.ticksProvider.MinorProvider; }
			}

			#region ITicksProvider<double> Members

			public ITicksInfo<double> GetTicks(Range<double> range, int ticksCount)
			{
				var minorProvider = MinorTicksProvider;
				var ticks = minorProvider.GetTicks(range, ticksCount);

				return ticks;
			}

			public int DecreaseTickCount(int ticksCount)
			{
				return MinorTicksProvider.DecreaseTickCount(ticksCount);
			}

			public int IncreaseTickCount(int ticksCount)
			{
				return MinorTicksProvider.IncreaseTickCount(ticksCount);
			}

			public ITicksProvider<double> MinorProvider
			{
				get { return MinorTicksProvider.MinorProvider; }
			}

			public ITicksProvider<double> MajorProvider
			{
				get { return owner; }
			}

			public event EventHandler Changed;

			#endregion
		}
	}
}
