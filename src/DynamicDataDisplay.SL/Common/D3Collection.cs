using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Microsoft.Research.DynamicDataDisplay
{
    /// <summary>
    /// This is a base class for some DynamicDataDisplay collections.
    /// It provides means to be notified about item adding and added events, which enables successors to, for example,
    /// check if adding item is not equal to null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class D3Collection<T> : ObservableCollection<T>
    {
        protected override void InsertItem(int index, T item)
        {
            OnItemAdding(item);
            base.InsertItem(index, item);
            OnItemAdded(item);
        }

        protected override void ClearItems()
        {
            foreach (var item in Items)
            {
                OnItemRemoving(item);
            }
            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            T item = Items[index];
            OnItemRemoving(item);

            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = Items[index];
            OnItemRemoving(oldItem);

            OnItemAdding(item);
            base.SetItem(index, item);
            OnItemAdded(item);
        }

        /// <summary>
        /// Called before item added to collection. Enables to perform validation.
        /// </summary>
        /// <param name="item">The adding item.</param>
        protected virtual void OnItemAdding(T item) { }

        /// <summary>
        /// Called when item is added.
        /// </summary>
        /// <param name="item">The added item.</param>
        protected virtual void OnItemAdded(T item) { }
        /// <summary>
        /// Called when item is being removed, but before it is actually removed.
        /// </summary>
        /// <param name="item">The removing item.</param>
        protected virtual void OnItemRemoving(T item) { }
    }
}
