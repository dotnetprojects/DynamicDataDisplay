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
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
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
