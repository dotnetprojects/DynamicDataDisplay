using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	internal static class ObservableCollectionHelper
	{
		public static void ApplyChanges<T>(this ObservableCollection<T> collection, NotifyCollectionChangedEventArgs args)
		{
			if (args.NewItems != null)
			{
				int startingIndex = args.NewStartingIndex;
				var newItems = args.NewItems;

				for (int i = 0; i < newItems.Count; i++)
				{
					T addedItem = (T)newItems[i];
					collection.Insert(startingIndex + i, addedItem);
				}
			}
			if (args.OldItems != null)
			{
				for (int i = 0; i < args.OldItems.Count; i++)
				{
					T removedItem = (T)args.OldItems[i];
					collection.Remove(removedItem);
				}
			}
		}
	}
}
