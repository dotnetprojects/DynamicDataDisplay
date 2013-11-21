using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using System.Windows;
using System.Windows.Interop;
using SlimDX;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.DirectX2D
{
	public class DirectXHost : FrameworkElement, IPlotterElement
	{
		public DirectXHost()
		{
			Effect = new Transparency();
		}

		private Device device;
		private Direct3D direct3D;
		private D3DImage image = new D3DImage();
		private bool sizeChanged;
		private PresentParameters pp = new PresentParameters();

		public Device Device
		{
			get { return device; }
		}

		public Direct3D Direct3D
		{
			get { return direct3D; }
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			Initialize3D();
		}

		public void AddChild(object child)
		{
			AddLogicalChild(child);
		}

		public void RemoveChild(object child)
		{
			RemoveLogicalChild(child);
		}

		private void Initialize3D()
		{
			HwndSource hwnd = new HwndSource(0, 0, 0, 0, 0, "D3", IntPtr.Zero);

			pp.SwapEffect = SwapEffect.Discard;
			pp.DeviceWindowHandle = hwnd.Handle;
			pp.Windowed = true;
			pp.BackBufferWidth = (int)ActualWidth;
			pp.BackBufferHeight = (int)ActualHeight;
			pp.BackBufferFormat = Format.X8R8G8B8;

			try
			{
				var direct3DEx = new Direct3DEx();
				direct3D = direct3DEx;
				device = new DeviceEx(direct3DEx, 0, DeviceType.Hardware, hwnd.Handle, CreateFlags.HardwareVertexProcessing, pp);
			}
			catch
			{
				direct3D = new Direct3D();
				device = new Device(direct3D, 0, DeviceType.Hardware, hwnd.Handle, CreateFlags.HardwareVertexProcessing, pp);
			}

			System.Windows.Media.CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
		}

		int counter = 0;
		private void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			if (counter++ % 2 == 0) return;
			if (image == null) return;

			try
			{
				if (sizeChanged)
				{
					pp.BackBufferWidth = (int)ActualWidth;
					pp.BackBufferHeight = (int)ActualHeight;
					device.Reset(pp);
					sizeChanged = false;
				}

				if (image.IsFrontBufferAvailable)
				{
					Result result = Device.TestCooperativeLevel();
					if (result.IsFailure)
					{
						throw new Direct3D9Exception();
					}
					image.Lock();

					device.SetRenderState(SlimDX.Direct3D9.RenderState.CullMode, Cull.None);
					device.SetRenderState(SlimDX.Direct3D9.RenderState.ZEnable, true);
					device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(1, 1, 1, 1), 1.0f, 0);
					device.BeginScene();

					try
					{
						Render.Raise(this);
					}
					catch (Exception exc)
					{
						Debug.WriteLine("Error in rendering in DirectXHost: " + exc.Message);
					}

					device.EndScene();
					device.Present();

					image.SetBackBuffer(D3DResourceType.IDirect3DSurface9, Device.GetBackBuffer(0, 0).ComPointer);
					image.AddDirtyRect(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight));
					image.Unlock();
				}
			}
			catch (Direct3D9Exception exc)
			{
				Device.Reset(pp);
				Debug.WriteLine("Exception in main render loop: " + exc.Message);
			}
		}

		public event EventHandler Render;

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			drawingContext.DrawImage(image, new Rect(RenderSize));
		}

		#region IPlotterElement Members

		private Plotter2D plotter;
		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.CentralGrid.Children.Add(this);
			this.plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;
		}

		private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Output")
			{
				sizeChanged = true;
			}
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;
			plotter.CentralGrid.Children.Remove(this);
			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion

	}
}
