using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;

namespace Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom
{
    public class DeepZoomTileProvider : MapTileProvider
    {
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public override DataRect GetTileBounds(TileIndex id)
        {
            return new DataRect(0, 0, 1, 1);
        }

        //public override double GetTileWidth(double level)
        //{
        //    throw new NotImplementedException();
        //}

        //public override double GetTileHeight(double level)
        //{
        //    throw new NotImplementedException();
        //}

        //public override IEnumerable<TileIndex> GetTilesForRegion(DataRect region, double level)
        //{
        //    if (region.IsEmpty)
        //        yield break;

        //    checked
        //    {
        //        double tileWidth = TileWidth;
        //        double tileHeight = TileHeight;

        //        int minIx = (int)Math.Floor(region.XMin / tileWidth);
        //        int maxIx = (int)Math.Ceiling(region.XMax / tileWidth);

        //        int minIy = (int)Math.Floor(region.YMin / tileHeight);
        //        int maxIy = (int)Math.Ceiling(region.YMax / tileHeight);

        //        var maxSideCount = GetSideTilesCount(Level);

        //        int maxIndex = maxSideCount / 2;
        //        if (maxIx > maxIndex)
        //            maxIx = maxIndex;
        //        if (maxIy > maxIndex)
        //            maxIy = maxIndex;
        //        if (minIx < -maxIndex)
        //            minIx = -maxIndex;
        //        if (minIy < -maxIndex)
        //            minIy = -maxIndex;

        //        if (level != 0)
        //        {
        //            maxIx--;
        //            maxIy--;
        //        }


        //        for (int ix = minIx; ix <= maxIx; ix++)
        //        {
        //            for (int iy = minIy; iy <= maxIy; iy++)
        //            {
        //                yield return new TileIndex(ix, iy, level);
        //            }
        //        }
        //    }
        //}
    }
}
