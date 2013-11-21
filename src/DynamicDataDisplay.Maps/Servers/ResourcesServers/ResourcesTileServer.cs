using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers
{
	public abstract class ResourcesTileServer : ITileServer
	{
		private const string prefix = "Microsoft.Research.DynamicDataDisplay.Maps.Tiles";

		private static readonly Assembly currentAssembly;
		private static readonly string[] resourceNames;
		static ResourcesTileServer()
		{
			currentAssembly = typeof(ResourcesTileServer).Assembly;
			resourceNames = currentAssembly.GetManifestResourceNames();
		}

		private int minLevel = 1;
		private int maxLevel = 2;

		#region ITileServer Members

		public bool Contains(TileIndex id)
		{
			return (minLevel <= id.Level && id.Level <= maxLevel);
		}

		private string currentPrefix;
		public void BeginLoadImage(TileIndex id)
		{
			BitmapImage bmp = new BitmapImage();
			string resourceName = CreateResourceName(id);
			using (Stream stream = currentAssembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
				{
					ReportFailure(id);
					return;
				}

				bmp.BeginInit();
				bmp.StreamSource = stream;
				bmp.CacheOption = BitmapCacheOption.OnLoad;
				bmp.EndInit();
				if (bmp.IsDownloading) { }
			}

			ReportSuccess(bmp, id);
		}

		protected void ReportSuccess(BitmapImage bmp, TileIndex id)
		{
			Debug.WriteLine(String.Format("{0}: loaded id = {1}", ServerName, id));
			RaiseDataLoaded(new TileLoadResultEventArgs
			{
				Image = bmp,
				Result = TileLoadResult.Success,
				ID = id
			});
		}

		protected void ReportFailure(TileIndex id)
		{
			Debug.WriteLine(String.Format("{0}: failed id = {1}", ServerName, id));
			RaiseDataLoaded(new TileLoadResultEventArgs
			{
				Image = null,
				ID = id,
				Result = TileLoadResult.Failure
			});
		}


		private string CreateResourceName(TileIndex id)
		{
			StringBuilder builder = new StringBuilder(currentPrefix);
			builder = builder.Append(id.Level).Append(".").Append(id.X).Append("x")
				.Append(id.Y).Append(fileExtension);

			return builder.ToString();
		}

		private void RaiseDataLoaded(TileLoadResultEventArgs args)
		{
			ImageLoaded.Raise(this, args);
		}
		public event EventHandler<TileLoadResultEventArgs> ImageLoaded;

		private string name = "Resource tile server";
		public string ServerName
		{
			get { return name; }
			protected set
			{
				name = value;
				currentPrefix = prefix + "." + name + ".z";
			}
		}

		private string fileExtension = ".png";
		public string FileExtension
		{
			get { return fileExtension; }
			set { fileExtension = value; }
		}

		#endregion

		#region ITileServer Members

		protected void RaiseChangedEvent()
		{
			Changed.Raise(this);
		}
		public event EventHandler Changed;

		#endregion
	}
}
