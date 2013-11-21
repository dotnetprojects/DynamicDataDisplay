using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.IO;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers.FileServers
{
	public sealed class DefaultPathProvider : TilePathProvider
	{
		public override string GetTilePath(TileIndex id)
		{
			StringBuilder builder = new StringBuilder("z");

			builder = builder.Append(id.Level).Append(Path.DirectorySeparatorChar).Append(id.X).Append('x').Append(id.Y);

			return builder.ToString();
		}
	}
}
