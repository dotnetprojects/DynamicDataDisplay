using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	public interface IDirectAccessTileServer : ITileServer
	{
		BitmapSource this[TileIndex id] { get; }
	}
}
