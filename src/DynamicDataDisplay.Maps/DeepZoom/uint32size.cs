using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom
{
    public struct uint32size
    {
        /// <summary>
        /// The width of the image.
        /// </summary>
        [XmlAttribute]
        public ulong Width { get; set; }
        /// <summary>
        /// The height of the image.
        /// </summary>
        [XmlAttribute]
        public ulong Height { get; set; }
    }
}
