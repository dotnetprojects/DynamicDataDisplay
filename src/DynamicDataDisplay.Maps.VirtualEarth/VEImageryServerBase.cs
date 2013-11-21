using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.ServiceModel;
using System.ComponentModel;
using System.Net;
using System.Diagnostics;
using VE.VESvc;

namespace Microsoft.Research.DynamicDataDisplay.Maps
{
	/// <summary>
	/// Represents a VirtualEarth Imagery Service tile server.
	/// </summary>
	public abstract class VEImageryServerBase : NetworkTileServer, ISupportInitialize
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VEImageryServerBase"/> class.
		/// </summary>
		protected VEImageryServerBase()
		{
			ServerName = "Virtual Earth Imagery ";
			//MaxLatitude = 85.051128;
			MaxLatitude = 85.28799;
		}

		private string applicationId = "";
		/// <summary>
		/// Gets or sets VirtualEarth application id.
		/// </summary>
		/// <value>The application id.</value>
		public string ApplicationId
		{
			get { return applicationId; }
			set { applicationId = value; }
		}

		private string token = "";
		/// <summary>
		/// Gets or sets VirtualEarth token.
		/// </summary>
		/// <value>The token.</value>
		public string Token
		{
			get { return token; }
			set { token = value; }
		}

		private bool loaded = false;
		private void client_GetImageryMetadataCompleted(object sender, GetImageryMetadataCompletedEventArgs e)
		{
			if (e.Error == null)
			{
				loaded = true;

				ImageryMetadataResponse metadataResponse = (ImageryMetadataResponse)e.Result;
				ImageryMetadataResult result = metadataResponse.Results[0];

				servers = result.ImageUriSubdomains;
				ServersNum = servers.Length;

				TileWidth = result.ImageSize.Width;
				TileHeight = result.ImageSize.Height;

				UriFormat = result.ImageUri.Replace("{culture}", "en-us").Replace("{token}", token).Replace("{subdomain}", "{0}").Replace("{quadkey}", "{1}");

				MinLevel = result.ZoomRange.From;
				MaxLevel = result.ZoomRange.To;

				RaiseChangedEvent();
			}
			else if (e.Error is EndpointNotFoundException)
			{
				Debug.WriteLine(ServerName + ": error occured during loading: " + e.Error.Message);
			}
			else
			{
				throw new InvalidOperationException(VE.Properties.Resources.VEServerCannotLoadTile, e.Error);
			}
		}

		public override bool Contains(TileIndex id)
		{
			if (!metadataRequestSent)
			{
				SendMetadataRequest();
			}
			return loaded && base.Contains(id);
		}

		private string[] servers;
		protected override string CreateRequestUriCore(TileIndex index)
		{
			if (!loaded || String.IsNullOrEmpty(UriFormat))
				throw new InvalidOperationException(VE.Properties.Resources.VEServerCannotLoadTile);

			string indexString = CreateTileIndexString(index);
			string currentServerName = servers[CurrentServer];

			string result = String.Format(UriFormat, currentServerName, indexString);
			return result;
		}

		protected override bool IsGoodTileResponse(WebResponse response)
		{
			if (response.Headers.AllKeys.Contains("X-VE-Tile-Info"))
			{
				return false;
			}
			return true;
		}

		private string CreateTileIndexString(TileIndex index)
		{
			StringBuilder builder = new StringBuilder();

			checked
			{
				for (int level = MinLevel; level <= index.Level; level++)
				{
					char ch = '0';
					int halfTilesNum = (int)Math.Pow(2, index.Level - level);
					if ((index.X & halfTilesNum) != 0)
						ch += (char)1;
					if ((index.Y & halfTilesNum) == 0)
						ch += (char)2;
					builder.Append(ch);
				}
			}

			return builder.ToString();
		}

		#region ISupportInitialize Members

		public void BeginInit()
		{
		}

		private bool metadataRequestSent = false;
		public void EndInit()
		{
			SendMetadataRequest();
		}

		private MapStyle mapStyle = MapStyle.Road;
		public MapStyle MapStyle
		{
			get { return mapStyle; }
			protected set { mapStyle = value; }
		}

		private void SendMetadataRequest()
		{
			var address = VE.Properties.Settings.Default.ServiceUriDev;

			using (ImageryServiceClient client = new ImageryServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport), new EndpointAddress(address)))
			{
				client.GetImageryMetadataCompleted += client_GetImageryMetadataCompleted;

				ImageryMetadataRequest request = new ImageryMetadataRequest();
				request.ExecutionOptions = new ExecutionOptions { SuppressFaults = false };
				request.Credentials = new Credentials { ApplicationId = applicationId, Token = token };
				request.Culture = "en-us";
				request.Style = mapStyle;

				client.GetImageryMetadataAsync(request, null);

				metadataRequestSent = true;
			}
		}

		#endregion
	}
}
