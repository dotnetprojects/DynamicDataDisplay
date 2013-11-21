using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public struct IntPoint
	{
		public IntPoint(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public int X;
		public int Y;
	}
}
