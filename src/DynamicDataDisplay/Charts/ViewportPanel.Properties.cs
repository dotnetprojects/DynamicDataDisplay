using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public partial class ViewportPanel
	{
		#region Properties

		#region ViewportBounds

		[AttachedPropertyBrowsableForChildren]
		public static DataRect GetViewportBounds(DependencyObject obj)
		{
			return (DataRect)obj.GetValue(ViewportBoundsProperty);
		}

		public static void SetViewportBounds(DependencyObject obj, DataRect value)
		{
			obj.SetValue(ViewportBoundsProperty, value);
		}

		public static readonly DependencyProperty ViewportBoundsProperty = DependencyProperty.RegisterAttached(
		  "ViewportBounds",
		  typeof(DataRect),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(DataRect.Empty, OnLayoutPropertyChanged));

		#endregion

		#region X

		[AttachedPropertyBrowsableForChildren]
		public static double GetX(DependencyObject obj)
		{
			return (double)obj.GetValue(XProperty);
		}

		public static void SetX(DependencyObject obj, double value)
		{
			obj.SetValue(XProperty, value);
		}

		public static readonly DependencyProperty XProperty = DependencyProperty.RegisterAttached(
		  "X",
		  typeof(double),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(Double.NaN, OnLayoutPropertyChanged));

		#endregion

		#region Y

		[AttachedPropertyBrowsableForChildren]
		public static double GetY(DependencyObject obj)
		{
			return (double)obj.GetValue(YProperty);
		}

		public static void SetY(DependencyObject obj, double value)
		{
			obj.SetValue(YProperty, value);
		}

		public static readonly DependencyProperty YProperty = DependencyProperty.RegisterAttached(
		  "Y",
		  typeof(double),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(Double.NaN, OnLayoutPropertyChanged));

		#endregion

		#region ViewportWidth

		[AttachedPropertyBrowsableForChildren]
		public static double GetViewportWidth(DependencyObject obj)
		{
			return (double)obj.GetValue(ViewportWidthProperty);
		}

		public static void SetViewportWidth(DependencyObject obj, double value)
		{
			obj.SetValue(ViewportWidthProperty, value);
		}

		public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.RegisterAttached(
		  "ViewportWidth",
		  typeof(double),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(Double.NaN, OnLayoutPropertyChanged));

		#endregion

		#region ViewportHeight

		[AttachedPropertyBrowsableForChildren]
		public static double GetViewportHeight(DependencyObject obj)
		{
			return (double)obj.GetValue(ViewportHeightProperty);
		}

		public static void SetViewportHeight(DependencyObject obj, double value)
		{
			obj.SetValue(ViewportHeightProperty, value);
		}

		public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.RegisterAttached(
		  "ViewportHeight",
		  typeof(double),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(Double.NaN, OnLayoutPropertyChanged));

		#endregion

		#region ScreenOffsetX

		[AttachedPropertyBrowsableForChildren]
		public static double GetScreenOffsetX(DependencyObject obj)
		{
			return (double)obj.GetValue(ScreenOffsetXProperty);
		}

		public static void SetScreenOffsetX(DependencyObject obj, double value)
		{
			obj.SetValue(ScreenOffsetXProperty, value);
		}

		public static readonly DependencyProperty ScreenOffsetXProperty = DependencyProperty.RegisterAttached(
		  "ScreenOffsetX",
		  typeof(double),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(Double.NaN, OnLayoutPropertyChanged));

		#endregion

		#region ScreenOffsetY

		[AttachedPropertyBrowsableForChildren]
		public static double GetScreenOffsetY(DependencyObject obj)
		{
			return (double)obj.GetValue(ScreenOffsetYProperty);
		}

		public static void SetScreenOffsetY(DependencyObject obj, double value)
		{
			obj.SetValue(ScreenOffsetYProperty, value);
		}

		public static readonly DependencyProperty ScreenOffsetYProperty = DependencyProperty.RegisterAttached(
		  "ScreenOffsetY",
		  typeof(double),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(Double.NaN, OnLayoutPropertyChanged));

		#endregion

		#region HorizontalAlignment

		[AttachedPropertyBrowsableForChildren]
		public static HorizontalAlignment GetViewportHorizontalAlignment(DependencyObject obj)
		{
			return (HorizontalAlignment)obj.GetValue(ViewportHorizontalAlignmentProperty);
		}

		public static void SetViewportHorizontalAlignment(DependencyObject obj, HorizontalAlignment value)
		{
			obj.SetValue(ViewportHorizontalAlignmentProperty, value);
		}

		public static readonly DependencyProperty ViewportHorizontalAlignmentProperty = DependencyProperty.RegisterAttached(
		  "ViewportHorizontalAlignment",
		  typeof(HorizontalAlignment),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(HorizontalAlignment.Center, OnLayoutPropertyChanged));

		#endregion

		#region VerticalAlignment

		[AttachedPropertyBrowsableForChildren]
		public static VerticalAlignment GetViewportVerticalAlignment(DependencyObject obj)
		{
			return (VerticalAlignment)obj.GetValue(ViewportVerticalAlignmentProperty);
		}

		public static void SetViewportVerticalAlignment(DependencyObject obj, VerticalAlignment value)
		{
			obj.SetValue(ViewportVerticalAlignmentProperty, value);
		}

		public static readonly DependencyProperty ViewportVerticalAlignmentProperty = DependencyProperty.RegisterAttached(
		  "ViewportVerticalAlignment",
		  typeof(VerticalAlignment),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(VerticalAlignment.Center, OnLayoutPropertyChanged));

		#endregion

		#region ActualViewportBounds

		public static DataRect GetActualViewportBounds(DependencyObject obj)
		{
			return (DataRect)obj.GetValue(ActualViewportBoundsProperty);
		}

		public static void SetActualViewportBounds(DependencyObject obj, DataRect value)
		{
			obj.SetValue(ActualViewportBoundsProperty, value);
		}

		public static readonly DependencyProperty ActualViewportBoundsProperty = DependencyProperty.RegisterAttached(
		  "ActualViewportBounds",
		  typeof(DataRect),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(DataRect.Empty));

		#endregion

		#region PrevActualViewportBounds

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static DataRect GetPrevActualViewportBounds(DependencyObject obj)
		{
			return (DataRect)obj.GetValue(PrevActualViewportBoundsProperty);
		}

		public static void SetPrevActualViewportBounds(DependencyObject obj, DataRect value)
		{
			obj.SetValue(PrevActualViewportBoundsProperty, value);
		}

		public static readonly DependencyProperty PrevActualViewportBoundsProperty = DependencyProperty.RegisterAttached(
		  "PrevActualViewportBounds",
		  typeof(DataRect),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(DataRect.Empty));

		#endregion

		#region MinScreenWidth

		public static double GetMinScreenWidth(DependencyObject obj)
		{
			return (double)obj.GetValue(MinScreenWidthProperty);
		}

		public static void SetMinScreenWidth(DependencyObject obj, double value)
		{
			obj.SetValue(MinScreenWidthProperty, value);
		}

		public static readonly DependencyProperty MinScreenWidthProperty = DependencyProperty.RegisterAttached(
		  "MinScreenWidth",
		  typeof(double),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(Double.NaN, OnLayoutPropertyChanged));

		#endregion // end of MinScreenWidth

		protected static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement uiElement = d as FrameworkElement;
			if (uiElement != null)
			{
				ViewportPanel panel = VisualTreeHelper.GetParent(uiElement) as ViewportPanel;
				if (panel != null)
				{
					// invalidating not self arrange, but calling Arrange method of only that uiElement which has changed position
					panel.InvalidatePosition(uiElement);
				}
			}
		}

		#region IsMarkersHost

		public bool IsMarkersHost
		{
			get { return (bool)GetValue(IsMarkersHostProperty); }
			set { SetValue(IsMarkersHostProperty, value); }
		}

		public static readonly DependencyProperty IsMarkersHostProperty = DependencyProperty.Register(
		  "IsMarkersHost",
		  typeof(bool),
		  typeof(ViewportPanel),
		  new FrameworkPropertyMetadata(false));

		#endregion // end of IsMarkersHost

		#endregion
	}
}
