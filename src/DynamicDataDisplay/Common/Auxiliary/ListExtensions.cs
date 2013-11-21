using System;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay
{
	internal static class ListExtensions
	{
		/// <summary>
		/// Gets last element of list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		internal static T GetLast<T>(this List<T> list)
		{
			if (list == null) throw new ArgumentNullException("list");
			if (list.Count == 0) throw new InvalidOperationException(Strings.Exceptions.CannotGetLastElement);

			return list[list.Count - 1];
		}

		internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			if (action == null)
				throw new ArgumentNullException("action");
			if (source == null)
				throw new ArgumentNullException("source");

			foreach (var item in source)
			{
				action(item);
			}
		}
	}
}
