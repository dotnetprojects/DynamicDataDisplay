using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Common.Palettes
{
	/// <summary>
	/// Represents a palette that calculates minimal and maximal values of interpolation coefficient and every 100000 calls writes these values 
	/// to DEBUG console.
	/// </summary>
	public class MinMaxLoggingPalete : DecoratorPaletteBase
	{
		double min = Double.MaxValue;
		double max = Double.MinValue;
		int counter = 0;
		public override Color GetColor(double t)
		{
			if (t < min) min = t;
			if (t > max) max = t;
			counter++;

			if (counter % 100000 == 0)
			{
				Debug.WriteLine("Palette: Min = " + min + ", max = " + max);
			}

			return base.GetColor(t);
		}
	}
}
