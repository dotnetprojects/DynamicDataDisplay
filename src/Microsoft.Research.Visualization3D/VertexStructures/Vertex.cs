using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace Microsoft.Research.Visualization3D.VertexStructures
{
    [StructLayout(LayoutKind.Sequential)]
    struct Vertex
    {
        [VertexStructures.VertexElement(DeclarationType.Float3, DeclarationUsage.Position)]
        public Vector3 Position;

        public static int SizeInBytes
        {
            get { return Marshal.SizeOf(typeof(Vertex)); }
        }

    }
}
