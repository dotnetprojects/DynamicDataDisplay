using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Threading;
using SlimDX;
using System.Drawing;

namespace Microsoft.Research.DynamicDataDisplay.DirectX2D
{
	public class SampleLine : DirectXChart
	{
		private IPointDataSource animatedDataSource;
		private readonly double[] animatedX = new double[100];
		private readonly double[] animatedY = new double[100];
		private double phase = 0;
		private readonly DispatcherTimer timer = new DispatcherTimer();
		private Camera camera = new Camera();

		public SampleLine()
		{
			EnumerableDataSource<double> xSrc = new EnumerableDataSource<double>(animatedX);
			xSrc.SetXMapping(x => x);
			var yDS = new EnumerableDataSource<double>(animatedY);
			yDS.SetYMapping(y => y);
			animatedDataSource = new CompositeDataSource(xSrc, yDS);

			timer.Interval = TimeSpan.FromMilliseconds(10);
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();

			camera.FieldOfView = (float)(Math.PI / 4);
			camera.NearPlane = 0.0f;
			camera.FarPlane = 40.0f;
			camera.Location = new Vector3(0.0f, 7.0f, 20.0f);
			camera.Target = Vector3.Zero;
		}

		void timer_Tick(object sender, EventArgs e)
		{
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

		protected override void OnDirectXRender()
		{
			var device = Device;

			var points = animatedDataSource.GetPoints().ToArray();
			var pointList = new VertexPosition4Color[points.Length];

			for (int i = 0; i < points.Length; i++)
			{
				pointList[i] = new VertexPosition4Color
				{
					Position = new Vector4(100 + 500 * (float)points[i].X, 500 + 500 * (float)points[i].Y, 0.5f,1 ),
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

			Device.SetTransform(TransformState.World, Matrix.Translation(100, 0, 0));
			Device.SetTransform(TransformState.View, camera.ViewMatrix);
			Device.SetTransform(TransformState.Projection, camera.ProjectionMatrix);

			device.SetRenderState(SlimDX.Direct3D9.RenderState.AntialiasedLineEnable, true);
			device.VertexFormat = VertexFormat.Diffuse | VertexFormat.PositionRhw;
			device.DrawIndexedUserPrimitives<short, VertexPosition4Color>(PrimitiveType.LineList, 0, points.Length, points.Length - 1, lineListIndices, Format.Index16, pointList, VertexPosition4Color.SizeInBytes);
		}
	}
}
