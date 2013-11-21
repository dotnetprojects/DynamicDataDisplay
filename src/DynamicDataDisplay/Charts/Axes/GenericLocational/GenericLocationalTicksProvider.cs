using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.DataSearch;
using System.Windows;
using System.Collections;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes.GenericLocational
{
	public class GenericLocationalTicksProvider<TCollection, TAxis> : ITicksProvider<TAxis> where TAxis : IComparable<TAxis>
	{
		private IList<TCollection> collection;
		public IList<TCollection> Collection
		{
			get { return collection; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				Changed.Raise(this);
				collection = value;
			}
		}

		private Func<TCollection, TAxis> axisMapping;
		public Func<TCollection, TAxis> AxisMapping
		{
			get { return axisMapping; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				Changed.Raise(this);
				axisMapping = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericLocationalTicksProvider&lt;T&gt;"/> class.
		/// </summary>
		public GenericLocationalTicksProvider() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericLocationalTicksProvider&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="collection">The collection of axis ticks and labels.</param>
		public GenericLocationalTicksProvider(IList<TCollection> collection)
		{
			Collection = collection;
		}

		public GenericLocationalTicksProvider(IList<TCollection> collection, Func<TCollection, TAxis> coordinateMapping)
		{
			Collection = collection;
			AxisMapping = coordinateMapping;
		}

		#region ITicksProvider<T> Members

		SearchResult1d minResult = SearchResult1d.Empty;
		SearchResult1d maxResult = SearchResult1d.Empty;
		GenericSearcher1d<TCollection, TAxis> searcher;
		/// <summary>
		/// Generates ticks for given range and preferred ticks count.
		/// </summary>
		/// <param name="range">The range.</param>
		/// <param name="ticksCount">The ticks count.</param>
		/// <returns></returns>
		public ITicksInfo<TAxis> GetTicks(Range<TAxis> range, int ticksCount)
		{
			EnsureSearcher();

			//minResult = searcher.SearchBetween(range.Min, minResult);
			//maxResult = searcher.SearchBetween(range.Max, maxResult);

			minResult = searcher.SearchFirstLess(range.Min);
			maxResult = searcher.SearchGreater(range.Max);

			if (!(minResult.IsEmpty && maxResult.IsEmpty))
			{
				int startIndex = !minResult.IsEmpty ? minResult.Index : 0;
				int endIndex = !maxResult.IsEmpty ? maxResult.Index : collection.Count - 1;

				int count = endIndex - startIndex + 1;

				TAxis[] ticks = new TAxis[count];
				for (int i = startIndex; i <= endIndex; i++)
				{
					ticks[i - startIndex] = axisMapping(collection[i]);
				}

				TicksInfo<TAxis> result = new TicksInfo<TAxis>
				{
					Info = startIndex,
					TickSizes = ArrayExtensions.CreateArray(count, 1.0),
					Ticks = ticks
				};

				return result;
			}
			else
			{
				return TicksInfo<TAxis>.Empty;
			}
		}

		private void EnsureSearcher()
		{
			if (searcher == null)
			{
				if (collection == null || axisMapping == null)
					throw new InvalidOperationException(Strings.Exceptions.GenericLocationalProviderInvalidState);

				searcher = new GenericSearcher1d<TCollection, TAxis>(collection, axisMapping);
			}
		}

		public int DecreaseTickCount(int ticksCount)
		{
			return collection.Count;
		}

		public int IncreaseTickCount(int ticksCount)
		{
			return collection.Count;
		}

		public ITicksProvider<TAxis> MinorProvider
		{
			get { return null; }
		}

		public ITicksProvider<TAxis> MajorProvider
		{
			get { return null; }
		}

		public event EventHandler Changed;

		#endregion
	}
}
