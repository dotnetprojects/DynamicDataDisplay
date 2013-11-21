using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.IDataSource2D<double>;
using SlimDX.Direct3D9;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using SlimDX;

namespace Microsoft.Research.DynamicDataDisplay.DirectX2D
{
	public class DirectXWorkingColorMap : DirectXChart
	{
		#region Properties

		#region DataSource

		public IDataSource2D<double> DataSource
		{
			get { return (IDataSource2D<double>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(IDataSource2D<double>),
		  typeof(DirectXWorkingColorMap),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DirectXWorkingColorMap owner = (DirectXWorkingColorMap)d;
			owner.OnDataSourceReplaced((DataSource)e.OldValue, (DataSource)e.NewValue);
		}

		private void OnDataSourceReplaced(IDataSource2D<double> prevDataSource, IDataSource2D<double> currDataSource)
		{
			if (prevDataSource != null)
			{
				prevDataSource.Changed -= OnDataSourceChanged;
			}
			if (currDataSource != null)
			{
				currDataSource.Changed += OnDataSourceChanged;
			}

			FillVertexBuffer();
		}

		private void OnDataSourceChanged(object sender, EventArgs e)
		{
			FillVertexBuffer();
		}

		#endregion

		#region Palette property

		public IPalette Palette
		{
			get { return (IPalette)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}

		public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
		  "Palette",
		  typeof(IPalette),
		  typeof(DirectXWorkingColorMap),
		  new FrameworkPropertyMetadata(new HSBPalette(), OnPaletteChanged));

		private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DirectXWorkingColorMap owner = (DirectXWorkingColorMap)d;
			// todo 
		}

		#endregion // end of Palette property

		#endregion // end of Properties

		protected override void OnDirectXRender()
		{
			if (vertexBuffer == null) FillVertexBuffer();
			if (vertexBuffer == null) return;

			//Device.SetTransform(TransformState.View, /*Matrix.Translation(100, 100, 0)* */
			//    Matrix.Translation(-0.05f, -0.05f, 0) *
			//    Matrix.Scaling(3.0f, 3.0f, 1.0f)
			//    );

			Device.VertexFormat = VertexFormat.PositionRhw | VertexFormat.Diffuse;
			//Device.SetStreamSource(0, vertexBuffer, 0, VertexPositionColor.SizeInBytes);
			//Device.Indices = indexBuffer;
			//Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, indicesCount / 3);
			Device.DrawIndexedUserPrimitives<int, VertexPosition4Color>(PrimitiveType.TriangleList, 0, vertexCount, indicesCount / 3, indicesArray, Format.Index32, verticesArray, VertexPosition4Color.SizeInBytes);
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);
			Plotter.Viewport.PropertyChanged += new EventHandler<ExtendedPropertyChangedEventArgs>(Viewport_PropertyChanged);
			FillVertexBuffer();
		}

		private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				FillVertexBuffer();
			}
		}

		VertexPosition4Color[] verticesArray;
		int[] indicesArray;
		private int indicesCount;
		private int vertexCount;
		private VertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;
		private void FillVertexBuffer()
		{
			if (DxHost == null) return;
			if (DataSource == null) return;
			if (Palette == null) return;
			if (Device == null) return;

			var dataSource = DataSource;
			var palette = Palette;
			var minMax = dataSource.GetMinMax();
			var transform = Plotter.Transform;

			vertexCount = DataSource.Width * DataSource.Height;

			verticesArray = new VertexPosition4Color[vertexCount];
			for (int i = 0; i < verticesArray.Length; i++)
			{
				int ix = i % DataSource.Width;
				int iy = i / DataSource.Width;
				Point point = dataSource.Grid[ix, iy];
				double data = dataSource.Data[ix, iy];

				double interpolatedData = (data - minMax.Min) / minMax.GetLength();
				var color = palette.GetColor(interpolatedData);

				var pointInScreen = point.DataToScreen(transform);
				var position = new Vector4((float)pointInScreen.X, (float)pointInScreen.Y, 0.5f, 1);
				verticesArray[i] = new VertexPosition4Color
				{
					Position = position,
					Color =
						//System.Windows.Media.Colors.Blue.ToArgb()
						color.ToArgb()
				};
			}

			vertexBuffer = new VertexBuffer(Device, vertexCount * VertexPosition4Color.SizeInBytes, Usage.WriteOnly, VertexFormat.Position | VertexFormat.Diffuse, Pool.Default);
			using (var stream = vertexBuffer.Lock(0, vertexCount * VertexPosition4Color.SizeInBytes, LockFlags.None))
			{
				stream.WriteRange<VertexPosition4Color>(verticesArray);
			}
			vertexBuffer.Unlock();

			indicesCount = (dataSource.Width - 1) * (dataSource.Height - 1) * 2 * 3;

			indicesArray = new int[indicesCount];
			int index = 0;
			int width = dataSource.Width;
			for (int iy = 0; iy < dataSource.Height - 1; iy++)
			{
				for (int ix = 0; ix < dataSource.Width - 1; ix++)
				{
					indicesArray[index + 0] = ix + 0 + iy * width;
					indicesArray[index + 1] = ix + 1 + iy * width;
					indicesArray[index + 2] = ix + (iy + 1) * width;

					indicesArray[index + 3] = ix + 1 + iy * width;
					indicesArray[index + 4] = ix + (iy + 1) * width;
					indicesArray[index + 5] = ix + 1 + (iy + 1) * width;

					index += 6;
				}
			}

			indexBuffer = new IndexBuffer(Device, indicesCount * sizeof(int), Usage.WriteOnly, Pool.Default, false);
			using (var stream = indexBuffer.Lock(0, indicesCount * sizeof(int), LockFlags.None))
			{
				stream.WriteRange<int>(indicesArray);
			}
			indexBuffer.Unlock();
		}
	}
}
