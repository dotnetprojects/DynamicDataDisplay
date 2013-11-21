using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class NormalizeFilter : VectorFieldConvolutionFilter
	{
		public override void Filter(int[] pixels, int width, int height)
		{
			double minBrightness = Double.PositiveInfinity;
			double maxBrightness = Double.NegativeInfinity;

			for (int i = 0; i < pixels.Length; i++)
			{
				int x = i % width;
				int y = i / width;

				if (width - x < 10) continue;
				if (height - y < 10) continue;

				int argb = pixels[i];
				var color = HsbColor.FromArgb(argb);
				var brightness = color.Brightness;

				if (brightness < minBrightness)
					minBrightness = brightness;
				if (brightness > maxBrightness)
					maxBrightness = brightness;
			}

			for (int i = 0; i < pixels.Length; i++)
			{
				int argb = pixels[i];
				var color = HsbColor.FromArgb(argb);
				var brightness = color.Brightness;

				double ratio = (brightness - minBrightness) / (maxBrightness - minBrightness);
				color.Brightness = ratio;
				pixels[i] = color.ToArgb();
			}
		}
	}
}
