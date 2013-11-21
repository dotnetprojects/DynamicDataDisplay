using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
    public class FileSystemTileServer : TileServerBase
    {
        protected sealed override string GetCustomName()
        {
            return "File " + ServerName;
        }

        private string defaultCachePath;

        private CacheLocation cacheLocation = CacheLocation.TempFiles;
        public CacheLocation CacheLocation
        {
            get { return cacheLocation; }
            set
            {
                cacheLocation = value;
                SetCachePath();
            }
        }

        private string fileExtension = ".png";
        /// <summary>
        /// Gets or sets the extension that is added to file while saving and opening.
        /// Should be in format '.xxx', where 'xxx' is an actual extension.
        /// </summary>
        /// <value>The file extension.</value>
        public string FileExtension
        {
            get { return fileExtension; }
            set { fileExtension = value; }
        }

        private string cachePath = null;
        /// <summary>
        /// Gets the full path to the folder that serves as cache of downloaded tiles.
        /// </summary>
        /// <value>The cache path.</value>
        public string CachePath
        {
            get { return cachePath; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("CachePath");
                cachePath = value;
                SetCachePath();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemTileServer"/> class.
        /// </summary>
        public FileSystemTileServer()
        {
            SetCachePath();
        }

        private void SetCachePath()
        {
            string appData = null;
            switch (cacheLocation)
            {
                case CacheLocation.ApplicationFolder:
                    string dllPath = Assembly.GetAssembly(typeof(FileSystemTileServer)).Location;
                    appData = Path.GetDirectoryName(dllPath);
                    break;
                case CacheLocation.ApplicationDataFolder:
                    appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    appData = AppendAssemblyId(appData);
                    break;
                case CacheLocation.TemporaryInternetFiles:
                    var cachePath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                    appData = AppendAssemblyId(cachePath);
                    break;
                case CacheLocation.TempFiles:
                    cachePath = Path.GetTempPath();
                    appData = AppendAssemblyId(cachePath);
                    break;
                case CacheLocation.CustomPath:
                    appData = this.cachePath;
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (cacheLocation != CacheLocation.CustomPath)
                defaultCachePath = Path.Combine(appData, "Cache");
            else
                defaultCachePath = appData;
        }

        private static string AppendAssemblyId(string appData)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            // can be null while loaded for tests execution
            if (entryAssembly == null) return appData;

            var module = entryAssembly.ManifestModule;
            var assemblyNameParts = module.Name.Split('.');
            var assemblyName = assemblyNameParts[0];
            appData = Path.Combine(appData, assemblyName);
#if !DEBUG
			appData = Path.Combine(appData, module.ModuleVersionId.ToString());
#endif
            return appData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemTileServer"/> class.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        public FileSystemTileServer(string serverName)
            : this()
        {
            ServerName = serverName;
        }

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>The name.</value>
        public override string ServerName
        {
            get { return base.ServerName; }
            set
            {
                base.ServerName = value;
                cachePath = Path.Combine(defaultCachePath, value);
            }
        }

        protected virtual string GetNameByIndex(TileIndex index)
        {
            return String.Concat(index.X.ToString(), "x", index.Y.ToString());
        }

        private readonly Dictionary<double, string> zoomDirs = new Dictionary<double, string>();
        protected string GetImagePath(TileIndex index)
        {
            string id = GetNameByIndex(index);

            string zoomDirPath = GetZoomDir(index.Level);
            string imagePath = Path.Combine(zoomDirPath, GetFileName(id));

            return imagePath;
        }

        protected string GetZoomDir(double level)
        {
            if (!zoomDirs.ContainsKey(level))
            {
                if (String.IsNullOrEmpty(cachePath))
                    throw new InvalidOperationException("Name is not assigned.");

                string zoomDirPath = Path.Combine(cachePath, GetDirPath(level));
                zoomDirs[level] = zoomDirPath;
            }

            return zoomDirs[level];
        }

        protected virtual string GetDirPath(double level)
        {
            return "z" + level.ToString();
        }

        protected string GetFileName(string id)
        {
            return id + fileExtension;
        }

        #region ITileServer Members

        private double currentLevel;
        /// <summary>
        /// Contains bool value whether there is image with following tile index.
        /// </summary>
        private Dictionary<TileIndex, bool> fileMap = new Dictionary<TileIndex, bool>(new TileIndex.TileIndexEqualityComparer());

        protected Dictionary<TileIndex, bool> FileMap
        {
            get { return fileMap; }
        }

        private int maxIndicesToPrecache = 1024;
        public int MaxIndicesToPrecache
        {
            get { return maxIndicesToPrecache; }
            set { maxIndicesToPrecache = value; }
        }

        public override bool Contains(TileIndex id)
        {
            // todo probably preload existing images into fileMap.
            // todo probably save previous fileMaps.
            if (id.Level != currentLevel)
            {
                fileMap = new Dictionary<TileIndex, bool>(new TileIndex.TileIndexEqualityComparer());
                currentLevel = id.Level;

                if (MapTileProvider.GetTotalTilesCount(currentLevel) <= maxIndicesToPrecache)
                {
                    Stopwatch timer = Stopwatch.StartNew();
                    var directory = new DirectoryInfo(GetZoomDir(currentLevel));
                    if (directory.Exists)
                    {
                        var files = directory.GetFiles();
                        var fileNames = (from file in files
                                         select file.Name).ToList();
                        fileNames.Sort();

                        var tileInfos = from tile in MapTileProvider.GetTilesForLevel(currentLevel)
                                        let name = GetFileName(GetNameByIndex(tile))
                                        orderby name
                                        select new { Tile = tile, Name = name };

                        foreach (var tileInfo in tileInfos)
                        {
                            fileMap[tileInfo.Tile] = fileNames.Contains(tileInfo.Name);
                        }
                        Debug.WriteLine("Precached directory for level " + currentLevel + ": " + timer.ElapsedMilliseconds + " ms");
                    }
                }
            }

            if (fileMap.ContainsKey(id))
            {
                return fileMap[id];
            }
            else
            {
                string path = GetImagePath(id);
                bool res = File.Exists(path);
                fileMap[id] = res;
                return res;
            }
        }

        public override void BeginLoadImage(TileIndex id)
        {
            string imagePath = GetImagePath(id);

            FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            BeginLoadBitmapImpl(stream, id);
        }

        protected override void OnBmpDownloadCompleted(object sender, EventArgs e)
        {
            base.OnBmpDownloadCompleted(sender, e);

            BitmapImage bmp = (BitmapImage)sender;
            bmp.StreamSource.Dispose();
        }

        protected override void OnBmpDownloadFailed(object sender, ExceptionEventArgs e)
        {
            base.OnBmpDownloadFailed(sender, e);

            BitmapImage bmp = (BitmapImage)sender;
            bmp.StreamSource.Dispose();
        }

        #endregion
    }
}

