using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace Microsoft.Research.Visualization3D.VertexStructures
{
    /// <summary>
    /// Represent Vertex structure for advanced particle system
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ParticleVertex
    {
        private Vector3 position;
        /// <summary>
        /// Particle Position
        /// </summary>
        [VertexElement(DeclarationType.Float3, DeclarationUsage.Position)]
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        private Vector3 velocity;
        /// <summary>
        /// Particle Velocity
        /// </summary>
        [VertexElement(DeclarationType.Float3, DeclarationUsage.Normal)]
        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        private int color;
        /// <summary>
        /// Particle color
        /// </summary>
        [VertexElement(DeclarationType.Color, DeclarationUsage.Color)]
        public int Color
        {
            get { return color; }
            set { color = value; }
        }

        private float time;
        /// <summary>
        /// particle creation time
        /// </summary>
        [VertexElement(DeclarationType.Float1, DeclarationUsage.TextureCoordinate)]
        public float Time
        {
            get { return time; }
            set { time = value; }
        }

        /// <summary>
        /// Gets the size in bytes.
        /// </summary>
        /// <value>The size in bytes.</value>
        public static int SizeInBytes
        {
            get { return Marshal.SizeOf(typeof(ParticleVertex)); }
        }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>The format.</value>
        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse | VertexFormat.Texture1; }
        }

        public ParticleVertex(Vector3 position, Vector3 velocity, int color, float time)
        {
            this.position = position;
            this.velocity = velocity;
            this.color = color;
            this.time = time;
        }
    }
}
