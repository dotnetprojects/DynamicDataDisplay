using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Common.Palettes
{
	/// <summary>
	/// Represents a color step with its offset in limits [0..1].
	/// </summary>
	[DebuggerDisplay("Color={Color}, Offset={Offset}")]
	public class LinearPaletteColorStep
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LinearPaletteColorStep"/> class.
		/// </summary>
		public LinearPaletteColorStep() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="LinearPaletteColorStep"/> class.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="offset">The offset.</param>
		public LinearPaletteColorStep(Color color, double offset)
		{
			this.Color = color;
			this.Offset = offset;
		}

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public Color Color { get; set; }
		/// <summary>
		/// Gets or sets the offset.
		/// </summary>
		/// <value>The offset.</value>
		public double Offset { get; set; }
	}

	/// <summary>
	/// Represents a palette with start and stop colors and intermediate colors with their custom offsets.
	/// </summary>
	public class LinearPalette : IPalette
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LinearPalette"/> class.
		/// </summary>
		/// <param name="startColor">The start color.</param>
		/// <param name="endColor">The end color.</param>
		/// <param name="steps">The steps.</param>
		public LinearPalette(Color startColor, Color endColor, params LinearPaletteColorStep[] steps)
		{
			this.steps.Add(new LinearPaletteColorStep(startColor, 0));
			if (steps != null)
				this.steps.AddMany(steps);
			this.steps.Add(new LinearPaletteColorStep(endColor, 1));
		}

		private readonly List<LinearPaletteColorStep> steps = new List<LinearPaletteColorStep>();

		#region IPalette Members

		/// <summary>
		/// Gets the color by interpolation coefficient.
		/// </summary>
		/// <param name="t">Interpolation coefficient, should belong to [0..1].</param>
		/// <returns>Color.</returns>
		public Color GetColor(double t)
		{
			if (t < 0) return steps[0].Color;
			if (t > 1) return steps[steps.Count - 1].Color;

			int i = 0;
			double x = 0;
			while (x <= t)
			{
				x = steps[i + 1].Offset;
				i++;
			}

			Color c0 = steps[i - 1].Color;
			Color c1 = steps[i].Color;
			double ratio = (t - steps[i - 1].Offset) / (steps[i].Offset - steps[i - 1].Offset);

			Color result = Color.FromRgb(
				(byte)((1 - ratio) * c0.R + ratio * c1.R),
				(byte)((1 - ratio) * c0.G + ratio * c1.G),
				(byte)((1 - ratio) * c0.B + ratio * c1.B));
			return result;
		}

		public event EventHandler Changed;

		#endregion
	}
}
