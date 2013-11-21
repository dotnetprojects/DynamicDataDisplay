using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.Research.DynamicDataDisplay;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	/// <summary>
	/// Control to display palette as an image and axis with ticks.
	/// </summary>
	[ContentProperty("Palette")]
	public partial class PaletteControl : UserControl
	{
		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="PaletteControl"/> class.
		/// </summary>
		public PaletteControl()
		{
			InitializeComponent();

			axis.DrawMinorTicks = false;
			axis.TickSize = 3;
			axis.IsStaticAxis = true;

			Loaded += PaletteControl_Loaded;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the palette.
		/// </summary>
		/// <value>The palette.</value>
		public IPalette Palette
		{
			get { return (IPalette)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}

		/// <summary>
		/// Gets or sets the range of axis values.
		/// </summary>
		/// <value>The range.</value>
		public Range<double> Range
		{
			get { return (Range<double>)GetValue(RangeProperty); }
			set { SetValue(RangeProperty, value); }
		}

		#endregion

		#region Private methods

		private void PaletteControl_Loaded(object sender, RoutedEventArgs e)
		{
			OnRangeChanged();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			UpdateBitmap();
		}

		private void OnPaletteChanged(IPalette oldPalette, IPalette newPalette)
		{
			if (oldPalette != null)
			{
				oldPalette.Changed -= Palette_Changed;
			}
			if (newPalette != null)
			{
				newPalette.Changed += Palette_Changed;
			}

			UpdateBitmap();
		}

		void Palette_Changed(object sender, EventArgs e)
		{
			UpdateBitmap();
		}

		private void UpdateBitmap()
		{
			if (ActualWidth == 0)
			{
				image.Source = null;
				return;
			}

			if (Palette == null)
			{
				image.Source = null;
				return;
			}

			int width = (int)ActualWidth;

			int height = 1;
			double dpi = 96;

			WriteableBitmap bmp = new WriteableBitmap(width, height,
				dpi, dpi,
				PixelFormats.Bgra32, null);

			int[] pixels = new int[width];
			for (int i = 0; i < width; i++)
			{
				double ratio = i / ((double)width);
				Color color = Palette.GetColor(ratio);
				int argb = color.ToArgb();
				pixels[i] = argb;
			}

			bmp.WritePixels(
				new Int32Rect(0, 0, width, height),
				pixels,
				bmp.BackBufferStride,
				0);

			image.Source = bmp;
		}

		private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PaletteControl control = (PaletteControl)d;
			control.OnRangeChanged();
		}

		private void OnRangeChanged()
		{
			axis.Range = Range;
		}

		#endregion

		#region Dependency properties

		public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
			"Palette",
			  typeof(IPalette),
			  typeof(PaletteControl),
			  new FrameworkPropertyMetadata(null, OnPaletteChanged));

		private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PaletteControl control = (PaletteControl)d;
			control.OnPaletteChanged((IPalette)e.OldValue, (IPalette)e.NewValue);
		}

		public static readonly DependencyProperty RangeProperty = DependencyProperty.Register(
		  "Range",
		  typeof(Range<double>),
		  typeof(PaletteControl),
		  new FrameworkPropertyMetadata(new Range<double>(0, 1), OnRangeChanged));

		public bool ShowAxis
		{
			get { return (bool)GetValue(ShowAxisProperty); }
			set { SetValue(ShowAxisProperty, value); }
		}

		public static readonly DependencyProperty ShowAxisProperty = DependencyProperty.Register(
		  "ShowAxis",
		  typeof(bool),
		  typeof(PaletteControl),
		  new FrameworkPropertyMetadata(true));

		public double PaletteHeight
		{
			get { return (double)GetValue(PaletteHeightProperty); }
			set { SetValue(PaletteHeightProperty, value); }
		}

		public static readonly DependencyProperty PaletteHeightProperty = DependencyProperty.Register(
		  "PaletteHeight",
		  typeof(double),
		  typeof(PaletteControl),
		  new FrameworkPropertyMetadata(Double.NaN));

		#endregion
	}
}
