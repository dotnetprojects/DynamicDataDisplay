using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;

namespace Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom
{
    public class DeepZoomFileServer : AsyncFileSystemServer
    {
        protected override string GetNameByIndex(TileIndex index)
        {
            int level = (int)index.Level;
            int x = index.X; //MapTileProvider.GetSideTilesCount(level) + index.X - 1;
            int y = index.Y; //MapTileProvider.GetSideTilesCount(level) - index.Y - 2;

            var result = String.Format("{0}_{1}", x.ToString(), y.ToString());
            return result;
        }

        protected override string GetDirPath(double level)
        {
            var result = ((int)level).ToString();
            return result;
        }
    }
}
