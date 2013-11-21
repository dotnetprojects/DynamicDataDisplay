using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Microsoft.Research.DynamicDataDisplay.DirectX2D
{
	public struct VertexPosition3Color
	{
		public static int SizeInBytes
		{
			get { return 3 * sizeof(float) + sizeof(int); }
		}

		public Vector3 Position;
		public int Color;
	}
}
