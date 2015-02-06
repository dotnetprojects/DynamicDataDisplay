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

namespace Microsoft.Research.DynamicDataDisplay.Filters
{
    public abstract class PointsFilterBase : IPointsFilter
    {
        #region IPointsFilter Members

        public abstract List<Point> Filter(List<Point> points);

        public virtual void SetScreenRect(Rect screenRect) { }

        protected void RaiseChanged()
        {
            Changed.Raise(this);
        }
        public event EventHandler Changed;

        #endregion
    }
}
