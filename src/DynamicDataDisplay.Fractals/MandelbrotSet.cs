using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;

namespace Microsoft.Research.DynamicDataDisplay.Fractals
{
	public class MandelbrotSet
	{
		private double xMin;
		private double yMin;
		private double xMax;
		private double yMax;

		private int size;
		public double BailOut = 5;
		public int MaxIterations = 500;

		public MandelbrotSet(int size, DataRect bounds)
		{
			this.size = size;
			this.xMin = bounds.XMin;
			this.yMin = bounds.YMin;
			this.xMax = bounds.XMax;
			this.yMax = bounds.YMax;
		}

		public int GetNumIterations(double re, double im)
		{
			double bailOutSquare = BailOut * BailOut;
			double z_re = 0;
			double z_im = 0;
			double prev_re = z_re * z_re;
			double prev_im = z_im * z_im;
			for (int i = 0; i < MaxIterations; i++)
			{
				double t_re = prev_re - prev_im + re;
				double t_im = 2 * z_re * z_im + im;
				z_re = t_re;
				z_im = t_im;
				prev_re = z_re * z_re;
				prev_im = z_im * z_im;
				if (prev_re + prev_im > bailOutSquare)
					return i;
			}
			return MaxIterations;
		}

		public IPalette Palette { get; set; }

		public int GetColor(double re, double im)
		{
			int n = GetNumIterations(re, im);
			if (n < MaxIterations)
			{
				double t = n / (double)MaxIterations;

				return Palette.GetColor(t).ToArgb();

				return Color.FromArgb(255, (byte)(255 * t), (byte)(255 * (1 - t)), (byte)(255 * (t))).ToArgb();

				return Color.FromArgb(255, (byte)(255 * Math.Sqrt(1 - t)), 0, (byte)Math.Sqrt(t)).ToArgb();


				//return Color.FromArgb(255, (byte)(255 * Math.Sqrt(1 - t)), 0, (byte)Math.Sqrt(t)).ToArgb();
			}
			else
			{
				return Colors.Black.ToArgb();
			}
		}

		public BitmapSource Draw()
		{
			int count = size * size;
			int[] pixels = new int[count];

			for (int i = 0; i < count; i++)
			{
				int x = i % size;
				int y = i / size;

				int pixel = GetColor(xMin + x * (xMax - xMin) / (size - 1),
					yMin + y * (yMax - yMin) / (size - 1));
				pixels[i] = pixel;
			}

			var bmp = WriteableBitmap.Create(size, size, 96, 96, PixelFormats.Bgra32, null, pixels, size * 4);
			return bmp;
		}
	}
}
