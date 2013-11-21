using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class BrushHelper
	{
		/// <summary>
		/// Creates a SolidColorBrush with random hue of its color.
		/// </summary>
		/// <returns>A SolicColorBrush with random hue of its color.</returns>
		public static SolidColorBrush CreateBrushWithRandomHue()
		{
			return new SolidColorBrush { Color = ColorHelper.CreateColorWithRandomHue() };
		}

		/// <summary>
		/// Makes SolidColorBrush transparent.
		/// </summary>
		/// <param name="brush">The brush.</param>
		/// <param name="alpha">The alpha, [0..255]</param>
		/// <returns></returns>
		public static SolidColorBrush MakeTransparent(this SolidColorBrush brush, int alpha)
		{
			Color color = brush.Color;
			color.A = (byte)alpha;

			return new SolidColorBrush(color);
		}

		/// <summary>
		/// Makes SolidColorBrush transparent.
		/// </summary>
		/// <param name="brush">The brush.</param>
		/// <param name="alpha">The alpha, [0.0 .. 1.0].</param>
		/// <returns></returns>
		public static SolidColorBrush MakeTransparent(this SolidColorBrush brush, double opacity)
		{
			return MakeTransparent(brush, (int)(opacity * 255));
		}

		public static SolidColorBrush ChangeLightness(this SolidColorBrush brush, double lightnessFactor)
		{
			Color color = brush.Color;
			HsbColor hsbColor = HsbColor.FromArgbColor(color);
			hsbColor.Brightness *= lightnessFactor;

			if (hsbColor.Brightness > 1.0) hsbColor.Brightness = 1.0;

			SolidColorBrush result = new SolidColorBrush(hsbColor.ToArgbColor());
			return result;
		}

		public static SolidColorBrush ChangeSaturation(this SolidColorBrush brush, double saturationFactor)
		{
			Color color = brush.Color;
			HsbColor hsbColor = HsbColor.FromArgbColor(color);
			hsbColor.Saturation *= saturationFactor;

			if (hsbColor.Saturation > 1.0) hsbColor.Saturation = 1.0;

			SolidColorBrush result = new SolidColorBrush(hsbColor.ToArgbColor());
			return result;
		}
	}
}
