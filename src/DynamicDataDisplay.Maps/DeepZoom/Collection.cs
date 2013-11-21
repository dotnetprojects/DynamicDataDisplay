using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom
{
    public class Collection
    {
        public Collection()
        {
            Quality = 1;
        }

        /// <summary>
        /// Contains the items in the collection.
        /// </summary>
        public List<I> Items { get; set; }

        /// <summary>
        /// The maximum pyramid level the tiles are stored at. 
        /// This must be less than or equal to log2(TileSize) - typically equal to this value or one less.
        /// </summary>
        [XmlAttribute]
        public byte MaxLevel { get; set; }

        /// <summary>
        /// The size of the tiles. Note they have to be square. 
        /// This is true for both images and collections. 
        /// However, for collections, the TileSize also has to be a power of 2 (e.g. 128, 256, 512, etc).
        /// </summary>
        [XmlAttribute]
        public long TileSize { get; set; }

        /// <summary>
        /// This defines the file format of the tiles.
        /// </summary>
        [XmlAttribute]
        public string Format { get; set; }

        /// <summary>
        /// Used when creating thumbnail tiles, from 0 to 1. 1 is highest quality, 0 is lowest. 
        /// Generally this value should be 0.8 or higher; however, for Deep Zoom this does not matter since collections are read-only.
        /// </summary>
        [XmlAttribute]
        public float Quality { get; set; }

        /// <summary>
        /// Gets the count of items in the collection; however this does not actually matter for Deep Zoom since collections are read only.
        /// </summary>
        [XmlAttribute]
        public ulong NextItemId { get; set; }
    }
}
