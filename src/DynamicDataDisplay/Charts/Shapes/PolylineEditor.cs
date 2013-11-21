using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	/// <summary>
	/// Represents an editor of points' position of ViewportPolyline or ViewportPolygon.
	/// </summary>
	[ContentProperty("Polyline")]
	public class PolylineEditor : IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PolylineEditor"/> class.
		/// </summary>
		public PolylineEditor() { }

		private ViewportPolylineBase polyline;
		/// <summary>
		/// Gets or sets the polyline, to edit points of which.
		/// </summary>
		/// <value>The polyline.</value>
		[NotNull]
		public ViewportPolylineBase Polyline
		{
			get { return polyline; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Polyline");

				if (polyline != value)
				{
					polyline = value;
					var descr = DependencyPropertyDescriptor.FromProperty(ViewportPolylineBase.PointsProperty, typeof(ViewportPolylineBase));
					descr.AddValueChanged(polyline, OnPointsReplaced);

					if (plotter != null)
					{
						AddLineToPlotter(false);
					}
				}
			}
		}

		bool pointsAdded = false;
		private void OnPointsReplaced(object sender, EventArgs e)
		{
			if (plotter == null)
				return;
			if (pointsAdded)
				return;

			ViewportPolylineBase line = (ViewportPolylineBase)sender;

			pointsAdded = true;
			List<IPlotterElement> draggablePoints = new List<IPlotterElement>();
			GetDraggablePoints(draggablePoints);

			foreach (var point in draggablePoints)
			{
				plotter.Children.Add(point);
			}
		}

		private void AddLineToPlotter(bool async)
		{
			if (!async)
			{
				foreach (var item in GetAllElementsToAdd())
				{
					plotter.Children.Add(item);
				}
			}
			else
			{
				plotter.Dispatcher.BeginInvoke(((Action)(() => { AddLineToPlotter(false); })), DispatcherPriority.Send);
			}
		}

		private List<IPlotterElement> GetAllElementsToAdd()
		{
			var result = new List<IPlotterElement>(1 + polyline.Points.Count);
			result.Add(polyline);

			GetDraggablePoints(result);

			return result;
		}

		private void GetDraggablePoints(List<IPlotterElement> collection)
		{
			for (int i = 0; i < polyline.Points.Count; i++)
			{
				DraggablePoint point = new DraggablePoint();
				point.SetBinding(DraggablePoint.PositionProperty, new Binding
				{
					Source = polyline,
					Path = new PropertyPath("Points[" + i + "]"),
					Mode = BindingMode.TwoWay
				});
				collection.Add(point);
			}
		}

		#region IPlotterElement Members

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;

			if (polyline != null)
			{
				AddLineToPlotter(true);
			}
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			this.plotter = null;
		}

		private Plotter2D plotter;
		/// <summary>
		/// Gets the parent plotter of chart.
		/// Should be equal to null if item is not connected to any plotter.
		/// </summary>
		/// <value>The plotter.</value>
		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
