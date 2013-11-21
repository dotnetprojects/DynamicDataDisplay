using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom
{
    public class DeepZoomTileServer : SourceTileServer
    {
        private const string DeepZoomNamespace = "http://schemas.microsoft.com/deepzoom/2009";
        private DeepZoomFileServer fileServer;
        private DeepZoomTileProvider tileProvider;

        public DeepZoomFileServer FileServer
        {
            get { return fileServer; }
        }

        public DeepZoomTileServer()
        {
        }

        void fileServer_ImageLoaded(object sender, TileLoadResultEventArgs e)
        {
            if (e.Result == TileLoadResult.Success)
                ReportSuccess(e.Image, e.ID);
            else
                ReportFailure(e.ID);
        }

        void fileServer_Changed(object sender, EventArgs e)
        {
        }

        private string imagePath;
        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("ImagePath");

                imagePath = value;
                OnImagePathChanged();
            }
        }

        private void OnImagePathChanged()
        {
            Collection collection = null;
            using (XmlReader reader = XmlReader.Create(imagePath))
            {
                XmlSerializer collectionSerializer = new XmlSerializer(typeof(Collection), DeepZoomNamespace);
                collection = (Collection)collectionSerializer.Deserialize(reader);

                XmlSerializer imageSerializer = new XmlSerializer(typeof(Image), DeepZoomNamespace);
                var imageFolder = Path.GetDirectoryName(imagePath);
                var cacheFolder = Path.Combine(imageFolder, @"dzc_output_images\dsc01770_files");

                fileServer = new DeepZoomFileServer { CacheLocation = CacheLocation.CustomPath, CachePath = cacheFolder };
                fileServer.Changed += new EventHandler(fileServer_Changed);
                fileServer.ImageLoaded += new EventHandler<TileLoadResultEventArgs>(fileServer_ImageLoaded);
                fileServer.FileExtension = "." + collection.Format;

                foreach (var i in collection.Items)
                {
                    var imgPath = Path.Combine(imageFolder, i.Source);
                    using (XmlReader imageReader = XmlReader.Create(imgPath))
                    {
                        Image image = (Image)imageSerializer.Deserialize(imageReader);
                        i.Image = image;
                    }
                }
            }

            var source = collection.Items[0].Source;

            var cacheParentDir = Path.Combine(Path.GetDirectoryName(imagePath), Path.GetDirectoryName(source));
            var dirName = Path.GetFileNameWithoutExtension(source) + "_files";
            var imagesPath = Path.Combine(cacheParentDir, dirName);

            LoadDeepZoomImages(imagesPath, collection.Format);
        }

        private void LoadDeepZoomImages(string imagesPath, string extension)
        {
            string searchPattern = "*." + extension;
            DirectoryInfo parentDir = new DirectoryInfo(imagesPath);
            foreach (var dir in parentDir.GetDirectories())
            {
                int zoomLevel = Int32.Parse(dir.Name);

                foreach (var file in dir.GetFiles(searchPattern))
                {
                    string[] coordStrings = Path.GetFileNameWithoutExtension(file.Name).Split('_');
                    int x = Int32.Parse(coordStrings[0]);
                    int y = Int32.Parse(coordStrings[1]);

                    deepZoomImages.Add(new TileIndex(x, y, zoomLevel));
                }
            }
        }

        private readonly HashSet<TileIndex> deepZoomImages = new HashSet<TileIndex>();

        public override bool Contains(TileIndex id)
        {
            return deepZoomImages.Contains(id);
        }

        public override void BeginLoadImage(TileIndex id)
        {
            if (Contains(id) && fileServer != null)
                fileServer.BeginLoadImage(id);
        }

        public override bool CanLoadFast(TileIndex id)
        {
            return deepZoomImages.Contains(id);
            //return base.CanLoadFast(id);
        }
    }
}
