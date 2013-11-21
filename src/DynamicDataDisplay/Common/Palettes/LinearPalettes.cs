using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Common.Palettes
{
	/// <summary>
	/// Contains some predefined linear palettes.
	/// </summary>
	public static class LinearPalettes
	{
		private const double geoHeight = 8845 + 400;
		private static LinearPalette geoHeightsPalette = new LinearPalette(
			Color.FromRgb(1, 99, 69), Colors.White,
			new LinearPaletteColorStep(Color.FromRgb(28, 128, 52), (50 + 400) / geoHeight),
			new LinearPaletteColorStep(Color.FromRgb(229, 209, 119), (200 + 400) / geoHeight),
			new LinearPaletteColorStep(Color.FromRgb(160, 66, 1), (1000 + 400) / geoHeight),
			new LinearPaletteColorStep(Color.FromRgb(129, 32, 32), (2000 + 400) / geoHeight),
			new LinearPaletteColorStep(Color.FromRgb(119, 119, 119), (4000 + 400) / geoHeight),
			new LinearPaletteColorStep(Color.FromRgb(244, 244, 244), (6000 + 400) / geoHeight));

		/// <summary>
		/// Gets the palette for geo height map visualization.
		/// </summary>
		/// <value>The geo heights palette.</value>
		public static LinearPalette GeoHeightsPalette { get { return geoHeightsPalette; } }
	}
}
