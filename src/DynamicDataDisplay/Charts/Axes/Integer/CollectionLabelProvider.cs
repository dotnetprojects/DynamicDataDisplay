using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
	public class CollectionLabelProvider<T> : LabelProviderBase<int>
	{
		private IList<T> collection;

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public IList<T> Collection
		{
			get { return collection; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (collection != value)
				{
					DetachCollection();

					collection = value;

					AttachCollection();

					RaiseChanged();
				}
			}
		}

		#region Collection changed

		private void AttachCollection()
		{
			INotifyCollectionChanged observableCollection = collection as INotifyCollectionChanged;
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged += OnCollectionChanged;
			}
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaiseChanged();
		}

		private void DetachCollection()
		{
			INotifyCollectionChanged observableCollection = collection as INotifyCollectionChanged;
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged -= OnCollectionChanged;
			}
		} 

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionLabelProvider&lt;T&gt;"/> class with empty labels collection.
		/// </summary>
		public CollectionLabelProvider() { }

		public CollectionLabelProvider(IList<T> collection)
			: this()
		{
			Collection = collection;
		}

		public override UIElement[] CreateLabels(ITicksInfo<int> ticksInfo)
		{
			var ticks = ticksInfo.Ticks;

			UIElement[] res = new UIElement[ticks.Length];

			var tickInfo = new LabelTickInfo<int> { Info = ticksInfo.Info };

			for (int i = 0; i < res.Length; i++)
			{
				int tick = ticks[i];
				tickInfo.Tick = tick;

				if (0 <= tick && tick < collection.Count)
				{
					string text = collection[tick].ToString();
					res[i] = new TextBlock
					{
						Text = text,
						ToolTip = text
					};
				}
				else
				{
					res[i] = null;
				}
			}
			return res;
		}
	}
}
