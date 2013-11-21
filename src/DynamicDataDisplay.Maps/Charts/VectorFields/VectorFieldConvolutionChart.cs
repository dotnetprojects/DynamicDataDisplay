using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.INonUniformDataSource2D<System.Windows.Vector>;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields;
using Microsoft.Research.DynamicDataDisplay.Charts.Isolines;
using System.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts
{
	public class VectorFieldConvolutionChart : FrameworkElement, IPlotterElement
	{
		public VectorFieldConvolutionChart()
		{
			//Effect = effect;
		}

		#region Properties

		public DataSource DataSource
		{
			get { return (DataSource)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(DataSource),
		  typeof(VectorFieldConvolutionChart),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VectorFieldConvolutionChart owner = (VectorFieldConvolutionChart)d;
			owner.OnDataSourceReplaced((DataSource)e.OldValue, (DataSource)e.NewValue);
		}

		VectorFieldConvolutionEffect effect = new VectorFieldConvolutionEffect();
		BitmapSource whiteNoizeBmp;
		private void OnDataSourceReplaced(IDataSource2D<Vector> prevDataSource, IDataSource2D<Vector> currDataSource)
		{
			whiteNoizeBmp = CreateWhiteNoizeBmp(currDataSource.Width, currDataSource.Height);
		}

		private BitmapSource CreateWhiteNoizeBmp(int width, int height)
		{
			var dataSource = DataSource;
			Random rnd = new Random();
			int[] pixels = new int[width * height];
			for (int i = 0; i < width * height; i++)
			{
				HsbColor color = new HsbColor(0, 0, Math.Round(5 * rnd.NextDouble()) / 4);
				int argb = color.ToArgb();
				pixels[i] = argb;
			}
			//var whiteNoizeBmp = WriteableBitmap.Create(width, height, 96, 96, PixelFormats.Pbgra32, null, pixels, width * 4);

			var contentBounds = dataSource.Grid.GetGridBounds();
			Viewport2D.SetContentBounds(this, contentBounds);

			int[] effectivePixels = CreateConvolutionArray(width, height, pixels);

			var filter = new NormalizeFilter();
			filter.Filter(effectivePixels, width, height);

			var result = WriteableBitmap.Create(width, height, 96, 96, PixelFormats.Pbgra32, null, effectivePixels, width * 4);
			//ScreenshotHelper.SaveBitmapToFile(result, "1.png");

			return result;
		}

		private int[] CreateConvolutionArray(int width, int height, int[] pixels)
		{
			var dataSource = DataSource;

			const int L = 20;

			int[] effectivePixels = new int[width * height];
			Parallel.For(0, width * height, i =>
			{
				int ix = i % width;
				int iy = i / width;

				double sum = 1;
				double positiveDistance = 0;
				Point position = dataSource.Grid[ix, iy];
				ConvolutionColor color = ConvolutionColor.FromArgb(pixels[ix + width * iy]);
				int counter = 0;
				do
				{
					counter++;

					var vector = dataSource.Data[ix, iy];
					vector.Normalize();
					position += vector;
					positiveDistance += vector.Length;

					IntPoint coordinate;
					bool found = GetCoordinate(dataSource, position, out coordinate);
					if (found)
					{
						ix = coordinate.X;
						iy = coordinate.Y;
						var currentColor = ConvolutionColor.FromArgb(pixels[ix + iy * width]);// *1 / Math.Sqrt(counter);
						color += currentColor;

						sum += 1;
					}
					else
					{
						break;
					}
				}
				while (positiveDistance < L || counter < 50);

				var negativeDistance = 0.0;
				counter = 0;
				do
				{
					counter++;

					var vector = dataSource.Data[ix, iy];
					vector.Normalize();
					position -= vector;
					negativeDistance += vector.Length;

					IntPoint coordinate;
					bool found = GetCoordinate(dataSource, position, out coordinate);
					if (found)
					{
						ix = coordinate.X;
						iy = coordinate.Y;
						var currentColor = ConvolutionColor.FromArgb(pixels[ix + iy * width]);// * 1 / Math.Sqrt(counter);
						color += currentColor;

						sum += 1;
					}
					else
					{
						break;
					}
				}
				while (negativeDistance < L || counter < 50);

				color /= sum;
				effectivePixels[i] = color.ToArgb();
			});
			return effectivePixels;
		}

		private static ConvolutionColor GetColor(DataSource dataSource, int[] pixels, IntPoint coordinate, Point point)
		{
			int x = coordinate.X;
			int y = coordinate.Y;

			var x0 = dataSource.XCoordinates[x];
			var x1 = dataSource.XCoordinates[x + 1];

			var y0 = dataSource.YCoordinates[y];
			var y1 = dataSource.YCoordinates[y + 1];

			double xRatio = GetRatio(x0, x1, point.X);
			double yRatio = GetRatio(y0, y1, point.Y);

			int width = dataSource.Width;

			var v00 = pixels[x + y * width];
			var v01 = pixels[x + 1 + y * width];
			var v10 = pixels[x + (y + 1) * width];
			var v11 = pixels[x + 1 + (y + 1) * width];

			//var result = (int)(((1 - xRatio) * v00 + xRatio * v10 +
			//                (1 - xRatio) * v01 + xRatio * v11 +
			//                (1 - yRatio) * v00 + yRatio * v01 +
			//                (1 - yRatio) * v10 + yRatio * v11) * 0.25);
			var result = v00;

			return ConvolutionColor.FromArgb(result);
		}

		private static Vector GetVector(DataSource dataSource, IntPoint coordinate, Point point)
		{
			int x = coordinate.X;
			int y = coordinate.Y;

			var x0 = dataSource.XCoordinates[x];
			var x1 = dataSource.XCoordinates[x + 1];

			var y0 = dataSource.YCoordinates[y];
			var y1 = dataSource.YCoordinates[y + 1];

			double xRatio = GetRatio(x0, x1, point.X);
			double yRatio = GetRatio(y0, y1, point.Y);

			Vector v00 = dataSource.Data[x, y];
			Vector v01 = dataSource.Data[x, y + 1];
			Vector v10 = dataSource.Data[x + 1, y];
			Vector v11 = dataSource.Data[x + 1, y + 1];

			Vector result = (1 - xRatio) * v00 + xRatio * v10 +
							(1 - xRatio) * v01 + xRatio * v11 +
							(1 - yRatio) * v00 + yRatio * v01 +
							(1 - yRatio) * v10 + yRatio * v11;

			result *= 0.25;

			return result;
		}

		private static double GetRatio(double a, double b, double x)
		{
			return (x - a) / (b - a);
		}

		private static bool GetCoordinate(DataSource dataSource, Point point, out IntPoint coordinate)
		{
			var ix = IndexOf(dataSource.XCoordinates, point.X);
			int iy = IndexOf(dataSource.YCoordinates, point.Y);

			coordinate = new IntPoint(ix, iy);

			return ix > -1 && iy > -1;
		}

		private static int IndexOf(double[] coordinates, double x)
		{
			int ix = -1;
			for (int i = 0; i < coordinates.Length - 1; i++)
			{
				if (coordinates[i + 1] > x)
				{
					ix = i;
					break;
				}
			}

			return ix;
		}

		#endregion // end of Properties

		protected override void OnRender(DrawingContext drawingContext)
		{
			var dc = drawingContext;

			if (plotter == null) return;
			var transform = plotter.Transform;
			var contentBounds = Viewport2D.GetContentBounds(this);
			var screenBounds = contentBounds.ViewportToScreen(transform);

			dc.DrawImage(whiteNoizeBmp, screenBounds);
		}

		#region IPlotterElement Members

		Plotter2D plotter;
		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.CentralGrid.Children.Add(this);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			plotter.CentralGrid.Children.Remove(this);
			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
