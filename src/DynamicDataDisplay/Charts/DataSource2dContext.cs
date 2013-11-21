using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	// todo probably remove
	public sealed class DataSource2dContext : DependencyObject
	{
		public static DataRect GetVisibleRect(DependencyObject obj)
		{
			return (DataRect)obj.GetValue(VisibleRectProperty);
		}

		public static void SetVisibleRect(DependencyObject obj, DataRect value)
		{
			obj.SetValue(VisibleRectProperty, value);
		}

		public static readonly DependencyProperty VisibleRectProperty = DependencyProperty.RegisterAttached(
		  "VisibleRect",
		  typeof(DataRect),
		  typeof(DataSource2dContext),
		  new FrameworkPropertyMetadata(new DataRect()));

		public static Rect GetScreenRect(DependencyObject obj)
		{
			return (Rect)obj.GetValue(ScreenRectProperty);
		}

		public static void SetScreenRect(DependencyObject obj, Rect value)
		{
			obj.SetValue(ScreenRectProperty, value);
		}

		public static readonly DependencyProperty ScreenRectProperty = DependencyProperty.RegisterAttached(
		  "ScreenRect",
		  typeof(Rect),
		  typeof(DataSource2dContext),
		  new FrameworkPropertyMetadata(new Rect()));
	}
}

