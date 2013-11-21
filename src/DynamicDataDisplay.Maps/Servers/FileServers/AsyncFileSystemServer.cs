using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Threading.Collections;
using System.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	public class AsyncFileSystemServer : WriteableFileSystemTileServer
	{
		public AsyncFileSystemServer() : base() { }

		public AsyncFileSystemServer(string name)
			: base(name)
		{
			corruptedFilesDeleteTimer.Tick += corruptedFilesDeleteTimer_Tick;
		}

		// todo is this neccessary?
		private int maxParallelRequests = Int32.MaxValue;
		public int MaxParallelRequests
		{
			get { return maxParallelRequests; }
			set { maxParallelRequests = value; }
		}

		private bool CanRunNextRequest
		{
			get { return runningRequests <= maxParallelRequests; }
		}

		private int runningRequests = 0;
		private readonly ConcurrentStack<TileIndex> requests = new ConcurrentStack<TileIndex>();
		protected ConcurrentStack<TileIndex> Requests
		{
			get { return requests; }
		}

		public override void BeginLoadImage(TileIndex id)
		{
			if (ImageLoadedHandler == null) { }

			string imagePath = GetImagePath(id);

			if (CanRunNextRequest)
			{
				runningRequests++;
				Statistics.IntValues["ImagesLoaded"]++;

				Task loadTileTask = Task.Create((unused) =>
				{
					var bmp = BeginLoadImageAsync(id);
					var stream = BeginLoadStreamAsync(id);
					OnImageLoadedAsync(id, bmp, stream);
				}).WithExceptionThrowingInDispatcher(Dispatcher);
			}
			else
			{
				requests.Push(id);
			}
		}

		private Stream BeginLoadStreamAsync(TileIndex id)
		{
			string imagePath = GetImagePath(id);

			return new FileStream(imagePath, FileMode.Open, FileAccess.Read);
		}

		private readonly DispatcherTimer corruptedFilesDeleteTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(2000)
		};
		private readonly List<Action> fileDeleteActions = new List<Action>();

		private void corruptedFilesDeleteTimer_Tick(object sender, EventArgs e)
		{
			Debug.WriteLine("Deleting files: " + Environment.TickCount);

			// todo is this is necessary?
			// GC is called to collect partly loaded corrupted bitmap
			// otherwise it prevented from image file being deleted.
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			foreach (var action in fileDeleteActions)
			{
				action.BeginInvoke(null, null);
			}
			fileDeleteActions.Clear();
			corruptedFilesDeleteTimer.Stop();
		}

		protected BitmapImage BeginLoadImageAsync(TileIndex id)
		{
			string imagePath = GetImagePath(id);

			BitmapImage bmp = new BitmapImage();
			bmp.BeginInit();
			bmp.CacheOption = BitmapCacheOption.OnLoad;
			bmp.UriSource = new Uri(imagePath);
			try
			{
				bmp.EndInit();
			}
			catch (NotSupportedException exc)
			{
				Debug.WriteLine(String.Format("{0}: failed id = {1}. Exc = \"{2}\"", GetCustomName(), id, exc.Message));

				Action corruptedFileDeleteAction = () =>
				{
					try
					{
						File.Delete(imagePath);
					}
					catch (Exception e)
					{
						Debug.WriteLine("Exception while deleting corrupted image file \"" + imagePath + "\": " + e.Message);
					}
				};

				lock (fileDeleteActions)
				{
					fileDeleteActions.Add(corruptedFileDeleteAction);
					if (!corruptedFilesDeleteTimer.IsEnabled)
						corruptedFilesDeleteTimer.Start();
				}

				return null;
			}
			catch (FileNotFoundException exc)
			{
				Debug.WriteLine(String.Format("{0}: failed id = {1}. Exception = \"{2}\"", GetCustomName(), id, exc.Message));
				return null;
			}
			catch (DirectoryNotFoundException exc)
			{
				Debug.WriteLine(String.Format("{0}: failed id = {1}, Exception = \"{2}\"", GetCustomName(), id, exc.Message));
				return null;
			}

			return (BitmapImage)bmp.GetAsFrozen();
		}

		private void OnImageLoadedAsync(TileIndex id, BitmapImage bmp, Stream stream)
		{
			Dispatcher.BeginInvoke(() =>
			{
				runningRequests--;
				if (bmp != null)
				{
					ReportSuccess(stream, bmp, id);
				}
				else
				{
					ReportFailure(id);
				}
				if (CanRunNextRequest)
				{
					TileIndex nextId;
					if (requests.TryPop(out nextId))
					{
						BeginLoadImage(nextId);
					}
				}
			}, DispatcherPriority.Background);
		}
	}
}
