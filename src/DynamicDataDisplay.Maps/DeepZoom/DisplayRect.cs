using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom
{
    /// <summary>
    /// One or more of these elements are used to describe available pixels.
    /// </summary>
    public struct DisplayRect
    {
        /// <summary>
        /// Defines the rectangle to be displayed.
        /// </summary>
        public uint32rect Rect { get; set; }

        /// <summary>
        /// Index of the lowest level at which the rectangle is displayed.
        /// </summary>
        public ulong MinLevel { get; set; }

        /// <summary>
        /// Index of the highest level at which the rectangle is displayed.
        /// </summary>
        public ulong MaxLevel { get; set; }
    }
}
