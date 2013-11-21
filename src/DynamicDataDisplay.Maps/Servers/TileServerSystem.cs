using System;
using System.Windows.Media.Imaging;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.IO;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
    public sealed class TileServerSystem : DispatcherObject, ITileSystem
    {
        public TileServerSystem() { SourceServer = new EmptyTileServer(); }
        public TileServerSystem(SourceTileServer sourceServer)
        {
            SourceServer = sourceServer;
        }

        public string ServerName
        {
            get { return sourceServer != null ? sourceServer.ServerName : "not set"; }
        }

        private void memoryServer_ImageLoaded(object sender, TileLoadResultEventArgs e)
        {
            pendingImages.Remove(e.ID);
        }

        private void fileServer_ImageLoaded(object sender, TileLoadResultEventArgs e)
        {
            pendingImages.Remove(e.ID);

            if (e.Result == TileLoadResult.Success)
            {
                memoryServer.BeginSaveImage(e.ID, e.Image, e.Stream);
            }

            ImageLoaded.Raise(this, e);
        }

        TileIndex? latestFailuredId = null;
        private void sourceServer_ImageLoaded(object sender, TileLoadResultEventArgs e)
        {
            pendingImages.Remove(e.ID);

            bool saveToFileCache = !sourceServer.CanLoadFast(e.ID) && saveToCache;
            if (saveToFileCache && e.Result == TileLoadResult.Success)
            {
                BeginSaveImage(e.ID, e.Image, e.Stream);
            }
            if (e.Result == TileLoadResult.Success)
            {
                memoryServer.BeginSaveImage(e.ID, e.Image, e.Stream);
            }
            else
            {
                latestFailuredId = e.ID;
            }
            ImageLoaded.Raise(this, e);
        }

        private readonly Set<TileIndex> pendingImages = new Set<TileIndex>();

        private SourceTileServer sourceServer;
        public SourceTileServer SourceServer
        {
            get { return sourceServer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (sourceServer == null || !sourceServer.Equals(value))
                {
                    if (sourceServer != null)
                    {
                        sourceServer.ImageLoaded -= sourceServer_ImageLoaded;
                        sourceServer.Changed -= sourceServer_Changed;
                    }

                    var prevServer = sourceServer;
                    sourceServer = value;
                    sourceServer.ImageLoaded += sourceServer_ImageLoaded;
                    sourceServer.Changed += sourceServer_Changed;
                    CreateServers();

                    SourceServerChanged.Raise(this, prevServer, sourceServer);
                }
            }
        }

        private void sourceServer_Changed(object sender, EventArgs e)
        {
            memoryServer.Clear();

            if (sourceServer.DeleteFileCacheOnUpdate)
            {
                fileServer.Clear();
            }

            Changed.Raise(this);
            SourceServerChanged.Raise(this, sourceServer, sourceServer);
        }

        public event EventHandler<ValueChangedEventArgs<SourceTileServer>> SourceServerChanged;

        private void CreateServers()
        {
            DetachPreviousServers();
            pendingImages.Clear();

            if (sourceServer != null)
            {
                //if (this.fileServer == null || (this.fileServer != null && String.IsNullOrEmpty(this.fileServer.ServerName)))
                //{
                var fileServer = new AsyncFileSystemServer(ServerName);
                fileServer.FileExtension = sourceServer.FileExtension;

                this.fileServer = fileServer;
                this.fileServer.ImageLoaded += fileServer_ImageLoaded;
                //}

                memoryServer = new LRUMemoryCache(ServerName);
                memoryServer.ImageLoaded += memoryServer_ImageLoaded;
            }
        }

        private void DetachPreviousServers()
        {
            DetachFileServer();

            if (memoryServer != null)
            {
                memoryServer.ImageLoaded -= memoryServer_ImageLoaded;
                memoryServer = null;

                //if (memoryServer is IDisposable)
                //{
                //    ((IDisposable)memoryServer).Dispose();
                //}
            }
        }

        private void DetachFileServer()
        {
            if (fileServer != null)
            {
                fileServer.ImageLoaded -= fileServer_ImageLoaded;

                if (fileServer is IDisposable)
                {
                    ((IDisposable)fileServer).Dispose();
                }

                fileServer = null;
            }
        }

        private IWriteableTileServer fileServer = null;
        public IWriteableTileServer FileServer
        {
            get { return fileServer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (fileServer != value)
                {
                    DetachFileServer();
                    fileServer = value;
                    fileServer.ImageLoaded += fileServer_ImageLoaded;
                }
            }
        }

        private ITileSystem memoryServer = null;
        public ITileSystem MemoryServer
        {
            get { return memoryServer; }
			set { memoryServer = value; }
        }

        private TileSystemMode mode = TileSystemMode.OnlineAndCache;
        /// <summary>
        /// Gets or sets the mode of network access of <see cref="TileServerSystem"/>.
        /// </summary>
        /// <remarks>Default value is <see cref="TileSystemMode.OnlineAndCache"/>.</remarks>
        /// <value>The mode.</value>
        public TileSystemMode Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    mode = value;
                    ModeChanged.Raise(this, new ModeChangedEventArgs(value));
                }
            }
        }

        /// <summary>
        /// Occurs when mode of network access changes.
        /// </summary>
        public event EventHandler<ModeChangedEventArgs> ModeChanged;

        private bool saveToCache = true;
        public bool SaveToCache
        {
            get { return saveToCache; }
            set { saveToCache = value; }
        }

        #region ITileServer Members

        public bool IsLoaded(TileIndex id)
        {
            return memoryServer.Contains(id);
        }

        public BitmapSource this[TileIndex id]
        {
            get { return memoryServer[id]; }
        }

        public bool Contains(TileIndex id)
        {
            switch (mode)
            {
                case TileSystemMode.OnlineOnly:
                    return true;
                case TileSystemMode.OnlineAndCache:
                    return true;
                case TileSystemMode.CacheOnly:
                    return memoryServer.Contains(id) ||
                        fileServer.Contains(id) ||
                        sourceServer.CanLoadFast(id) && sourceServer.Contains(id);
                default:
                    throw new InvalidOperationException();
            }
        }

        private bool allowFastNetworkLoad = true;
        public bool AllowFastNetworkLoad
        {
            get { return allowFastNetworkLoad; }
            set { allowFastNetworkLoad = value; }
        }

        public void BeginLoadImage(TileIndex id)
        {
            if (pendingImages.Contains(id))
                return;

            bool beganLoading = false;
            if (memoryServer.Contains(id))
            {
                // do nothing
            }
            else
            {
                if (allowFastNetworkLoad && sourceServer.CanLoadFast(id))
                {
                    sourceServer.BeginLoadImage(id);
                    beganLoading = true;
                }
                else
                {
                    switch (mode)
                    {
                        case TileSystemMode.OnlineOnly:
                            if (sourceServer.Contains(id))
                            {
                                sourceServer.BeginLoadImage(id);
                                beganLoading = true;
                            }
                            break;
                        case TileSystemMode.OnlineAndCache:
                            if (fileServer.Contains(id))
                            {
                                fileServer.BeginLoadImage(id);
                                beganLoading = true;
                            }
                            else if (sourceServer.Contains(id))
                            {
                                sourceServer.BeginLoadImage(id);
                                beganLoading = true;
                            }
                            break;
                        case TileSystemMode.CacheOnly:
                            if (fileServer.Contains(id))
                            {
                                fileServer.BeginLoadImage(id);
                                beganLoading = true;
                            }
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                if (beganLoading)
                {
                    bool wasLatestError = latestFailuredId != null && latestFailuredId.Value.Equals(id);
                    if (!memoryServer.Contains(id) && !wasLatestError)
                    {
                        pendingImages.Add(id);
                    }
                }
            }

            latestFailuredId = null;
        }

        public event EventHandler<TileLoadResultEventArgs> ImageLoaded;

        #endregion

        #region ITileStore Members

        public void BeginSaveImage(TileIndex id, BitmapSource image, Stream stream)
        {
            fileServer.BeginSaveImage(id, image, stream);
        }

        #endregion

        #region ITileServer Members

        private void RaiseChangedEvent()
        {
            Changed.Raise(this);
        }
        /// <summary>
        /// Occurs when <see cref="TileServerSystem"/> changes.
        /// </summary>
        public event EventHandler Changed;

        #endregion

        #region ITileStore Members

        public void Clear()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Enumerates network access modes of <see cref="TileServerSystem"/>.
    /// </summary>
    public enum TileSystemMode
    {
        /// <summary>
        /// <see cref="TileServerSystem"/> does not use file system cache to store downloaded tile images and loads new tiles only from <see cref="TileServerSystem.NetworkServer"/>.
        /// If NetworkServer is unaccessible, no tiles are shown.
        /// </summary>
        OnlineOnly,
        /// <summary>
        /// <see cref="TileServerSystem"/> uses both NetworkServer and FileServer to load tile images from. FileServer is used to store downloaded tile images.
        /// </summary>
        OnlineAndCache,
        /// <summary>
        /// <see cref="TileServerSystem"/> does not use <see cref="TileServerSystem.NetworkServer"/> to download tiles, it shows only tiles that were previously downloaded and 
        /// stored to <see cref="TileServerSystem.FileServer"/>.
        /// </summary>
        CacheOnly
    }
}
