using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Microsoft.Research.DynamicDataDisplay.DirectX2D
{
	public struct VertexPosition4Color
	{
		public static int SizeInBytes
		{
			get { return 4 * sizeof(float) + sizeof(int); }
		}

		public Vector4 Position;
		public int Color;
	}
}
