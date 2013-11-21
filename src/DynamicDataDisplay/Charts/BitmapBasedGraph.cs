using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using System.Windows.Input;

namespace Microsoft.Research.DynamicDataDisplay
{
	public abstract class BitmapBasedGraph : ViewportElement2D
	{
		static BitmapBasedGraph()
		{
			Type thisType = typeof(BitmapBasedGraph);
			BackgroundRenderer.UsesBackgroundRenderingProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(true));
		}

		private int nextRequestId = 0;

		/// <summary>Latest complete request</summary>
		private RenderRequest completedRequest = null;

		/// <summary>Currently running request</summary>
		private RenderRequest activeRequest = null;

		/// <summary>Result of latest complete request</summary>
		private BitmapSource completedBitmap = null;

		/// <summary>Pending render request</summary>
		private RenderRequest pendingRequest = null;

		/// <summary>Single apartment thread used for background rendering</summary>
		/// <remarks>STA is required for creating WPF components in this thread</remarks>
		private Thread renderThread = null;

		private AutoResetEvent renderRequested = new AutoResetEvent(false);

		private ManualResetEvent shutdownRequested = new ManualResetEvent(false);

		/// <summary>True means that current bitmap is invalidated and is to be re-rendered.</summary>
		private bool bitmapInvalidated = true;

		/// <summary>Shows tooltips.</summary>
		private PopupTip popup;

		/// <summary>
		/// Initializes a new instance of the <see cref="MarkerPointsGraph"/> class.
		/// </summary>
		public BitmapBasedGraph()
		{
			ManualTranslate = true;
		}

		protected virtual UIElement GetTooltipForPoint(Point point, DataRect visible, Rect output)
		{
			return null;
		}

		protected PopupTip GetPopupTipWindow()
		{
			if (popup != null)
				return popup;

			foreach (var item in Plotter.Children)
			{
				if (item is ViewportUIContainer)
				{
					ViewportUIContainer container = (ViewportUIContainer)item;
					if (container.Content is PopupTip)
						return popup = (PopupTip)container.Content;
				}
			}

			popup = new PopupTip();
			popup.Placement = PlacementMode.Relative;
			popup.PlacementTarget = this;
			Plotter.Children.Add(popup);
			return popup;
		}

		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseMove(e);

			var popup = GetPopupTipWindow();
			if (popup.IsOpen)
				popup.Hide();

			if (bitmapInvalidated) return;

			Point p = e.GetPosition(this);
			Point dp = p.ScreenToData(Plotter2D.Transform);

			var tooltip = GetTooltipForPoint(p, completedRequest.Visible, completedRequest.Output);
			if (tooltip == null) return;

			popup.VerticalOffset = p.Y + 20;
			popup.HorizontalOffset = p.X;

			popup.ShowDelayed(new TimeSpan(0, 0, 1));

			Grid grid = new Grid();

			Rectangle rect = new Rectangle
			{
				Stroke = Brushes.Black,
				Fill = SystemColors.InfoBrush
			};

			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Vertical;
			sp.Children.Add(tooltip);
			sp.Margin = new Thickness(4, 2, 4, 2);

			var tb = new TextBlock();
			tb.Text = String.Format("Location: {0:F2}, {1:F2}", dp.X, dp.Y);
			tb.Foreground = SystemColors.GrayTextBrush;
			sp.Children.Add(tb);

			grid.Children.Add(rect);
			grid.Children.Add(sp);
			grid.Measure(SizeHelper.CreateInfiniteSize());
			popup.Child = grid;
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			GetPopupTipWindow().Hide();
		}

		/// <summary>
		/// Adds a render task and invalidates visual.
		/// </summary>
		public void UpdateVisualization()
		{
			if (Viewport == null) return;

			Rect output = new Rect(this.RenderSize);
			CreateRenderTask(Viewport.Visible, output);
			InvalidateVisual();
		}

		protected override void OnVisibleChanged(DataRect newRect, DataRect oldRect)
		{
			base.OnVisibleChanged(newRect, oldRect);
			CreateRenderTask(newRect, Viewport.Output);
			InvalidateVisual();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			CreateRenderTask(Viewport.Visible, new Rect(sizeInfo.NewSize));
			InvalidateVisual();
		}

		protected abstract BitmapSource RenderFrame(DataRect visible, Rect output);

		private void RenderThreadFunc()
		{
			WaitHandle[] events = new WaitHandle[] { renderRequested, shutdownRequested };
			while (true)
			{
				lock (this)
				{
					activeRequest = null;
					if (pendingRequest != null)
					{
						activeRequest = pendingRequest;
						pendingRequest = null;
					}
				}
				if (activeRequest == null)
				{
					WaitHandle.WaitAny(events);
					if (shutdownRequested.WaitOne(0))
						break;
				}
				else
				{
					try
					{
						BitmapSource result = (BitmapSource)RenderFrame(activeRequest.Visible, activeRequest.Output);
						if (result != null)
							Dispatcher.BeginInvoke(
								new RenderCompletionHandler(OnRenderCompleted),
								new RenderResult(activeRequest, result));
					}
					catch (Exception exc)
					{
						Trace.WriteLine(String.Format("RenderRequest {0} failed: {1}", activeRequest.RequestID, exc.Message));
					}
				}
			}
		}

		private void CreateRenderTask(DataRect visible, Rect output)
		{
			lock (this)
			{
				bitmapInvalidated = true;

				if (activeRequest != null)
					activeRequest.Cancel();
				pendingRequest = new RenderRequest(nextRequestId++, visible, output);
				renderRequested.Set();
			}
			if (renderThread == null)
			{
				renderThread = new Thread(RenderThreadFunc);
				renderThread.IsBackground = true;
				renderThread.SetApartmentState(ApartmentState.STA);
				renderThread.Start();
			}
		}

		private delegate void RenderCompletionHandler(RenderResult result);

		protected virtual void OnRenderCompleted(RenderResult result)
		{
			if (result.IsSuccess)
			{
				completedRequest = result.Request;
				completedBitmap = result.CreateBitmap();
				bitmapInvalidated = false;

				InvalidateVisual();
				BackgroundRenderer.RaiseRenderingFinished(this);
			}
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state)
		{
			if (completedRequest != null && completedBitmap != null)
				dc.DrawImage(completedBitmap, completedRequest.Visible.ViewportToScreen(Viewport.Transform));
		}
	}

	public class RenderRequest
	{
		private int requestId;
		private DataRect visible;
		private Rect output;
		private int cancelling;

		public RenderRequest(int requestId, DataRect visible, Rect output)
		{
			this.requestId = requestId;
			this.visible = visible;
			this.output = output;
		}

		public int RequestID
		{
			get { return requestId; }
		}

		public DataRect Visible
		{
			get { return visible; }
		}

		public Rect Output
		{
			get { return output; }
		}

		public bool IsCancellingRequested
		{
			get { return cancelling > 0; }
		}

		public void Cancel()
		{
			Interlocked.Increment(ref cancelling);
		}
	}

	public class RenderResult
	{
		private RenderRequest request;
		private int pixelWidth, pixelHeight, stride;
		private double dpiX, dpiY;
		private BitmapPalette palette;
		private PixelFormat format;
		private Array pixels;

		/// <summary>Constructs successul rendering result</summary>
		/// <param name="request">Source request</param>
		/// <param name="result">Rendered bitmap</param>
		public RenderResult(RenderRequest request, BitmapSource result)
		{
			this.request = request;
			pixelWidth = result.PixelWidth;
			pixelHeight = result.PixelHeight;
			stride = result.PixelWidth * result.Format.BitsPerPixel / 8;
			dpiX = result.DpiX;
			dpiY = result.DpiY;
			palette = result.Palette;
			format = result.Format;
			pixels = new byte[pixelHeight * stride];
			result.CopyPixels(pixels, stride, 0);
		}

		public RenderRequest Request
		{
			get
			{
				return request;
			}
		}

		public bool IsSuccess
		{
			get
			{
				return pixels != null;
			}
		}

		public BitmapSource CreateBitmap()
		{
			return BitmapFrame.Create(pixelWidth, pixelHeight, dpiX, dpiY,
				format, palette, pixels, stride);
		}
	}
}
