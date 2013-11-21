using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class NaiveColorMap
	{
		public double[,] Data { get; set; }

		public IPalette Palette { get; set; }

		public BitmapSource BuildImage()
		{
			if (Data == null)
				throw new ArgumentNullException("Data");
			if (Palette == null)
				throw new ArgumentNullException("Palette");


			int width = Data.GetLength(0);
			int height = Data.GetLength(1);

			int[] pixels = new int[width * height];

			var minMax = Data.GetMinMax();
			var min = minMax.Min;
			var rangeDelta = minMax.GetLength();

			int pointer = 0;
			for (int iy = 0; iy < height; iy++)
			{
				for (int ix = 0; ix < width; ix++)
				{
					double value = Data[ix, height - 1 - iy];
					double ratio = (value - min) / rangeDelta;
					Color color = Palette.GetColor(ratio);
					int argb = color.ToArgb();

					pixels[pointer++] = argb;
				}
			}

			WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
			int bpp = (bitmap.Format.BitsPerPixel + 7) / 8;
			int stride = bitmap.PixelWidth * bpp;

			bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

			return bitmap;
		}
	}
}
