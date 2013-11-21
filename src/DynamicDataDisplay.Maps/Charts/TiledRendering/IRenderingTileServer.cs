using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.TiledRendering
{
	public interface IRenderingTileServer
	{
		DataRect ContentBounds { get; set; }
	}
}
