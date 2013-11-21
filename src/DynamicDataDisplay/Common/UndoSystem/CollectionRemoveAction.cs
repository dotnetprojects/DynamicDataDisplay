using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common.UndoSystem
{
	public sealed class CollectionRemoveAction<T> : UndoAction
	{
		private readonly IList<T> collection;
		private readonly T item;
		private readonly int index;

		public CollectionRemoveAction(IList<T> collection, T item, int index)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (item == null)
				throw new ArgumentNullException("addedItem");

			this.collection = collection;
			this.item = item;
			this.index = index;
		}

		public override void Do()
		{
			collection.Remove(item);
		}

		public override void Undo()
		{
			collection.Insert(index, item);
		}
	}
}
