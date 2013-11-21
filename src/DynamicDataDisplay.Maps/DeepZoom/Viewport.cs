using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom
{
    /// <summary>
    /// Location of the image in the viewport.
    /// </summary>
    public class Viewport
    {
        /// <summary>
        /// The width of the area of the image displayed. This value is in logical coordinates. 
        /// For example a value of 1 displays the entire image (no zoom), 
        /// a value of 0.5 is 200% zoomed in and a value of 0 is completely zoomed (user cannot see the image at all). 
        /// A value above 1 is zooming out from the image. 
        /// For example, a value of 2 means that the image will take up half the size of the MultiScaleSubImage control area (50% zoom).
        /// </summary>
        [XmlAttribute]
        public double Width { get; set; }

        /// <summary>
        /// The left coordinate of the rectangular area of the image to be displayed. 
        /// The coordinates of the point are in local coordinates (0-1) relative to the displayed image width.
        /// </summary>
        [XmlAttribute]
        public double X { get; set; }

        /// <summary>
        /// The top coordinate of the rectangular area of the image to be displayed. 
        /// The coordinates of the point are in local coordinates (0-1) relative to the displayed image width.
        /// </summary>
        [XmlAttribute]
        public double Y { get; set; }
    }
}
