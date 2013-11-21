using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Markers
{
	public class MarkerChart : PlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MarkerChart"/> class.
		/// </summary>
		public MarkerChart()
		{
			Viewport2D.SetIsContentBoundsHost(this, true);
		}

		#region DataSource

		private IEnumerable dataSource;
		/// <summary>
		/// Gets or sets the data source.
		/// Value can be null.
		/// </summary>
		/// <value>The data source.</value>
		public IEnumerable DataSource
		{
			get { return dataSource; }
			set
			{
				DetachDataSource();
				dataSource = value;
				AttachDataSource();

				OnDataSourceChanged();

				RebuildMarkers(true);
			}
		}

		protected virtual void OnDataSourceChanged() { }

		protected virtual void RebuildMarkers(bool shouldReleaseMarkers)
		{
			if (markerGenerator == null)
				return;

			if (shouldReleaseMarkers)
			{
				foreach (FrameworkElement item in itemsPanel.Children)
				{
					var enumerator = item.GetLocalValueEnumerator();
					while (enumerator.MoveNext())
					{
						item.ClearValue(enumerator.Current.Property);
					}

					descriptor.RemoveValueChanged(item, OnMarkerViewportBoundsChanged);
					markerGenerator.ReleaseMarker(item);
				}
			}

			itemsPanel.Children.Clear();

			if (dataSource == null)
				return;

			IndividualArrangePanel specialPanel = itemsPanel as IndividualArrangePanel;
			if (specialPanel != null)
				specialPanel.BeginBatchAdd();

			foreach (object item in dataSource)
			{
				var marker = CreateMarker(item);
				itemsPanel.Children.Add(marker);
			}

			if (specialPanel != null)
				specialPanel.EndBatchAdd();

			RecalculateViewportBounds();
		}

		private FrameworkElement CreateMarker(object item)
		{
			var marker = markerGenerator.CreateMarker(item);
			if (marker != null)
			{
				marker.DataContext = item;
				AttachViewportChangedListener(marker);

				BindMarkerEventArgs bindArgs = new BindMarkerEventArgs { Data = item, Marker = marker };

				OnMarkerBind(bindArgs);

				if (markerBindCallback != null)
				{
					markerBindCallback(bindArgs);
				}
			}

			return marker;
		}

		protected virtual void OnMarkerBind(BindMarkerEventArgs e) { }

		private Action<BindMarkerEventArgs> markerBindCallback = null;
		public Action<BindMarkerEventArgs> MarkerBindCallback
		{
			get { return markerBindCallback; }
			set
			{
				if (markerBindCallback != value)
				{
					markerBindCallback = value;
					RebuildMarkers(false);
				}
			}
		}

		private void AttachDataSource()
		{
			INotifyCollectionChanged notifyingCollection = dataSource as INotifyCollectionChanged;
			if (notifyingCollection != null)
			{
				notifyingCollection.CollectionChanged += OnDataSourceCollectionChanged;
			}
		}

		private void OnDataSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				int index = e.OldStartingIndex;
				foreach (object item in e.OldItems)
				{
					var oldMarker = itemsPanel.Children[index];

					if (markerGenerator != null)
					{
						FrameworkElement element = (FrameworkElement)oldMarker;
						// todo call cleanup callback here
						markerGenerator.ReleaseMarker(element);
						descriptor.RemoveValueChanged(element, OnMarkerViewportBoundsChanged);
					}

					itemsPanel.Children.RemoveAt(index);
				}
			}

			if (e.NewItems != null)
			{
				int index = e.NewStartingIndex;
				foreach (object item in e.NewItems)
				{
					var marker = CreateMarker(item);
					itemsPanel.Children.Insert(index, marker);
					index++;
				}
			}

			RecalculateViewportBounds();
		}

		DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(ViewportPanel.ActualViewportBoundsProperty, typeof(FrameworkElement));

		private void AttachViewportChangedListener(FrameworkElement element)
		{
			// todo or use this, or remove this

			//descriptor.RemoveValueChanged(element, OnMarkerViewportBoundsChanged);
			//descriptor.AddValueChanged(element, OnMarkerViewportBoundsChanged);
		}

		private Rect viewportBounds = Rect.Empty;

		private void OnMarkerViewportBoundsChanged(object sender, EventArgs e)
		{
			RecalculateViewportBounds();
			FrameworkElement element = (FrameworkElement)sender;
			DataRect elementBounds = ViewportPanel.GetActualViewportBounds(element);
			DataRect prevElementBounds = ViewportPanel.GetPrevActualViewportBounds(element);
		}

		private void RecalculateViewportBounds()
		{
			DataRect bounds = DataRect.Empty;

			foreach (UIElement item in itemsPanel.Children)
			{
				DataRect elementBounds = ViewportPanel.GetActualViewportBounds(item);
				bounds.Union(elementBounds);
			}

			Viewport2D.SetContentBounds(this, bounds);
		}

		private void DetachDataSource()
		{
			INotifyCollectionChanged notifyingCollection = dataSource as INotifyCollectionChanged;
			if (notifyingCollection != null)
			{
				notifyingCollection.CollectionChanged -= OnDataSourceCollectionChanged;
			}
		}

		#endregion

		#region Marker

		private OldMarkerGenerator markerGenerator;
		public OldMarkerGenerator MarkerGenerator
		{
			get { return markerGenerator; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (markerGenerator != value)
				{
					DetachMarkerGenerator();
					markerGenerator = value;
					AttachMarkerGenerator();

					RebuildMarkers(false);
				}
			}
		}

		private void AttachMarkerGenerator()
		{
			markerGenerator.Changed += OnMarkerChanged;
		}

		private void OnMarkerChanged(object sender, EventArgs e)
		{
			RebuildMarkers(false);
		}

		private void DetachMarkerGenerator()
		{
			if (markerGenerator != null)
			{
				markerGenerator.Changed -= OnMarkerChanged;
			}
		}

		#endregion

		#region Plotter attaching

		private ViewportPanel itemsPanel = new ViewportPanel();
		protected Panel ItemsPanel
		{
			get { return itemsPanel; }
		}

		private Plotter2D plotter;
		protected override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			this.plotter = (Plotter2D)plotter;
			((IPlotterElement)itemsPanel).OnPlotterAttached(plotter);
		}

		protected override void OnPlotterDetaching(Plotter plotter)
		{
			((IPlotterElement)itemsPanel).OnPlotterDetaching(plotter);

			this.plotter = null;

			base.OnPlotterDetaching(plotter);
		}

		#endregion
	}
}
