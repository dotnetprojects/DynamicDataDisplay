using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts
{
	public class MapElement : FrameworkElement
	{
		public ImageSource Bitmap
		{
			get { return (ImageSource)GetValue(BitmapProperty); }
			set { SetValue(BitmapProperty, value); }
		}

		public static readonly DependencyProperty BitmapProperty = DependencyProperty.Register(
		  "Bitmap",
		  typeof(ImageSource),
		  typeof(MapElement),
		  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		protected override void OnRender(DrawingContext drawingContext)
		{
			var dc = drawingContext;

			Rect bounds = new Rect(RenderSize);
			dc.DrawImage(Bitmap, bounds);
		}
	}
}
