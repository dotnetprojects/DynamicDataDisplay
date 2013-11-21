using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Common.Palettes
{
	public sealed class DelegatePalette : PaletteBase
	{
		public DelegatePalette(Func<double, Color> func)
		{
			if (func == null)
				throw new ArgumentNullException("func");

			this.func = func;
		}

		private readonly Func<double, Color> func;

		public override Color GetColor(double t)
		{
			return func(t);
		}
	}
}
