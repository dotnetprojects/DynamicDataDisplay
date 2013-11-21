using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Microsoft.Research.DynamicDataDisplay.DirectX2D
{
	public struct VertexPosition3ColorPointSize
	{
		public static int SizeInBytes
		{
			get { return 3 * 4 + 4 + 4; }
		}

		public Vector3 Position;
		public float PointSize;
		public int Color;
	}
}
