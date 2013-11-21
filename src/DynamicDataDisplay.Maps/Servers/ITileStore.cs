using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
    /// <summary>
    /// Contains a method to save tile image for given id.
    /// </summary>
    public interface ITileStore
    {
        /// <summary>
        /// Begins to save image for given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="image">The image.</param>
        void BeginSaveImage(TileIndex id, BitmapSource image, Stream stream);

        /// <summary>
        /// Clears this cache - deletes all tiles.
        /// </summary>
        void Clear();
    }
}
