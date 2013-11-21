using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Markers
{
	public partial class DevMarkerChart
	{
		private Updater resetUpdater = new ResetUpdater();
		protected Updater SelfResetUpdater
		{
			get { return resetUpdater; }
			set { resetUpdater = value; }
		}

		private Updater addUpdater = new AddUpdater();
		protected Updater SelfAddUpdater
		{
			get { return addUpdater; }
			set { addUpdater = value; }
		}

		private Updater removeUpdater = new RemoveUpdater();
		protected Updater SelfRemoveUpdater
		{
			get { return removeUpdater; }
			set { removeUpdater = value; }
		}

		protected abstract class Updater
		{
			public abstract void Work(DevMarkerChart owner, NotifyCollectionChangedEventArgs e);
		}

		private sealed class ResetUpdater : Updater
		{
			public override void Work(DevMarkerChart owner, NotifyCollectionChangedEventArgs e)
			{
				//var markerGenerator = owner.markerGenerator;
				//if (markerGenerator != null)
				//{
				//    foreach (FrameworkElement marker in owner.CurrentItemsPanel.Children)
				//    {
				//        owner.RemoveCommonBindings(marker);
				//        markerGenerator.Release(marker);
				//    }
				//}

				//owner.CurrentItemsPanel.Children.Clear();
				owner.DrawAllMarkers(true);
			}
		}

		private sealed class AddUpdater : Updater
		{
			private DevMarkerChart owner;
			public override void Work(DevMarkerChart owner, NotifyCollectionChangedEventArgs e)
			{
				this.owner = owner;

				if (!owner.IsReadyToDrawMarkers()) return;

				int newStartingIndex = e.NewStartingIndex;
				if (newStartingIndex == owner.lastIndex)
				{
					AddNewItems(e);
				}
				else if (owner.startIndex <= newStartingIndex && newStartingIndex <= owner.lastIndex)
				{
					foreach (UIElement child in owner.CurrentItemsPanel.Children)
					{
						var index = GetIndex(child);
						if (index >= newStartingIndex)
						{
							SetIndex(child, index + 1);
						}
					}
					AddNewItems(e);
				}
			}

			private void AddNewItems(NotifyCollectionChangedEventArgs e)
			{
				int index = e.NewStartingIndex;
				foreach (var dataItem in e.NewItems)
				{
					owner.CreateAndAddMarker(dataItem, index);
					index++;
				}

				owner.lastIndex = index;
			}
		}

		private sealed class RemoveUpdater : Updater
		{
			public override void Work(DevMarkerChart owner, NotifyCollectionChangedEventArgs e)
			{
				int oldStartingIndex = e.OldStartingIndex;

				int oldEndIndex = oldStartingIndex + e.OldItems.Count;

				var children = new UIElement[owner.CurrentItemsPanel.Children.Count];
				owner.CurrentItemsPanel.Children.CopyTo(children, 0);
				foreach (UIElement item in children)
				{
					var index = GetIndex(item);
					if (oldStartingIndex <= index && index < oldEndIndex)
					{
						owner.CurrentItemsPanel.Children.Remove(item);
					}
				}

				int oldCount = e.OldItems.Count;
				foreach (UIElement item in owner.CurrentItemsPanel.Children)
				{
					var index = GetIndex(item);
					if (index >= oldEndIndex)
					{
						SetIndex(item, index - oldCount);
					}
				}
			}
		}
	}
}
