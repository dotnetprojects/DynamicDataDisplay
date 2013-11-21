using System;
using System.Windows.Media;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class ColorHelper
	{
		private readonly static Random random = new Random();

		/// <summary>
		/// Creates color from HSB color space with random hue and saturation and brighness equal to 1.
		/// </summary>
		/// <returns></returns>
		public static Color CreateColorWithRandomHue()
		{
			double hue = random.NextDouble() * 360;
			HsbColor hsbColor = new HsbColor(hue, 1, 1);
			return hsbColor.ToArgbColor();
		}

		public static Color[] CreateRandomColors(int colorNum)
		{
			double startHue = random.NextDouble() * 360;

			Color[] res = new Color[colorNum];
			double hueStep = 360.0 / colorNum;
			for (int i = 0; i < res.Length; i++)
			{
				double hue = startHue + i * hueStep;
				res[i] = new HsbColor(hue, 1, 1).ToArgbColor();
			}

			return res;
		}

		/// <summary>
		/// Creates color with fully random hue and slightly random saturation and brightness.
		/// </summary>
		/// <returns></returns>
		public static Color CreateRandomHsbColor()
		{
			double h = random.NextDouble() * 360;
			double s = random.NextDouble() * 0.5 + 0.5;
			double b = random.NextDouble() * 0.25 + 0.75;
			return new HsbColor(h, s, b).ToArgbColor();
		}

		/// <summary>
		/// Creates color with random hue.
		/// </summary>
		/// <param name="saturation">The saturation, [0..1].</param>
		/// <param name="brightness">The brightness, [0..1]</param>
		/// <returns></returns>
		public static Color CreateColorWithRandomHue(double saturation, double brightness)
		{
			double h = random.NextDouble() * 360;

			return new HsbColor(h, saturation, brightness).ToArgbColor();
		}

		/// <summary>
		/// Creates brush with random hue.
		/// </summary>
		/// <param name="saturation">The saturation, [0..1].</param>
		/// <param name="brightness">The brightness, [0..1].</param>
		/// <returns></returns>
		public static Brush CreateBrushWithRandomHue(double saturation, double brightness)
		{
			Color color = CreateColorWithRandomHue(saturation, brightness);

			return new SolidColorBrush(color);
		}

		/// <summary>
		/// Gets the random color (this property is created to use it from Xaml).
		/// </summary>
		/// <value>The random color.</value>
		public static Color RandomColor
		{
			get { return CreateRandomHsbColor(); }
		}

		/// <summary>
		/// Gets the random brush.
		/// </summary>
		/// <value>The random brush.</value>
		public static SolidColorBrush RandomBrush
		{
			get { return new SolidColorBrush(CreateRandomHsbColor()); }
		}

		public static int ToArgb(this Color color)
		{
			int result =
				color.A << 24 |
				color.R << 16 |
				color.G << 8 |
				color.B;

			return result;
		}
	}
}
