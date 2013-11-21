using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class IListExtensions
	{
		public static void AddMany<T>(this IList<T> collection, IEnumerable<T> addingItems)
		{
			foreach (var item in addingItems)
			{
				collection.Add(item);
			}
		}

		public static void AddMany<T>(this IList<T> collection, params T[] children)
		{
			foreach (var child in children)
			{
				collection.Add(child);
			}
		}

		public static void RemoveAll<T>(this IList<T> collection, Type type)
		{
			var children = collection.Where(el => type.IsAssignableFrom(el.GetType())).ToArray();
			foreach (var child in children)
			{
				collection.Remove((T)child);
			}
		}

		public static void RemoveAll<T, TDelete>(this IList<T> collection)
		{
			var children = collection.OfType<TDelete>().ToArray();
			foreach (var child in children)
			{
				collection.Remove((T)(object)child);
			}
		}
	}
}
