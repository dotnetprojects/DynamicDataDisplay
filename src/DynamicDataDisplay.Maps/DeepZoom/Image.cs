using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom
{
    /// <summary>
    /// If you are defining a single image (versus a collection of images) for Deep Zoom, this element is the root element of the schema.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// If this element is not present, the image is not sparse - all pixels are available at all levels. 
        /// Otherwise, you can use the DisplayRect elements to describe all the available pixels.
        /// </summary>
        public List<DisplayRect> DisplayRects { get; set; }

        /// <summary>
        /// Defines the size of the image in pixels.
        /// </summary>
        [XmlElement]
        public uint32size Size { get; set; }

        /// <summary>
        /// The tile size of the level in pixels. Note that these have to be square. 
        /// Unlike Collection.TileSize, the TileSize for an Image does not have to be a power of 2 value.
        /// </summary>
        [XmlAttribute]
        public ulong TileSize { get; set; }

        /// <summary>
        /// The tile overlap on all four sides of the tiles. A value of 0 is allowed for Deep Zoom images.
        /// </summary>
        [XmlAttribute]
        public ulong Overlap { get; set; }

        /// <summary>
        /// This defines the file format of the tiles as an extension.
        /// </summary>
        [XmlAttribute]
        public string Format { get; set; }
    }
}
