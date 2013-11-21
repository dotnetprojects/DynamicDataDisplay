using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Input;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	public class ColorSelector : SelectorPlotter
	{
		static ColorSelector()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSelector), new FrameworkPropertyMetadata(typeof(ColorSelector)));
		}

		private PaletteControl paletteControl = new PaletteControl();
		public ColorSelector()
		{
			Palette = new HSBPalette();

			Background = Brushes.Transparent;

			paletteControl.SetBinding(PaletteControl.PaletteProperty, new Binding { Source = this, Path = new PropertyPath("Palette") });
			BottomPanel.Children.Add(paletteControl);

			paletteControl.MouseLeftButtonDown += paletteControl_MouseLeftButtonDown;
			paletteControl.MouseMove += paletteControl_MouseMove;

			Children.Remove(KeyboardNavigation);
			Children.Remove(MouseNavigation);
			Children.Remove(LeftHighlight);
			Children.Remove(RightHighlight);
			Children.Remove(SelectorNavigation);

			Viewport.Domain = new DataRect(0, 0, 1, 1);
		}

		public double SelectedValue
		{
			get { return Marker.Position.X; }
			set { Marker.Position = new Point(value, 0); }
		}

		private void paletteControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Point screenPos = e.GetPosition(this);
				Point viewportPos = screenPos.ScreenToViewport(Transform);
				Marker.Position = viewportPos;

				e.Handled = true;
			}
		}

		private void paletteControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point screenPos = e.GetPosition(this);
			Point viewportPos = screenPos.ScreenToViewport(Transform);
			Marker.Position = viewportPos;

			e.Handled = true;
		}

		protected override void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			// do nothing
		}

		public IPalette Palette
		{
			get { return (IPalette)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}

		public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
		  "Palette",
		  typeof(IPalette),
		  typeof(ColorSelector),
		  new FrameworkPropertyMetadata(null, OnPaletteChanged));

		private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ColorSelector owner = (ColorSelector)d;
			owner.PaletteRatio = 0.5;
		}

		protected override void OnMarkerPositionChanged(PositionChangedEventArgs e)
		{
			if (Palette != null)
			{
				SelectedColor = Palette.GetColor(e.Position.X);
				SelectedBrush = new SolidColorBrush(SelectedColor);
				SelectedValueChanged.Raise(this);
			}
		}

		public double PaletteRatio
		{
			get { return (double)GetValue(PaletteRatioProperty); }
			set { SetValue(PaletteRatioProperty, value); }
		}

		public static readonly DependencyProperty PaletteRatioProperty = DependencyProperty.Register(
		  "PaletteRatio",
		  typeof(double),
		  typeof(ColorSelector),
		  new FrameworkPropertyMetadata(0.0, OnPaletteRatioChanged));

		private static void OnPaletteRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ColorSelector owner = (ColorSelector)d;
			owner.Marker.Position = new Point((double)e.NewValue, 1);
		}

		public Color SelectedColor
		{
			get { return (Color)GetValue(SelectedColorProperty); }
			protected set { SetValue(SelectedColorPropertyKey, value); }
		}

		private static readonly DependencyPropertyKey SelectedColorPropertyKey = DependencyProperty.RegisterReadOnly(
		  "SelectedColor",
		  typeof(Color),
		  typeof(ColorSelector),
		  new FrameworkPropertyMetadata(new Color()));

		public static readonly DependencyProperty SelectedColorProperty = SelectedColorPropertyKey.DependencyProperty;

		public Brush SelectedBrush
		{
			get { return (Brush)GetValue(SelectedBrushProperty); }
			protected set { SetValue(SelectedBrushPropertyKey, value); }
		}

		private static readonly DependencyPropertyKey SelectedBrushPropertyKey = DependencyProperty.RegisterReadOnly(
		  "SelectedBrush",
		  typeof(Brush),
		  typeof(ColorSelector),
		  new FrameworkPropertyMetadata(null));

		public static readonly DependencyProperty SelectedBrushProperty = SelectedBrushPropertyKey.DependencyProperty;

		/// <summary>
		/// Occurs when selected value changes.
		/// </summary>
		public event EventHandler SelectedValueChanged;
	}
}
