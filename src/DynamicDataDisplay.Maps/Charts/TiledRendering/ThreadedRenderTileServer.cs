using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.Threading;
using System.Threading.Collections;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.TiledRendering
{
	public class ThreadedRenderTileServer : SourceTileServer, IRenderingTileServer
	{
		private Thread[] renderingThreads;
		private CapturingPlotter[] plotters;
		private bool[] isRendering;
		private BlockingCollection<TileIndex> waitingIndexes;
		private Size tileSize = new Size(256, 512);

		public ThreadedRenderTileServer()
		{
			waitingIndexes = new BlockingCollection<TileIndex>();

			TileWidth = (int)tileSize.Width;
			TileHeight = (int)tileSize.Height;
			ServerName = "Render";

			DeleteFileCacheOnUpdate = true;

			MinLevel = Int32.MinValue;
			MaxLevel = Int32.MaxValue;
		}

		private int threadsCount = 1;
		public int ThreadsCount
		{
			get { return threadsCount; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("ThreadsCount", "Number of rendering threads should be positive.");

				if (value > 100)
					throw new ArgumentOutOfRangeException("ThreadsCount", "Number of rendering threads should be less than 100.");

				if (renderingThreads != null)
					throw new InvalidOperationException("Cannot change number of threads after it has been already set");

				threadsCount = value;
				CreateThreads();
			}
		}

		private Func<IPlotterElement> childCreateHandler;
		public Func<IPlotterElement> ChildCreateHandler
		{
			get { return childCreateHandler; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("ChildCreateHandler");

				childCreateHandler = value;

				if (createdPlottersCount == threadsCount)
				{
					for (int i = 0; i < plotters.Length; i++)
					{
						var plotter = plotters[i];
						plotter.Dispatcher.BeginInvoke(() =>
						{
							AddChildToPlotter(plotter);
						}, DispatcherPriority.Send);
					}
				}
			}
		}

		private void AddChildToPlotter(CapturingPlotter plotter)
		{
			if (plotter.Children.Count == 0 && childCreateHandler != null)
			{
				int plotterIndex = Array.IndexOf<CapturingPlotter>(plotters, plotter);


				var child = childCreateHandler();
				if (plotterIndex == 0)
				{
					FrameworkElement element = (FrameworkElement)child;
					element.AddHandler(Viewport2D.ContentBoundsChangedEvent, new RoutedEventHandler(OnChildContentBoundsChanged));
					element.AddHandler(BackgroundRenderer.UpdateRequested, new RoutedEventHandler(OnChildUpdateRequested));
				}
				plotter.Children.Add(child);
			}
		}

		private void EnsureRenderingThreads()
		{
			if (renderingThreads == null)
				CreateThreads();
		}

		private void CreateThreads()
		{
			renderingThreads = new Thread[threadsCount];
			plotters = new CapturingPlotter[threadsCount];
			isRendering = new bool[threadsCount];

			for (int i = 0; i < threadsCount; i++)
			{
				var thread = new Thread(new ThreadStart(RenderTask));
				thread.IsBackground = true;
				thread.Priority = ThreadPriority.BelowNormal;
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();

				renderingThreads[i] = thread;
			}
		}

		protected void OnChildContentBoundsChanged(object sender, RoutedEventArgs e)
		{
			var child = (DependencyObject)sender;
			ContentBounds = child.GetValueSync<DataRect>(Viewport2D.ContentBoundsProperty);

			DependencyObject dependencySender = sender as DependencyObject;
			var contentBounds = Viewport2D.GetContentBounds(dependencySender);
			Debug.WriteLine("ContentBounds = " + contentBounds);

			RaiseChangedAsync();
		}

		private void OnChildUpdateRequested(object sender, RoutedEventArgs e)
		{
			RaiseChangedAsync();
		}

		private void RaiseChangedAsync()
		{
			Dispatcher.BeginInvoke(() => { RaiseChangedEvent(); }, DispatcherPriority.Background);
		}

		int createdPlottersCount;
		private void RenderTask()
		{
			var threadIndex = Array.IndexOf<Thread>(renderingThreads, Thread.CurrentThread);

			CapturingPlotter plotter = new CapturingPlotter();
			plotters[threadIndex] = plotter;

			plotter.PerformLoad();
			plotter.ViewportClipToBoundsEnlargeFactor = 1;

			plotter.Measure(tileSize);
			plotter.Arrange(new Rect(tileSize));

			Interlocked.Increment(ref createdPlottersCount);

			AddChildToPlotter(plotter);

			plotter.Dispatcher.Hooks.DispatcherInactive += Hooks_DispatcherInactive;

			Dispatcher.Run();
		}

		void Hooks_DispatcherInactive(object sender, EventArgs e)
		{
			TileIndex id;
			if (!isRendering[GetIndex()] && waitingIndexes.TryRemove(out id))
			{
				RenderToBitmapCore(id);
			}
		}

		private CapturingPlotter GetPlotter()
		{
			var threadIndex = Array.IndexOf<Thread>(renderingThreads, Thread.CurrentThread);
			return plotters[threadIndex];
		}

		private int GetIndex()
		{
			var threadIndex = Array.IndexOf<Thread>(renderingThreads, Thread.CurrentThread);
			return threadIndex;
		}

		private void RenderToBitmapCore(TileIndex id)
		{
			isRendering[GetIndex()] = true;

			var plotter = GetPlotter();
			var visible = GetTileBounds(id);

			plotter.Visible = visible;

			// this is done to make all inside plotter to perform measure and arrange procedures
			plotter.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);

			RenderTargetBitmap bmp = new RenderTargetBitmap((int)tileSize.Width, (int)tileSize.Height, 96, 96, PixelFormats.Pbgra32);
			bmp.Render(plotter);
			bmp.Freeze();

			ReportSuccessAsync(null, bmp, id);

			isRendering[GetIndex()] = false;
		}

		public override bool Contains(TileIndex id)
		{
			if (renderingThreads == null) return false;

			foreach (var plotter in plotters)
			{
				plotter.Dispatcher.BeginInvoke(() => { }, DispatcherPriority.Background);
			}

			var tileBounds = GetTileBounds(id);

			bool contains = contentBounds.IntersectsWith(tileBounds);
			return contains;
		}

		public override void BeginLoadImage(TileIndex id)
		{
			waitingIndexes.Add(id);
		}

		private DataRect GetTileBounds(TileIndex id)
		{
			double tileSide = GetTileSide(id.Level);
			DataRect result = new DataRect(id.X * tileSide, id.Y * tileSide, tileSide, tileSide);
			return result;
		}

		private double GetTileSide(double level)
		{
			// we are assuming that tile in level 0 is a square of 1*1 units in viewport coordinates
			return 1.0 * Math.Pow(2, -level);
		}

		public override void CancelRunningOperations()
		{
			waitingIndexes = new BlockingCollection<TileIndex>();
		}

		#region IRenderingTileServer Members

		private DataRect contentBounds = DataRect.Empty;
		public DataRect ContentBounds
		{
			get { return contentBounds; }
			set
			{
				contentBounds = value;
				RaiseChangedAsync();
			}
		}

		#endregion
	}
}
