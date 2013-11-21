using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Filters;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Collections.Generic;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>Control for plotting 2d images</summary>
	public class Plotter2D : Plotter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plotter2D"/> class.
		/// </summary>
		public Plotter2D()
			: base(PlotterLoadMode.Normal)
		{
			InitViewport();
		}

		private void InitViewport()
		{
			viewportPanel = new Canvas();
			Grid.SetColumn(viewportPanel, 1);
			Grid.SetRow(viewportPanel, 1);

			viewport = new Viewport2D(viewportPanel, this);
			if (LoadMode != PlotterLoadMode.Empty)
			{
				MainGrid.Children.Add(viewportPanel);
			}
		}

		protected Plotter2D(PlotterLoadMode loadMode)
			: base(loadMode)
		{
			if (loadMode != PlotterLoadMode.Empty)
			{
				InitViewport();
			}
		}

		private Panel viewportPanel;
		/// <summary>
		/// Gets or sets the panel, which contains viewport.
		/// </summary>
		/// <value>The viewport panel.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel ViewportPanel
		{
			get { return viewportPanel; }
			protected set { viewportPanel = value; }
		}

		private Viewport2D viewport;
		private Viewport2dDeferredPanningProxy deferredPanningProxy;

		/// <summary>
		/// Gets the viewport.
		/// </summary>
		/// <value>The viewport.</value>
		[NotNull]
		public Viewport2D Viewport
		{
			get
			{
				bool useDeferredPanning = false;
				if (CurrentChild != null)
				{
					DependencyObject dependencyChild = CurrentChild as DependencyObject;
					if (dependencyChild != null)
					{
						useDeferredPanning = Viewport2D.GetUseDeferredPanning(dependencyChild);
					}
				}

				if (useDeferredPanning)
				{
					if (deferredPanningProxy == null)
					{
						deferredPanningProxy = new Viewport2dDeferredPanningProxy(viewport);
					}
					return deferredPanningProxy;
				}

				return viewport;
			}
			protected set { viewport = value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double ViewportClipToBoundsEnlargeFactor
		{
			get { return viewport.ClipToBoundsEnlargeFactor; }
			set { viewport.ClipToBoundsEnlargeFactor = value; }
		}

		/// <summary>
		/// Gets or sets the data transform.
		/// </summary>
		/// <value>The data transform.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTransform DataTransform
		{
			get { return viewport.Transform.DataTransform; }
			set { viewport.Transform = viewport.Transform.WithDataTransform(value); }
		}

		/// <summary>
		/// Gets or sets the transform.
		/// </summary>
		/// <value>The transform.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CoordinateTransform Transform
		{
			get { return viewport.Transform; }
			set { viewport.Transform = value; }
		}

		/// <summary>
		/// Fits to view.
		/// </summary>
		public void FitToView()
		{
			viewport.FitToView();
		}

		/// <summary>
		/// Gets or sets the visible area rectangle.
		/// </summary>
		/// <value>The visible.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataRect Visible
		{
			get { return viewport.Visible; }
			set { viewport.Visible = value; }
		}

		/// <summary>
		/// Gets or sets the domain - maximal value of visible area.
		/// </summary>
		/// <value>The domain.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataRect Domain
		{
			get { return viewport.Domain; }
			set { viewport.Domain = value; }
		}

		/// <summary>
		/// Gets the restrictions being applied to viewport.
		/// </summary>
		/// <value>The restrictions.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RestrictionCollection Restrictions
		{
			get { return viewport.Restrictions; }
		}

		/// <summary>
		/// Gets the fit to view restrictions being applied to viewport in 'fit to view' state.
		/// </summary>
		/// <value>The fit to view restrictions.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RestrictionCollection FitToViewRestrictions
		{
			get { return viewport.FitToViewRestrictions; }
		}
	}
}