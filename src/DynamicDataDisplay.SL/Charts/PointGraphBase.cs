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
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace Microsoft.Research.DynamicDataDisplay
{
    public abstract class PointsGraphBase : ViewportElement2D, IOneDimensionalChart
    {

        #region DataSource

        public IPointDataSource DataSource
        {
            get { return (IPointDataSource)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(
              "DataSource",
              typeof(IPointDataSource),
              typeof(PointsGraphBase),
              new PropertyMetadata(null,OnDataSourceChangedCallback)
              // Affects Render
            );

        private static void OnDataSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointsGraphBase graph = (PointsGraphBase)d;
            if (e.NewValue != e.OldValue)
            {
                graph.DetachDataSource(e.OldValue as IPointDataSource);
                graph.AttachDataSource(e.NewValue as IPointDataSource);
            }
            graph.OnDataSourceChanged(e);
        }

        private void AttachDataSource(IPointDataSource source)
        {
            if (source != null)
            {
                source.DataChanged += OnDataChanged;
            }
        }

        private void DetachDataSource(IPointDataSource source)
        {
            if (source != null)
            {
                source.DataChanged -= OnDataChanged;
            }
        }

        private void OnDataChanged(object sender, EventArgs e)
        {
            OnDataChanged();
        }

        protected virtual void OnDataChanged()
        {
            UpdateBounds(DataSource);

            RaiseDataChanged();
            UpdateCore();
        }

        private void RaiseDataChanged()
        {
            if (DataChanged != null)
            {
                DataChanged(this, EventArgs.Empty);
            }
        }
        public event EventHandler DataChanged;

        protected virtual void OnDataSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            IPointDataSource newDataSource = (IPointDataSource)args.NewValue;
            if (newDataSource != null)
            {
                UpdateBounds(newDataSource);
            }

            UpdateCore();
        }

        public override void OnPlotterAttached(Plotter plotter)
        {
            base.OnPlotterAttached(plotter);
            UpdateBounds(DataSource);
        }

        private void UpdateBounds(IPointDataSource dataSource)
        {
            if (Plotter2D != null)
            {
                var transform = GetTransform();
                Rect bounds = BoundsHelper.GetViewportBounds(dataSource.GetPoints(), transform.DataTransform);
                ContentBounds = bounds;
            }
        }

        #endregion

        #region DataTransform

        private DataTransform dataTransform = null;
        public DataTransform DataTransform
        {
            get { return dataTransform; }
            set
            {
                if (dataTransform != value)
                {
                    dataTransform = value;
                    UpdateCore();
                }
            }
        }

        protected CoordinateTransform GetTransform()
        {
            if (Plotter == null)
                return null;

            var transform = Plotter2D.Viewport.Transform;
            if (dataTransform != null)
                transform = transform.WithDataTransform(dataTransform);

            return transform;
        }

        #endregion

        protected IEnumerable<Point> GetPoints()
        {
            return DataSource.GetPoints(GetContext());
        }

        private readonly Context context = new Context();
        protected DependencyObject GetContext()
        {
            context.ClearValues();

            context.VisibleRect = Plotter2D.Viewport.Visible;
            context.Output = Plotter2D.Viewport.Output;

            return context;
        }
    }
}
