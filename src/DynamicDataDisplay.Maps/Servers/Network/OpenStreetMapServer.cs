using System;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	/// <summary>
	/// Sample network tile server, which downloads tile images from OpenStreetMap server.
	/// <remarks>
	/// OpenStreetMap - http://www.openstreetmap.org/
	/// Used here by permission of OpenStreetMap.
	/// To use this sample server in your applications, you should read, agree and follow to
	/// OpenStreetMap license.
	/// Do not use this server too much - do not create high load on OpenStreetMap servers.
	/// OpenStreetMap tile isage policy - http://wiki.openstreetmap.org/wiki/Tile_usage_policy
	/// </remarks>
	/// </summary>
	public sealed class OpenStreetMapServer : NetworkTileServer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenStreetMapServer"/> class.
		/// </summary>
		public OpenStreetMapServer()
		{
			UriFormat = "http://{3}.{4}/{0}/{1}/{2}.png";
			ServerName = "Open Street Maps";

            MaxLatitude = 85.2878;
			MinLevel = 0;
			MaxLevel = 17;
			MaxConcurrentDownloads = 3;
			ServersNum = 3;
			//todo determine max level
		}

		private OpenStreetMapRenderer renderer = OpenStreetMapRenderer.Mapnik;
		public OpenStreetMapRenderer Renderer
		{
			get { return renderer; }
			set
			{
				renderer = value;
				RaiseChangedEvent();
			}
		}
		
        protected override string CreateRequestUriCore(TileIndex index)
		{
			int z = (int)index.Level;
			int shift = MapTileProvider.GetSideTilesCount(z) / 2;
			int x = index.X + shift;
			int y = MapTileProvider.GetSideTilesCount(z) - 1 - index.Y - shift;

			char serverIdx = (char)('a' + CurrentServer);
			string uri = "";
			switch (renderer)
			{
				case OpenStreetMapRenderer.Mapnik:
					uri = "tile.openstreetmap.org";
					break;
				case OpenStreetMapRenderer.Osmarenderer:
					uri = "tah.openstreetmap.org/Tiles/tile";
					break;
				case OpenStreetMapRenderer.CycleMap:
					uri = "andy.sandbox.cloudmade.com/tiles/cycle";
					break;
				case OpenStreetMapRenderer.NoName:
					uri = "tile.cloudmade.com/fd093e52f0965d46bb1c6c6281022199/3/256";
					break;
				default:
					break;
			}

			return String.Format(UriFormat, z.ToString(), x.ToString(), y.ToString(), serverIdx.ToString(), uri);
		}
	}
}
