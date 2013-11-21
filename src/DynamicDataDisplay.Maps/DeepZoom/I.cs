using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom
{
    public class I
    {
        /// <summary>
        /// Size of the image in pixels.
        /// </summary>
        [XmlElement]
        public uint32size Size { get; set; }

        /// <summary>
        /// This is the number of the item (Morton Number) where it appears in the tiles.
        /// </summary>
        [XmlAttribute]
        public ulong N { get; set; }

        /// <summary>
        /// This is a number associated with the item. It could be a database key or any other number that you will find useful. By default it’s the same as I.N.
        /// </summary>
        [XmlAttribute]
        public ulong Id { get; set; }

        /// <summary>
        /// This is the path to the .dzi file associated with this item in the collection. It can be absolute or relative.
        /// </summary>
        [XmlAttribute]
        public string Source { get; set; }

        private bool isPath = true;
        /// <summary>
        /// Determines whether the Source is a path. Always 1 for Deep Zoom images so this attribute does not need to be included.
        /// </summary>
        [XmlAttribute]
        public bool IsPath
        {
            get { return isPath; }
            set { isPath = value; }
        }

        private string type = "ImagePixelSource";
        /// <summary>
        /// The pixel source type. Always ImagePixelSource for Deep Zoom images so this attribute does not need to be included.
        /// </summary>
        [XmlAttribute]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Location of the image in the viewport.
        /// </summary>
        [XmlElement]
        public Viewport Viewport { get; set; }

        public Image Image { get; set; }
    }
}
