using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network
{
	public abstract class SourceTileServer : TileServerBase
	{
		protected SourceTileServer() { }

		private int maxConcurrentDownloads = Int32.MaxValue;
		public int MaxConcurrentDownloads
		{
			get { return maxConcurrentDownloads; }
			protected set { maxConcurrentDownloads = value; }
		}

		private int minLevel = 1;
		public int MinLevel
		{
			get { return minLevel; }
			protected set { minLevel = value; }
		}

		private int maxLevel = 17;
		public int MaxLevel
		{
			get { return maxLevel; }
			protected set { maxLevel = value; }
		}

		private int serversNum = 0;
		public int ServersNum
		{
			get { return serversNum; }
			protected set { serversNum = value; }
		}

		private int minServer = 0;
		public int MinServer
		{
			get { return minServer; }
			protected set
			{
				minServer = value;
				currentServer = value;
			}
		}

		private string fileExtension = ".png";
		public string FileExtension
		{
			get { return fileExtension; }
			protected set { fileExtension = value; }
		}

		private int currentServer = 0;
		protected int CurrentServer
		{
			get { return currentServer; }
			set { currentServer = value; }
		}

		private string userAgent = "Dynamic Data Display";
		public string UserAgent
		{
			get { return userAgent; }
			set { userAgent = value; }
		}

		private string referer = null;
		public string Referer
		{
			get { return referer; }
			set { referer = value; }
		}

		private string uriFormat = null;
		public string UriFormat
		{
			get { return uriFormat; }
			protected set { uriFormat = value; }
		}

		private double maxLatitude = 85;
		public double MaxLatitude
		{
			get { return maxLatitude; }
			protected set { maxLatitude = value; }
		}

		public virtual bool CanLoadFast(TileIndex id)
		{
			return false;
		}

		private int tileWidth = 256;
		public int TileWidth
		{
			get { return tileWidth; }
			protected set { tileWidth = value; }
		}

		private int tileHeight = 256;
		public int TileHeight
		{
			get { return tileHeight; }
			protected set { tileHeight = value; }
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			return obj.GetType() == GetType();
		}


        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        private bool deleteFileCacheOnUpdate = false;
        public bool DeleteFileCacheOnUpdate
        {
            get { return deleteFileCacheOnUpdate; }
            protected set { deleteFileCacheOnUpdate = value; }
        }

        public virtual void CancelRunningOperations() { }
	}
}
