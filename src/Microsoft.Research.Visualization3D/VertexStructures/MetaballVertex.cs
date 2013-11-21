using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SlimDX.Direct3D9;
using SlimDX;
using System.Globalization;

namespace Microsoft.Research.Visualization3D.VertexStructures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MetaballVertex
    {
         private Vector3 m_Position;
        /// <summary>
        /// Gets or sets the transformed position of the vertex.
        /// </summary>
        /// <value>The transformed position of the vertex.</value>
        [VertexElement(DeclarationType.Float3, DeclarationUsage.Position)]
        public Vector3 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        private Vector3 m_Normal;
        /// <summary>
        /// Gets of sets transformed normal of the vertex
        /// </summary>
        [VertexElement(DeclarationType.Float3, DeclarationUsage.Normal)]
        public Vector3 Normal
        {
            get { return m_Normal; }
            set { m_Normal = value; }
        }

        /// <summary>
        /// Gets the size in bytes.
        /// </summary>
        /// <value>The size in bytes.</value>
        public static int SizeInBytes
        {
            get { return Marshal.SizeOf(typeof(MetaballVertex)); }
        }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>The format.</value>
        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Normal; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformedColoredTexturedVertex"/> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        /// <param name="textureCoordinates">The texture coordinates.</param>
        public MetaballVertex(Vector3 position, Vector3 normal)
            : this()
        {
            Position = position;
            Normal = normal;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MetaballVertex left, MetaballVertex right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(MetaballVertex left, MetaballVertex right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return Position.GetHashCode() + Normal.GetHashCode();
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return Equals((MetaballVertex)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(MetaballVertex other)
        {
            return (Position == other.Position&& Normal == other.Normal);
        }

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> representing the vertex.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0} ({1}, {2})", Position.ToString(), Normal.ToString());
        }
    }
}
