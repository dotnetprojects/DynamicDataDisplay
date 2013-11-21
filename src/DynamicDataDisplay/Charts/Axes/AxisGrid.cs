using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Controls;
using System;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;


namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Draws grid over viewport. Number of 
	/// grid lines depends on Plotter's MainHorizontalAxis and MainVerticalAxis ticks.
	/// </summary>
	public class AxisGrid : ContentControl, IPlotterElement
	{
		static AxisGrid()
		{
			Type thisType = typeof(AxisGrid);
			Panel.ZIndexProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(-1));
		}

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		internal void BeginTicksUpdate()
		{
		}
		internal void EndTicksUpdate()
		{
			UpdateUIRepresentation();
		}

		protected internal MinorTickInfo<double>[] MinorHorizontalTicks { get; set; }

		protected internal MinorTickInfo<double>[] MinorVerticalTicks { get; set; }

		protected internal double[] HorizontalTicks { get; set; }

		protected internal double[] VerticalTicks { get; set; }


		private bool drawVerticalTicks = true;
		/// <summary>
		/// Gets or sets a value indicating whether to draw vertical tick lines.
		/// </summary>
		/// <value><c>true</c> if draw vertical ticks; otherwise, <c>false</c>.</value>
		public bool DrawVerticalTicks
		{
			get { return drawVerticalTicks; }
			set
			{
				if (drawVerticalTicks != value)
				{
					drawVerticalTicks = value;
					UpdateUIRepresentation();
				}
			}
		}

		private bool drawHorizontalTicks = true;
		/// <summary>
		/// Gets or sets a value indicating whether to draw horizontal tick lines.
		/// </summary>
		/// <value><c>true</c> if draw horizontal ticks; otherwise, <c>false</c>.</value>
		public bool DrawHorizontalTicks
		{
			get { return drawHorizontalTicks; }
			set
			{
				if (drawHorizontalTicks != value)
				{
					drawHorizontalTicks = value;
					UpdateUIRepresentation();
				}
			}
		}

		private bool drawHorizontalMinorTicks = false;
		/// <summary>
		/// Gets or sets a value indicating whether to draw horizontal minor ticks.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if draw horizontal minor ticks; otherwise, <c>false</c>.
		/// </value>
		public bool DrawHorizontalMinorTicks
		{
			get { return drawHorizontalMinorTicks; }
			set
			{
				if (drawHorizontalMinorTicks != value)
				{
					drawHorizontalMinorTicks = value;
					UpdateUIRepresentation();
				}
			}
		}

		private bool drawVerticalMinorTicks = false;
		/// <summary>
		/// Gets or sets a value indicating whether to draw vertical minor ticks.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if draw vertical minor ticks; otherwise, <c>false</c>.
		/// </value>
		public bool DrawVerticalMinorTicks
		{
			get { return drawVerticalMinorTicks; }
			set
			{
				if (drawVerticalMinorTicks != value)
				{
					drawVerticalMinorTicks = value;
					UpdateUIRepresentation();
				}
			}
		}

		private double gridBrushThickness = 1;

		private Path path = new Path();
		private Canvas canvas = new Canvas();
		/// <summary>
		/// Initializes a new instance of the <see cref="AxisGrid"/> class.
		/// </summary>
		public AxisGrid()
		{
			IsHitTestVisible = false;

			canvas.ClipToBounds = true;

			path.Stroke = Brushes.LightGray;
			path.StrokeThickness = gridBrushThickness;

			Content = canvas;
		}

		private readonly ResourcePool<LineGeometry> lineGeometryPool = new ResourcePool<LineGeometry>();
		private readonly ResourcePool<Line> linePool = new ResourcePool<Line>();

		private void UpdateUIRepresentation()
		{
			foreach (UIElement item in canvas.Children)
			{
				Line line = item as Line;
				if (line != null)
				{
					linePool.Put(line);
				}
			}

			canvas.Children.Clear();
			Size size = RenderSize;

			DrawMinorHorizontalTicks();
			DrawMinorVerticalTicks();

			GeometryGroup prevGroup = path.Data as GeometryGroup;
			if (prevGroup != null)
			{
				foreach (LineGeometry geometry in prevGroup.Children)
				{
					lineGeometryPool.Put(geometry);
				}
			}

			GeometryGroup group = new GeometryGroup();
			if (HorizontalTicks != null && drawHorizontalTicks)
			{
				double minY = 0;
				double maxY = size.Height;

				for (int i = 0; i < HorizontalTicks.Length; i++)
				{
					double screenX = HorizontalTicks[i];
					LineGeometry line = lineGeometryPool.GetOrCreate();
					line.StartPoint = new Point(screenX, minY);
					line.EndPoint = new Point(screenX, maxY);
					group.Children.Add(line);
				}
			}

			if (VerticalTicks != null && drawVerticalTicks)
			{
				double minX = 0;
				double maxX = size.Width;

				for (int i = 0; i < VerticalTicks.Length; i++)
				{
					double screenY = VerticalTicks[i];
					LineGeometry line = lineGeometryPool.GetOrCreate();
					line.StartPoint = new Point(minX, screenY);
					line.EndPoint = new Point(maxX, screenY);
					group.Children.Add(line);
				}
			}

			canvas.Children.Add(path);
			path.Data = group;
		}

		private void DrawMinorVerticalTicks()
		{
			Size size = RenderSize;
			if (MinorVerticalTicks != null && drawVerticalMinorTicks)
			{
				double minX = 0;
				double maxX = size.Width;

				for (int i = 0; i < MinorVerticalTicks.Length; i++)
				{
					double screenY = MinorVerticalTicks[i].Tick;
					if (screenY < 0)
						continue;
					if (screenY > size.Height)
						continue;

					Line line = linePool.GetOrCreate();

					line.Y1 = screenY;
					line.Y2 = screenY;
					line.X1 = minX;
					line.X2 = maxX;
					line.Stroke = Brushes.LightGray;
					line.StrokeThickness = MinorVerticalTicks[i].Value * gridBrushThickness;

					canvas.Children.Add(line);
				}
			}
		}

		private void DrawMinorHorizontalTicks()
		{
			Size size = RenderSize;
			if (MinorHorizontalTicks != null && drawHorizontalMinorTicks)
			{
				double minY = 0;
				double maxY = size.Height;

				for (int i = 0; i < MinorHorizontalTicks.Length; i++)
				{
					double screenX = MinorHorizontalTicks[i].Tick;
					if (screenX < 0)
						continue;
					if (screenX > size.Width)
						continue;

					Line line = linePool.GetOrCreate();
					line.X1 = screenX;
					line.X2 = screenX;
					line.Y1 = minY;
					line.Y2 = maxY;
					line.Stroke = Brushes.LightGray;
					line.StrokeThickness = MinorHorizontalTicks[i].Value * gridBrushThickness;

					canvas.Children.Add(line);
				}
			}
		}

		#region IPlotterElement Members

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;
			plotter.CentralGrid.Children.Add(this);
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			plotter.CentralGrid.Children.Remove(this);
			this.plotter = null;
		}

		private Plotter plotter;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}