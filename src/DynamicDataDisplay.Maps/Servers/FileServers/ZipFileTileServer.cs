using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.IO.Packaging;
using System.IO;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.FileServers;
using System.Net.Mime;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	public class ZipFileTileServer : ReadonlyTileServer, IDisposable, ISupportInitialize
	{
		public void CreatePackage(string sourceDirectory, string packagePath)
		{
			var foundTiles = pathProvider.GetTiles(sourceDirectory, extension);

			foreach (var foundTile in foundTiles)
			{
				Cache.Add(foundTile.ID, true);
			}

			string shortExtension = "image/" + extension.TrimStart('.');

			const int flushCnt = 20;
			int counter = 0;
			using (Package package = Package.Open(packagePath, FileMode.Create))
			{
				WriteCache(package);

				foreach (var foundTile in foundTiles)
				{
					Uri partUri = PackUriHelper.CreatePartUri(new Uri(foundTile.Path, UriKind.Relative));
					PackagePart part = package.CreatePart(partUri, shortExtension, CompressionOption.Fast);

					using (FileStream fs = new FileStream(Path.Combine(sourceDirectory, foundTile.Path), FileMode.Open, FileAccess.Read))
					{
						CopyStream(fs, part.GetStream());
					}

					package.CreateRelationship(part.Uri, TargetMode.Internal, D3AssemblyConstants.DefaultXmlNamespace);

					counter++;
					if (counter % flushCnt == 0)
					{
						counter = 0;
						package.Flush();
					}

					Console.WriteLine(foundTile.Path);
				}
			}
		}

		const string cacheName = "cache.dat";
		private void WriteCache(Package package)
		{
			Uri cacheUri = GetPartUri(cacheName);
			PackagePart part = package.CreatePart(cacheUri, MediaTypeNames.Application.Octet, CompressionOption.Fast);

			BinaryFormatter formatter = new BinaryFormatter();
			using (Stream stream = part.GetStream())
			{
				formatter.Serialize(stream, Cache);
			}

			package.CreateRelationship(cacheUri, TargetMode.Internal, D3AssemblyConstants.DefaultXmlNamespace);
		}

		private static void CopyStream(Stream source, Stream target)
		{
			const int bufSize = 0x1000;
			byte[] buf = new byte[bufSize];
			int bytesRead = 0;
			while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
				target.Write(buf, 0, bytesRead);
		}

		public override bool Contains(TileIndex id)
		{
			string path = pathProvider.GetTilePath(id) + extension;
			Uri uri = GetPartUri(path);

			bool exists = package.PartExists(uri);
			return exists;
		}

		public ZipFileTileServer() { }

		private string packagePath;
		public string PackagePath
		{
			get { return packagePath; }
			set { packagePath = value; }
		}

		private Package package;
		public ZipFileTileServer(string packagePath)
		{
			this.packagePath = packagePath;

			EndInit();
		}

		private Uri GetPartUri(string uri)
		{
			return PackUriHelper.CreatePartUri(new Uri(uri, UriKind.Relative));
		}

		private TilePathProvider pathProvider = new DefaultPathProvider();
		public TilePathProvider PathProvider
		{
			get { return pathProvider; }
			set { pathProvider = value; }
		}

		private string extension = ".png";
		public string Extension
		{
			get { return extension; }
			set { extension = value; }
		}

		public override void BeginLoadImage(TileIndex id)
		{
			string fileName = pathProvider.GetTilePath(id) + extension;

			Uri partUri = PackUriHelper.CreatePartUri(new Uri(fileName, UriKind.Relative));
			PackagePart part = package.GetPart(partUri);

			BeginLoadBitmapImpl(part.GetStream(FileMode.Open, FileAccess.Read), id);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (package != null)
			{
				((IDisposable)package).Dispose();
			}
		}

		#endregion

		#region ISupportInitialize Members

		public void BeginInit()
		{
		}

		public void EndInit()
		{
			package = Package.Open(packagePath, FileMode.Open, FileAccess.Read);

			BinaryFormatter formatter = new BinaryFormatter();

			PackagePart cachePart = package.GetPart(GetPartUri(cacheName));
			using (Stream stream = cachePart.GetStream(FileMode.Open, FileAccess.Read))
			{
				Cache = (ReadonlyTileCache)formatter.Deserialize(stream);
			}
		}

		#endregion
	}
}
