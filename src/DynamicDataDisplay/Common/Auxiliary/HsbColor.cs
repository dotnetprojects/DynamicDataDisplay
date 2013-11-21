using System;
using System.Windows.Media;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Represents color in Hue Saturation Brightness color space.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hsb")]
	[DebuggerDisplay("HSBColor A={Alpha} H={Hue} S={Saturation} B={Brightness}")]
	public struct HsbColor
	{
		private double hue;
		private double saturation;
		private double brightness;
		private double alpha;

		/// <summary>Hue; [0, 360]</summary>
		public double Hue
		{
			get { return hue; }
			set
			{
				if (value < 0)
					value = 360 - value;

				hue = value % 360;
			}
		}

		/// <summary>Saturation; [0, 1]</summary>
		public double Saturation
		{
			get { return saturation; }
			set { saturation = value; }
		}

		/// <summary>Brightness; [0, 1]</summary>
		public double Brightness
		{
			get { return brightness; }
			set { brightness = value; }
		}

		/// <summary>Alpha; [0, 1]</summary>
		public double Alpha
		{
			get { return alpha; }
			set { alpha = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HSBColor"/> struct.
		/// </summary>
		/// <param name="hue">The hue; [0; 360]</param>
		/// <param name="saturation">The saturation; [0, 1]</param>
		/// <param name="brightness">The brightness; [0, 1]</param>
		public HsbColor(double hue, double saturation, double brightness)
		{
			this.hue = hue;
			this.saturation = saturation;
			this.brightness = brightness;
			alpha = 1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HSBColor"/> struct.
		/// </summary>
		/// <param name="hue">The hue; [0, 360]</param>
		/// <param name="saturation">The saturation; [0, 1]</param>
		/// <param name="brightness">The brightness; [0, 1]</param>
		/// <param name="alpha">The alpha; [0, 1]</param>
		public HsbColor(double hue, double saturation, double brightness, double alpha)
		{
			this.hue = hue;
			this.saturation = saturation;
			this.brightness = brightness;
			this.alpha = alpha;
		}

		/// <summary>
		/// Creates HSBColor from the ARGB color.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns></returns>
		public static HsbColor FromArgbColor(Color color)
		{
			double limit255 = 255;

			double r = color.R / limit255;
			double g = color.G / limit255;
			double b = color.B / limit255;

			double max = Math.Max(Math.Max(r, g), b);
			double min = Math.Min(Math.Min(r, g), b);

			double len = max - min;

			double brightness = max; // 0.5 * (max + min);
			double sat;
			double hue;


			if (max == 0 || len == 0)
			{
				sat = hue = 0;
			}
			else
			{
				sat = len / max;
				if (r == max)
				{
					hue = (g - b) / len;
				}
				else if (g == max)
				{
					hue = 2 + (b - r) / len;
				}
				else
				{
					hue = 4 + (r - g) / len;
				}
			}

			hue *= 60;
			if (hue < 0)
				hue += 360;


			HsbColor res = new HsbColor();
			res.hue = hue;
			res.saturation = sat;
			res.brightness = brightness;
			res.alpha = color.A / limit255;
			return res;
		}

		public static HsbColor FromArgb(int argb)
		{
			byte a = (byte)(argb >> 24);
			byte r = (byte)((argb >> 16) & 0xFF);
			byte g = (byte)((argb >> 8) & 0xFF);
			byte b = (byte)(argb & 0xFF);
			return FromArgbColor(Color.FromArgb(a, r, g, b));
		}

		/// <summary>
		/// Converts HSBColor to ARGB color space.
		/// </summary>
		/// <returns></returns>
		public Color ToArgbColor()
		{
			double r = 0.0;
			double g = 0.0;
			double b = 0.0;
			double hue = this.hue % 360.0;
			if (saturation == 0.0)
			{
				r = g = b = brightness;
			}
			else
			{
				double smallHue = hue / 60.0;
				int smallHueInt = (int)Math.Floor(smallHue);
				double smallHueFrac = smallHue - smallHueInt;
				double val1 = brightness * (1.0 - saturation);
				double val2 = brightness * (1.0 - (saturation * smallHueFrac));
				double val3 = brightness * (1.0 - (saturation * (1.0 - smallHueFrac)));
				switch (smallHueInt)
				{
					case 0:
						r = brightness;
						g = val3;
						b = val1;
						break;

					case 1:
						r = val2;
						g = brightness;
						b = val1;
						break;

					case 2:
						r = val1;
						g = brightness;
						b = val3;
						break;

					case 3:
						r = val1;
						g = val2;
						b = brightness;
						break;

					case 4:
						r = val3;
						g = val1;
						b = brightness;
						break;

					case 5:
						r = brightness;
						g = val1;
						b = val2;
						break;
				}
			}


			return Color.FromArgb(
				(byte)(Math.Round(alpha * 255)),
				(byte)(Math.Round(r * 255)),
				(byte)(Math.Round(g * 255)),
				(byte)(Math.Round(b * 255)));
		}

		public int ToArgb()
		{
			return ToArgbColor().ToArgb();
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj is HsbColor)
			{
				HsbColor c = (HsbColor)obj;
				return (c.alpha == alpha &&
					c.brightness == brightness &&
					c.hue == hue &&
					c.saturation == saturation);
			}
			else
				return false;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return alpha.GetHashCode() ^
				brightness.GetHashCode() ^
				hue.GetHashCode() ^
				saturation.GetHashCode();
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="first">The first.</param>
		/// <param name="second">The second.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(HsbColor first, HsbColor second)
		{
			return (first.alpha == second.alpha &&
				first.brightness == second.brightness &&
				first.hue == second.hue &&
				first.saturation == second.saturation);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="first">The first.</param>
		/// <param name="second">The second.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(HsbColor first, HsbColor second)
		{
			return (first.alpha != second.alpha ||
				first.brightness != second.brightness ||
				first.hue != second.hue ||
				first.saturation != second.saturation);
		}
	}

	public static class ColorExtensions
	{
		/// <summary>
		/// Converts the ARGB color to the HSB color.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hsb")]
		public static HsbColor ToHsbColor(this Color color)
		{
			return HsbColor.FromArgbColor(color);
		}
	}
}
