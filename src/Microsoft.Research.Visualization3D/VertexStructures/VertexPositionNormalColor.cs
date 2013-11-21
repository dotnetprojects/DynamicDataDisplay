using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using System.Runtime.InteropServices;
using SlimDX;
using System.Globalization;

namespace Microsoft.Research.Visualization3D.VertexStructures
{
    /// <summary>
    /// Represents a single vertex with position, normal and color.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalColor : IEquatable<VertexPositionNormalColor>
    {

        public static readonly VertexElement[] VertexElements;

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

        private int m_Color;
        /// <summary>
        /// Gets or sets the color of the vertex.
        /// </summary>
        /// <value>The color of the vertex.</value>
        [VertexElement(DeclarationType.Color, DeclarationUsage.Color)]
        public int Color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }

        /// <summary>
        /// Gets the size in bytes.
        /// </summary>
        /// <value>The size in bytes.</value>
        public static int SizeInBytes
        {
            get { return Marshal.SizeOf(typeof(VertexPositionNormalColor)); }
        }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>The format.</value>
        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformedColoredTexturedVertex"/> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        /// <param name="textureCoordinates">The texture coordinates.</param>
        public VertexPositionNormalColor(Vector3 position, Vector3 normal, int color)
            : this()
        {
            Position = position;
            Color = color;
            Normal = normal;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(VertexPositionNormalColor left, VertexPositionNormalColor right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(VertexPositionNormalColor left, VertexPositionNormalColor right)
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
            return Position.GetHashCode() + Color.GetHashCode() + Normal.GetHashCode();
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

            return Equals((VertexPositionNormalColor)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(VertexPositionNormalColor other)
        {
            return (Position == other.Position && Color == other.Color && Normal == other.Normal);
        }

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> representing the vertex.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0} ({1}, {2})", Position.ToString(), Color.ToString(), Normal.ToString());
        }

        static VertexPositionNormalColor()
        {
            VertexElements = new VertexElement[] 
            { 
                new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0,12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                new VertexElement(0, 24, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0) 
            };
        }
    }
}
