using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common.UndoSystem
{
	public sealed class CollectionAddAction<T> : UndoAction
	{
		private readonly ICollection<T> collection;
		private readonly T item;

		public CollectionAddAction(ICollection<T> collection, T item)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (item == null)
				throw new ArgumentNullException("addedItem");

			this.collection = collection;
			this.item = item;
		}

		public override void Do()
		{
			collection.Add(item);
		}

		public override void Undo()
		{
			collection.Remove(item);
		}
	}
}
