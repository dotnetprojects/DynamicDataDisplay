using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom
{
    public class DeepZoomViewer : Map
    {
        private DeepZoomTileServer tileServer = new DeepZoomTileServer();

        public DeepZoomViewer()
        {
            SourceTileServer = tileServer;
            Mode = TileSystemMode.OnlineOnly;
            TileProvider = new DeepZoomTileProvider();

            ProportionsRestriction.ProportionRatio = 1;
        }

        public DeepZoomViewer(string imagePath)
            : this()
        {
            ImagePath = imagePath;
        }

        public string ImagePath
        {
            get { return tileServer.ImagePath; }
            set
            {
                tileServer.ImagePath = value;
            }
        }

    }
}
