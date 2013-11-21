using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	public class RingArray<T> : INotifyCollectionChanged, IList<T>
	{
		public RingArray(int capacity)
		{
			this.capacity = capacity;
			array = new T[capacity];
		}

		public void Add(T item)
		{
			int index = (startIndex + count) % capacity;
			if (startIndex + count >= capacity)
			{
				startIndex++;
			}
			else
			{
				count++;
			}

			array[index] = item;

			CollectionChanged.Raise(this);
		}

		public T this[int index]
		{
			get { return array[(startIndex + index) % capacity]; }
			set
			{
				array[(startIndex + index) % capacity] = value;
				CollectionChanged.Raise(this);
			}
		}

		public void Clear()
		{
			count = 0;
			startIndex = 0;
			array = new T[capacity];
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < count; i++)
			{
				yield return this[i];
			}
		}

		private int count;
		public int Count
		{
			get { return count; }
		}

		private T[] array;

		private int capacity;
		public int Capacity
		{
			get { return capacity; }
		}

		private int startIndex = 0;

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion

		#region IList<T> Members

		public int IndexOf(T item)
		{
			int index = Array.IndexOf(array, item);

			if (index == -1)
				return -1;

			return (index - startIndex + count) % capacity;
		}

		public void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICollection<T> Members

		public bool Contains(T item)
		{
			return Array.IndexOf(array, item) > -1;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
