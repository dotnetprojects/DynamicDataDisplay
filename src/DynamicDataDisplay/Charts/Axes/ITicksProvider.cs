using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Contains information about one minor tick - its value (relative size) and its tick.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerDisplay("{Value} @ {Tick}")]
	public struct MinorTickInfo<T>
	{
		internal MinorTickInfo(double value, T tick)
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

	/// <summary>
	/// Contains data for all generated ticks.
	/// Used by TicksLabelProvider.
	/// </summary>
	/// <typeparam name="T">Type of axis tick.</typeparam>
	public interface ITicksInfo<T>
	{
		/// <summary>
		/// Gets the array of axis ticks.
		/// </summary>
		/// <value>The ticks.</value>
		T[] Ticks { get; }
		/// <summary>
		/// Gets the tick sizes.
		/// </summary>
		/// <value>The tick sizes.</value>
		double[] TickSizes { get; }
		/// <summary>
		/// Gets the additional information, added to ticks info and specifying range's features.
		/// </summary>
		/// <value>The info.</value>
		object Info { get; }
	}

	internal class TicksInfo<T> : ITicksInfo<T>
	{
		private T[] ticks = { };
		/// <summary>
		/// Gets the array of axis ticks.
		/// </summary>
		/// <value>The ticks.</value>
		public T[] Ticks
		{
			get { return ticks; }
			internal set { ticks = value; }
		}

		private double[] tickSizes = { };
		/// <summary>
		/// Gets the tick sizes.
		/// </summary>
		/// <value>The tick sizes.</value>
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
		/// <summary>
		/// Gets the additional information, added to ticks info and specifying range's features.
		/// </summary>
		/// <value>The info.</value>
		public object Info
		{
			get { return info; }
			internal set { info = value; }
		}

		private static readonly TicksInfo<T> empty = new TicksInfo<T> { info = null, ticks = new T[0], tickSizes = new double[0] };
		internal static TicksInfo<T> Empty
		{
			get { return empty; }
		}
	}

	/// <summary>
	///	Base interface for ticks generator.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ITicksProvider<T>
	{
		/// <summary>
		/// Generates ticks for given range and preferred ticks count.
		/// </summary>
		/// <param name="range">The range.</param>
		/// <param name="ticksCount">The ticks count.</param>
		/// <returns></returns>
		ITicksInfo<T> GetTicks(Range<T> range, int ticksCount);
		/// <summary>
		/// Decreases the tick count.
		/// Returned value should be later passed as ticksCount parameter to GetTicks method.
		/// </summary>
		/// <param name="ticksCount">The ticks count.</param>
		/// <returns>Decreased ticks count.</returns>
		int DecreaseTickCount(int ticksCount);
		/// <summary>
		/// Increases the tick count.
		/// Returned value should be later passed as ticksCount parameter to GetTicks method.
		/// </summary>
		/// <param name="ticksCount">The ticks count.</param>
		/// <returns>Increased ticks count.</returns>
		int IncreaseTickCount(int ticksCount);

		/// <summary>
		/// Gets the minor ticks provider, used to generate ticks between each two adjacent ticks.
		/// </summary>
		/// <value>The minor provider. If there is no minor provider available, returns null.</value>
		ITicksProvider<T> MinorProvider { get; }
		/// <summary>
		/// Gets the major provider, used to generate major ticks - for example, years for common ticks as months.
		/// </summary>
		/// <value>The major provider. If there is no major provider available, returns null.</value>
		ITicksProvider<T> MajorProvider { get; }

		/// <summary>
		/// Occurs when properties of ticks provider changeds.
		/// Notifies axis to rebuild its view.
		/// </summary>
		event EventHandler Changed;
	}
}
