using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;

namespace Microsoft.Research.Visualization3D.VertexStructures
{
    /// <summary>
    /// Indicates that the target code element is part of a vertex declaration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class VertexElementAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the stream index.
        /// </summary>
        /// <value>The stream index.</value>
        public int Stream
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tessellation method.
        /// </summary>
        /// <value>The tessellation method.</value>
        public DeclarationMethod Method
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the element usage.
        /// </summary>
        /// <value>The element usage.</value>
        public DeclarationUsage Usage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public DeclarationType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        internal int Offset
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexElementAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="usage">The vertex element usage.</param>
        public VertexElementAttribute(DeclarationType type, DeclarationUsage usage)
        {
            Type = type;
            Usage = usage;
        }
    }
}
