using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.IO;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	public partial class Map
	{
		private void OnOpenCache(object sender, RoutedEventArgs e)
		{
			var fileTileServer = FileTileServer as FileSystemTileServer;
			if (fileTileServer != null)
			{
				var cachePath = fileTileServer.CachePath;
				while (!Directory.Exists(cachePath))
				{
					var lastDirIndex = cachePath.LastIndexOf(Path.DirectorySeparatorChar);
					if (lastDirIndex > -1)
						cachePath = cachePath.Substring(0, lastDirIndex);
				}
				Process.Start(cachePath);
			}
		}

		private void OnClearCache(object sender, RoutedEventArgs e)
		{
			var fileTileServer = FileTileServer as FileSystemTileServer;
			if (fileTileServer != null)
			{
				try
				{
					DirectoryInfo cacheDirectory = new DirectoryInfo(fileTileServer.CachePath);
					if (cacheDirectory.Exists)
						cacheDirectory.Delete(true);
				}
				catch (Exception exc)
				{
					Debug.WriteLine("Exception while deleting of file cache directory: " + exc.Message);
				}
			}
		}

		private void OnDeleteCaches(object sender, RoutedEventArgs e)
		{
			var fileTileServer = FileTileServer as FileSystemTileServer;
			if (fileTileServer != null)
			{
				try
				{
					DirectoryInfo cacheDirectory = new DirectoryInfo(fileTileServer.CachePath);
					cacheDirectory.Parent.Delete(true);
				}
				catch (Exception exc)
				{
					Debug.WriteLine("Exception while deleting of all file cache directories: " + exc.Message);
				}
			}
		}

		private void OnClearMemoryCache(object sender, RoutedEventArgs e)
		{
			var memoryTileServer = TileSystem.MemoryServer as LRUMemoryCache;
			if (memoryTileServer != null)
			{
				memoryTileServer.Clear();
			}
		}
	}
}
