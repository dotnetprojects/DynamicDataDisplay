using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common.DataSearch
{
	internal sealed class GenericSearcher1d<TCollection, TMember> where TMember : IComparable<TMember>
	{
		private readonly Func<TCollection, TMember> selector;
		private readonly IList<TCollection> collection;
		public GenericSearcher1d(IList<TCollection> collection, Func<TCollection, TMember> selector)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (selector == null)
				throw new ArgumentNullException("selector");

			this.collection = collection;
			this.selector = selector;
		}

		public SearchResult1d SearchBetween(TMember x)
		{
			return SearchBetween(x, SearchResult1d.Empty);
		}

		public SearchResult1d SearchBetween(TMember x, SearchResult1d result)
		{
			if (collection.Count == 0)
				return SearchResult1d.Empty;

			int lastIndex = collection.Count - 1;

			if (x.CompareTo(selector(collection[0])) < 0)
				return SearchResult1d.Empty;
			else if (selector(collection[lastIndex]).CompareTo(x) < 0)
				return SearchResult1d.Empty;

			int startIndex = !result.IsEmpty ? Math.Min(result.Index, lastIndex) : 0;

			// searching ascending
			if (selector(collection[startIndex]).CompareTo(x) < 0)
			{
				for (int i = startIndex + 1; i <= lastIndex; i++)
					if (selector(collection[i]).CompareTo(x) >= 0)
						return new SearchResult1d { Index = i - 1 };
			}
			else // searching descending
			{
				for (int i = startIndex - 1; i >= 0; i--)
					if (selector(collection[i]).CompareTo(x) <= 0)
						return new SearchResult1d { Index = i };
			}

			throw new InvalidOperationException("Should not appear here.");
		}

		public SearchResult1d SearchFirstLess(TMember x)
		{
			if (collection.Count == 0)
				return SearchResult1d.Empty;

			SearchResult1d result = SearchResult1d.Empty;
			for (int i = 0; i < collection.Count; i++)
			{
				if (selector(collection[i]).CompareTo(x) >= 0)
				{
					result.Index = i;
					break;
				}
			}

			return result;
		}

		public SearchResult1d SearchGreater(TMember x)
		{
			if (collection.Count == 0)
				return SearchResult1d.Empty;

			SearchResult1d result = SearchResult1d.Empty;
			for (int i = collection.Count - 1; i >= 0; i--)
			{
				if (selector(collection[i]).CompareTo(x) <= 0)
				{
					result.Index = i;
					break;
				}
			}

			return result;
		}
	}
}
