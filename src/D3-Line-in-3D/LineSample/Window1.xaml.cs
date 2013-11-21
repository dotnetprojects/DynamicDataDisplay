using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using SlimDX.Direct3D9;
using SlimDX;
using System.Drawing;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Threading;
using System.Diagnostics;

namespace LineSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		Direct3DEx Direct3D = new Direct3DEx();
		PresentParameters pp;
		D3DImage d3dimage = new D3DImage();
		DeviceEx device;
		IPointDataSource animatedDataSource;
		readonly double[] animatedX = new double[100];
		readonly double[] animatedY = new double[100];
		double phase = 0;
		readonly DispatcherTimer timer = new DispatcherTimer();


		private Device Device
		{
			get { return device; }
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			image.Source = d3dimage;
			Initialize();

			EnumerableDataSource<double> xSrc = new EnumerableDataSource<double>(animatedX);
			xSrc.SetXMapping(x => x);
			var yDS = new EnumerableDataSource<double>(animatedY);
			yDS.SetYMapping(y => y);
			animatedDataSource = new CompositeDataSource(xSrc, yDS);

			timer.Interval = TimeSpan.FromMilliseconds(10);
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		Stopwatch watch2 = Stopwatch.StartNew();
		void timer_Tick(object sender, EventArgs e)
		{
			Debug.WriteLine(watch2.ElapsedMilliseconds);

			phase += 0.01;
			if (phase > 2 * Math.PI)
				phase -= 2 * Math.PI;
			for (int i = 0; i < animatedX.Length; i++)
			{
				animatedX[i] = 2 * Math.PI * i / animatedX.Length;

				if (i % 2 == 0)
					animatedY[i] = Math.Sin(animatedX[i] + phase);
				else
					animatedY[i] = -Math.Sin(animatedX[i] + phase);
			}
		}

		void Initialize()
		{
			ReleaseD3D();

			HwndSource hwnd = new HwndSource(0, 0, 0, 0, 0, "test", IntPtr.Zero);

			pp = new PresentParameters();
			pp.SwapEffect = SwapEffect.Discard;
			pp.DeviceWindowHandle = hwnd.Handle;
			pp.Windowed = true;
			pp.BackBufferWidth = (int)ActualWidth;
			pp.BackBufferHeight = (int)ActualHeight;
			pp.BackBufferFormat = Format.X8R8G8B8;
			//pp.Multisample = MultisampleType.EightSamples;


			device = new DeviceEx(Direct3D, 0,
								DeviceType.Hardware,
								hwnd.Handle,
								CreateFlags.HardwareVertexProcessing,
								pp);

			System.Windows.Media.CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
			d3dimage.IsFrontBufferAvailableChanged += new DependencyPropertyChangedEventHandler(d3dimage_IsFrontBufferAvailableChanged);

			d3dimage.Lock();
			d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, device.GetBackBuffer(0, 0).ComPointer);
			d3dimage.Unlock();
		}

		void d3dimage_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
		}

		bool sizeChanged = false;
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			sizeChanged = true;
		}

		public void ReleaseD3D()
		{
			if (device != null)
			{
				if (!device.Disposed)
				{
					device.Dispose();
					device = null;
				}
			}
			d3dimage.Lock();
			d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
			d3dimage.Unlock();
		}

		//Stopwatch watch = Stopwatch.StartNew();

		int counter = 0;
		private void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			if (counter++ % 2 == 0) return;

			//Debug.WriteLine(watch.ElapsedMilliseconds);

			if (sizeChanged)
			{
				pp.BackBufferWidth = (int)ActualWidth;
				pp.BackBufferHeight = (int)ActualHeight;
				//Device.Reset(pp);
				sizeChanged = false;
			}
			Result result;

			if (d3dimage.IsFrontBufferAvailable)
			{
				result = Device.TestCooperativeLevel();
				if (result.IsFailure)
				{
					throw new Direct3D9Exception();
				}
				d3dimage.Lock();

				Device.SetRenderState(RenderState.CullMode, Cull.None);
				Device.SetRenderState(RenderState.ZEnable, true);
				Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(1, 1, 1, 1), 1.0f, 0);
				Device.BeginScene();

				var points = animatedDataSource.GetPoints().ToArray();

				var pointList = new VertexPositionColor[points.Length];


				//phase += 0.01;
				//if (phase > 2 * Math.PI)
				//    phase -= 2 * Math.PI;
				//for (int i = 0; i < animatedX.Length; i++)
				//{
				//    animatedX[i] = 2 * Math.PI * i / animatedX.Length;

				//    if (i % 2 == 0)
				//        animatedY[i] = Math.Sin(animatedX[i] + phase);
				//    else
				//        animatedY[i] = -Math.Sin(animatedX[i] + phase);
				//}


				for (int i = 0; i < points.Length; i++)
				{
					pointList[i] = new VertexPositionColor
					{
						Position = new Vector4(100 + 500 * (float)points[i].X, 500 + 500 * (float)points[i].Y, 0.5f, 1),
						Color = Color.Orange.ToArgb()
					};
				}

				var lineListIndices = new short[(points.Length * 2) - 2];

				// Populate the array with references to indices in the vertex buffer
				for (int i = 0; i < points.Length - 1; i++)
				{
					lineListIndices[i * 2] = (short)(i);
					lineListIndices[(i * 2) + 1] = (short)(i + 1);
				}

				device.SetRenderState(RenderState.AntialiasedLineEnable, true);
				device.VertexFormat = VertexFormat.Diffuse | VertexFormat.PositionRhw;
				device.DrawIndexedUserPrimitives<short, VertexPositionColor>(PrimitiveType.LineList, 0, points.Length, points.Length - 1, lineListIndices, Format.Index16, pointList, 20);
				//pointList,
				//0,  // vertex buffer offset to add to each element of the index buffer
				//8,  // number of vertices in pointList
				//lineListIndices,  // the index buffer
				//0,  // first index element to read
				//7   // number of primitives to draw
				//);


				Device.EndScene();
				Device.Present();

				d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, Device.GetBackBuffer(0, 0).ComPointer);
				d3dimage.AddDirtyRect(new Int32Rect(0, 0, d3dimage.PixelWidth, d3dimage.PixelHeight));
				d3dimage.Unlock();
			}

		}
	}

	struct VertexPositionColor
	{
		public Vector4 Position;
		public int Color;
	}
}
