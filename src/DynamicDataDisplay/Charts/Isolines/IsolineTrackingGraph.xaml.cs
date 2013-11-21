using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts.Isolines;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Draws one isoline line through mouse position.
	/// </summary>
	public partial class IsolineTrackingGraph : IsolineGraphBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IsolineTrackingGraph"/> class.
		/// </summary>
		public IsolineTrackingGraph()
		{
			InitializeComponent();
		}

		private Style pathStyle = null;
		/// <summary>
		/// Gets or sets style, applied to line path.
		/// </summary>
		/// <value>The path style.</value>
		public Style PathStyle
		{
			get { return pathStyle; }
			set
			{
				pathStyle = value;
				foreach (var path in addedPaths)
				{
					path.Style = pathStyle;
				}
			}
		}

		Point prevMousePos;

		protected override void OnPlotterAttached()
		{
			UIElement parent = (UIElement)Parent;
			parent.MouseMove += parent_MouseMove;

			UpdateUIRepresentation();
		}

		protected override void OnPlotterDetaching()
		{
			UIElement parent = (UIElement)Parent;
			parent.MouseMove -= parent_MouseMove;
		}

		private void parent_MouseMove(object sender, MouseEventArgs e)
		{
			Point mousePos = e.GetPosition(this);
			if (mousePos != prevMousePos)
			{
				prevMousePos = mousePos;

				UpdateUIRepresentation();
			}
		}

		protected override void UpdateDataSource()
		{
			IsolineBuilder.DataSource = DataSource;

			UpdateUIRepresentation();
		}

		protected override void OnViewportPropertyChanged(ExtendedPropertyChangedEventArgs e)
		{
			UpdateUIRepresentation();
		}

		private readonly List<Path> addedPaths = new List<Path>();
		private Vector labelShift = new Vector(3, 3);
		private void UpdateUIRepresentation()
		{
			if (Plotter2D == null)
				return;

			foreach (var path in addedPaths)
			{
				content.Children.Remove(path);
			}
			addedPaths.Clear();

			if (DataSource == null)
			{
				labelGrid.Visibility = Visibility.Hidden;
				return;
			}

			Rect output = Plotter2D.Viewport.Output;

			Point mousePos = Mouse.GetPosition(this);
			if (!output.Contains(mousePos)) return;

			var transform = Plotter2D.Viewport.Transform;
			Point visiblePoint = mousePos.ScreenToData(transform);
			DataRect visible = Plotter2D.Viewport.Visible;

			double isolineLevel;
			if (Search(visiblePoint, out isolineLevel))
			{
				var collection = IsolineBuilder.BuildIsoline(isolineLevel);

				string format = "G3";
				if (Math.Abs(isolineLevel) < 1000)
					format = "F";

				textBlock.Text = isolineLevel.ToString(format);

				double x = mousePos.X + labelShift.X;
				if (x + labelGrid.ActualWidth > output.Right)
					x = mousePos.X - labelShift.X - labelGrid.ActualWidth;
				double y = mousePos.Y - labelShift.Y - labelGrid.ActualHeight;
				if (y < output.Top)
					y = mousePos.Y + labelShift.Y;

				Canvas.SetLeft(labelGrid, x);
				Canvas.SetTop(labelGrid, y);

				foreach (LevelLine segment in collection.Lines)
				{
					StreamGeometry streamGeom = new StreamGeometry();
					using (StreamGeometryContext context = streamGeom.Open())
					{
						Point startPoint = segment.StartPoint.DataToScreen(transform);
						var otherPoints = segment.OtherPoints.DataToScreenAsList(transform);
						context.BeginFigure(startPoint, false, false);
						context.PolyLineTo(otherPoints, true, true);
					}

					Path path = new Path
					{
						Stroke = new SolidColorBrush(Palette.GetColor(segment.Value01)),
						Data = streamGeom,
						Style = pathStyle
					};
					content.Children.Add(path);
					addedPaths.Add(path);

					labelGrid.Visibility = Visibility.Visible;

					Binding pathBinding = new Binding { Path = new PropertyPath("StrokeThickness"), Source = this };
					path.SetBinding(Path.StrokeThicknessProperty, pathBinding);
				}
			}
			else
			{
				labelGrid.Visibility = Visibility.Hidden;
			}
		}

		int foundI = 0;
		int foundJ = 0;
		Quad foundQuad = null;
		private bool Search(Point pt, out double foundVal)
		{
			var grid = DataSource.Grid;

			foundVal = 0;

			int width = DataSource.Width;
			int height = DataSource.Height;
			bool found = false;
			int i = 0, j = 0;
			for (i = 0; i < width - 1; i++)
			{
				for (j = 0; j < height - 1; j++)
				{
					Quad quad = new Quad(
					grid[i, j],
					grid[i, j + 1],
					grid[i + 1, j + 1],
					grid[i + 1, j]);
					if (quad.Contains(pt))
					{
						found = true;
						foundQuad = quad;
						foundI = i;
						foundJ = j;

						break;
					}
				}
				if (found) break;
			}
			if (!found)
			{
				foundQuad = null;
				return false;
			}

			var data = DataSource.Data;

			double x = pt.X;
			double y = pt.Y;
			Vector A = grid[i, j + 1].ToVector();					// @TODO: in common case add a sorting of points:
			Vector B = grid[i + 1, j + 1].ToVector();				//   maxA ___K___ B
			Vector C = grid[i + 1, j].ToVector();					//      |         |
			Vector D = grid[i, j].ToVector();						//      M    P    N
			double a = data[i, j + 1];						//		|         |
			double b = data[i + 1, j + 1];					//		В ___L____Сmin
			double c = data[i + 1, j];
			double d = data[i, j];

			Vector K, L;
			double k, l;
			if (x >= A.X)
				k = Interpolate(A, B, a, b, K = new Vector(x, GetY(A, B, x)));
			else
				k = Interpolate(D, A, d, a, K = new Vector(x, GetY(D, A, x)));

			if (x >= C.X)
				l = Interpolate(C, B, c, b, L = new Vector(x, GetY(C, B, x)));
			else
				l = Interpolate(D, C, d, c, L = new Vector(x, GetY(D, C, x)));

			foundVal = Interpolate(L, K, l, k, new Vector(x, y));
			return !Double.IsNaN(foundVal);
		}

		private double Interpolate(Vector v0, Vector v1, double u0, double u1, Vector a)
		{
			Vector l1 = a - v0;
			Vector l = v1 - v0;

			double res = (u1 - u0) / l.Length * l1.Length + u0;
			return res;
		}

		private double GetY(Vector v0, Vector v1, double x)
		{
			double res = v0.Y + (v1.Y - v0.Y) / (v1.X - v0.X) * (x - v0.X);
			return res;
		}
	}
}
