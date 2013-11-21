using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
{
	public sealed class ViewportMarginPanel : ViewportHostPanel
	{
		public static Size GetScreenMargin(DependencyObject obj)
		{
			return (Size)obj.GetValue(ScreenMarginProperty);
		}

		public static void SetScreenMargin(DependencyObject obj, Size value)
		{
			obj.SetValue(ScreenMarginProperty, value);
		}

		public static readonly DependencyProperty ScreenMarginProperty = DependencyProperty.RegisterAttached(
		  "ScreenMargin",
		  typeof(Size),
		  typeof(ViewportMarginPanel),
		  new FrameworkPropertyMetadata(Size.Empty, OnLayoutPropertyChanged));

		protected override Rect GetElementScreenBoundsCore(CoordinateTransform transform, UIElement child)
		{
			Rect bounds = base.GetElementScreenBoundsCore(transform, child);
			Size margin = GetScreenMargin(child);
			if (!margin.IsEmpty)
			{
				bounds.Inflate(margin);
			}
			return bounds;
		}
	}
}
