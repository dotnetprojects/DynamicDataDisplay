using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.ComponentModel;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Common.Palettes
{
	public sealed class HSBPalette : IPalette
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HSBPalette"/> class.
		/// </summary>
		public HSBPalette() { }

		private double start = 0;
		[DefaultValue(0.0)]
		public double Start
		{
			get { return start; }
			set
			{
				if (start != value)
				{
					start = value;
					Changed.Raise(this);
				}
			}
		}

		private double width = 360;
		[DefaultValue(360.0)]
		public double Width
		{
			get { return width; }
			set
			{
				if (width != value)
				{
					width = value;
					Changed.Raise(this);
				}
			}
		}

		#region IPalette Members

		public Color GetColor(double t)
		{
			Verify.IsTrue(0 <= t && t <= 1);

			return new HsbColor(start + t * width, 1, 1).ToArgbColor();
		}

		public event EventHandler Changed;

		#endregion
	}
}
