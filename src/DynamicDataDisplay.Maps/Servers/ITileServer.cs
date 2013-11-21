using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
    public interface ITileServer
    {
        bool Contains(TileIndex id);
        void BeginLoadImage(TileIndex id);
        event EventHandler<TileLoadResultEventArgs> ImageLoaded;

        string ServerName { get; }

        event EventHandler Changed;
    }
}
