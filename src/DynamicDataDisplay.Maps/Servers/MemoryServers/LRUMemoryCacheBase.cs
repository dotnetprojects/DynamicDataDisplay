using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Media;
using System.IO;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers
{
	public class LRUMemoryCacheBase<T> : TileServerBase, ITileSystem where T : class
	{
		private readonly Dictionary<TileIndex, object> cache = new Dictionary<TileIndex, object>(new TileIndex.TileIndexEqualityComparer());
		private readonly Dictionary<TileIndex, int> accessTimes = new Dictionary<TileIndex, int>(new TileIndex.TileIndexEqualityComparer());

		private readonly DispatcherTimer cleanupTimer;
		private int accessTime;

		public LRUMemoryCacheBase(string name)
		{
			cleanupTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(20) };
			cleanupTimer.Tick += cleanupTimer_Tick;

			this.ServerName = name;
		}

		protected override string GetCustomName()
		{
			return "LRU Memory " + base.GetCustomName();
		}

		public Dictionary<TileIndex, object>.KeyCollection GetCachedIndexes()
		{
			return cache.Keys;
		}

		public bool CleanupOnTimer
		{
			get { return cleanupTimer.IsEnabled; }
			set { cleanupTimer.IsEnabled = value; }
		}

		public int CacheSize
		{
			get { return cache.Count; }
		}

		private void cleanupTimer_Tick(object sender, EventArgs e)
		{
			PerformCleanup(true);
		}

		private int timeAfterAccessToCollect = 2000; // milliseconds
		public int TimeAfterAccessToCollect
		{
			get { return timeAfterAccessToCollect; }
			set { timeAfterAccessToCollect = value; }
		}

		private int maxCacheSize = 80;
		public int MaxCacheSize
		{
			get { return maxCacheSize; }
			set
			{
				if (maxCacheSize != value)
				{
					if (value <= 0)
						throw new ArgumentOutOfRangeException("value", "Value should be positive.");

					int prevMaxCacheSize = maxCacheSize;
					maxCacheSize = value;

					if (maxCacheSize < prevMaxCacheSize)
					{
						PerformCleanup(false);
					}
				}
			}
		}

		private int cleanupCalls = 0;

		private int cleanumSkipNum = 5;
		public int CleanumSkipNum
		{
			get { return cleanumSkipNum; }
			set { cleanumSkipNum = value; }
		}

		private int maxLevelToStoreAlways = 1;
		public int MaxLevelToStoreAlways
		{
			get { return maxLevelToStoreAlways; }
			set { maxLevelToStoreAlways = value; }
		}

		private void PerformCleanup(bool calledOnTimer)
		{
			if (cache.Keys.Count <= maxCacheSize)
			{
				cleanupTimer.IsEnabled = false;
				return;
			}

			if (!calledOnTimer && (cleanupCalls++ >= cleanumSkipNum))
			{
				cleanupCalls = 0;
				return;
			}

			var sortedAccessTimes = accessTimes.OrderBy(pair => pair.Value).ToList();

			int extraNum = sortedAccessTimes.Count - maxCacheSize;

			//MapsTraceSource.Instance.ServerInformationTraceSource.TraceInformation("LRUMemoryCache: CleanUp - " + extraNum + " tiles over limit of " + maxCacheSize);

			for (int i = 0; i < extraNum; i++)
			{
				var pair = sortedAccessTimes[i];

				int tileAccessTime = pair.Value;

				// do not delete tiles that were accessed at last time not very early before latest global
				// access
				if (accessTime - tileAccessTime < timeAfterAccessToCollect)
					break;

				object img = cache[pair.Key];
				if (img is BitmapSource && pair.Key.Level > maxLevelToStoreAlways)
				{
					cache[pair.Key] = new WeakReference(img);
				}
				else if (img is WeakReference)
				{
					WeakReference reference = img as WeakReference;
					if (!reference.IsAlive)
					{
						if (pair.Key.Level == 1) { }

						cache.Remove(pair.Key);
						accessTimes.Remove(pair.Key);
					}
				}
			}
		}

		public sealed override bool Contains(TileIndex id)
		{
			object img;
			if (cache.TryGetValue(id, out img))
			{
				if (img is T) return true;
				else
				{
					WeakReference reference = img as WeakReference;
					return reference.IsAlive;
				}
			}

			return false;
		}

		public override void BeginLoadImage(TileIndex id) { }

		public void Clear()
		{
			cache.Clear();
			accessTimes.Clear();
			cleanupCalls = 0;
		}

		#region IDirectAccessTileServer Members

		public T GetTile(TileIndex id)
		{
			object result;

			T img = null;
			if (cache.TryGetValue(id, out result))
			{
				img = result as T;
				if (img != null) { }
				else
				{
					WeakReference reference = result as WeakReference;
					if (reference != null && reference.IsAlive)
					{
						img = reference.Target as T;
						// changing weak reference to strong
						cache[id] = img;
					}
				}

				// update access time
				if (img != null)
				{
					accessTime = Environment.TickCount;
					accessTimes[id] = accessTime;
				}
				else
				{
					Debug.WriteLine("Cache miss due to GC.");

					cache.Remove(id);
					accessTimes.Remove(id);
				}
			}

			return img;
		}

		public BitmapSource this[TileIndex id]
		{
			get
			{
				return typeof(T).IsAssignableFrom(typeof(BitmapSource)) ? (BitmapSource)(object)GetTile(id) : null;
			}
		}

		#endregion

		#region ITileStore Members

		public virtual void BeginSaveImage(TileIndex id, BitmapSource image, Stream stream)
		{
			if (id.Level == 1) { }

			cache[id] = TransformImageAndStream(id, image, stream);
			accessTimes[id] = Environment.TickCount;

			if (Statistics.IntValues["MaxTilesCount"] < cache.Count)
				Statistics.IntValues["MaxTilesCount"] = cache.Count;

			if (cache.Keys.Count > maxCacheSize)
			{
				cleanupTimer.IsEnabled = true;
				PerformCleanup(false);
			}
		}

		public virtual object TransformImageAndStream(TileIndex id, BitmapSource image, Stream stream)
		{
			return image;
		}

		#endregion
	}
}
