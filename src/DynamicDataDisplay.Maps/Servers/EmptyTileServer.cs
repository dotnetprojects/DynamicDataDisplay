using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers
{
	public class EmptyTileServer : SourceTileServer
	{
		#region ITileServer Members

		public override bool Contains(TileIndex id)
		{
			return false;
		}

		public override void BeginLoadImage(TileIndex id) { }

		protected override string GetCustomName()
		{
			return "Empty";
		}

		#endregion
	}
}
