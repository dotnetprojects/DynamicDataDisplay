using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Filters;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Collections.Specialized;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents a collection of point filters of <see cref="LineGraph"/>.
	/// </summary>
	public sealed class FilterCollection : D3Collection<IPointsFilter>
	{
		protected override void OnItemAdding(IPointsFilter item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
		}

		protected override void OnItemAdded(IPointsFilter item)
		{
			item.Changed += OnItemChanged;
		}

		private void OnItemChanged(object sender, EventArgs e)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected override void OnItemRemoving(IPointsFilter item)
		{
			item.Changed -= OnItemChanged;
		}

		internal List<Point> Filter(List<Point> points, Rect screenRect)
		{
			foreach (var filter in Items)
			{
				filter.SetScreenRect(screenRect);
				points = filter.Filter(points);
			}

			return points;
		}
	}
}
