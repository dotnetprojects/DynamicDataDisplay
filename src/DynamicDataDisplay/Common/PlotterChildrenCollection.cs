using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Collections;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	/// <summary>
	/// Contains all charts added to ChartPlotter.
	/// </summary>
	[ContentWrapper(typeof(ViewportUIContainer))]
	public sealed class PlotterChildrenCollection : D3Collection<IPlotterElement>, IList
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PlotterChildrenCollection"/> class.
		/// </summary>
		internal PlotterChildrenCollection(Plotter plotter)
		{
			if (plotter == null)
				throw new ArgumentNullException("plotter");

			this.plotter = plotter;
		}

		private readonly Plotter plotter;
		public Plotter Plotter
		{
			get { return plotter; }
		}

		/// <summary>
		/// Called before item added to collection. Enables to perform validation.
		/// </summary>
		/// <param name="item">The adding item.</param>
		protected override void OnItemAdding(IPlotterElement item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
		}

		/// <summary>
		/// This override enables notifying about removing each element, instead of
		/// notifying about collection reset.
		/// </summary>
		protected override void ClearItems()
		{
			var items = new List<IPlotterElement>(base.Items);
			foreach (var item in items)
			{
				Remove(item);
			}
		}

		#region Foreign content

		public void Add(FrameworkElement content)
		{
			if (content == null)
				throw new ArgumentNullException("content");

			IPlotterElement plotterElement = content as IPlotterElement;
			if (plotterElement != null)
			{
				Add(plotterElement);
			}
			else
			{
				ViewportUIContainer container = new ViewportUIContainer(content);
				Add(container);
			}
		}

		#endregion // end of Foreign content

		#region IList Members

		int IList.Add(object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");


			FrameworkElement content = value as FrameworkElement;
			if (content != null)
			{
				Add(content);

				return 0;
			}

			IPlotterElement element = value as IPlotterElement;
			if (element != null)
			{
				Add(element);

				return 0;
			}

			throw new ArgumentException(String.Format("Children of type '{0}' are not supported.", value.GetType()));
		}

		void IList.Clear()
		{
			Clear();
		}

		bool IList.Contains(object value)
		{
			IPlotterElement element = value as IPlotterElement;
			return element != null && Contains(element);
		}

		int IList.IndexOf(object value)
		{
			IPlotterElement element = value as IPlotterElement;
			if (element != null)
				return IndexOf(element);
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			IPlotterElement element = value as IPlotterElement;
			if (element != null)
			{
				Insert(index, element);
			}
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool IList.IsReadOnly
		{
			get { return false; }
		}

		void IList.Remove(object value)
		{
			IPlotterElement element = value as IPlotterElement;
			if (element != null)
				Remove(element);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				IPlotterElement element = value as IPlotterElement;
				if (element != null)
					this[index] = element;
			}
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index)
		{
			IPlotterElement[] elements = array as IPlotterElement[];
			if (elements != null)
				CopyTo(elements, index);
		}

		int ICollection.Count
		{
			get { return Count; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return null; }
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
