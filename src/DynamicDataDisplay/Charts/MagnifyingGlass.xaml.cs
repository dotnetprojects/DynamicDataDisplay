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

namespace Microsoft.Research.DynamicDataDisplay
{
	public partial class MagnifyingGlass : Grid, IPlotterElement
	{
		public MagnifyingGlass()
		{
			InitializeComponent();
			Loaded += MagnifyingGlass_Loaded;

			whiteEllipse.Visibility = Visibility.Collapsed;
			magnifierEllipse.Visibility = Visibility.Collapsed;
		}

		private void MagnifyingGlass_Loaded(object sender, RoutedEventArgs e)
		{
			UpdateViewbox();
		}

		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
		{
			Magnification += e.Delta / Mouse.MouseWheelDeltaForOneLine * 0.2;
			e.Handled = false;
		}

		private void plotter_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			VisualBrush b = (VisualBrush)magnifierEllipse.Fill;
			Point pos = e.GetPosition(plotter.ParallelCanvas);

			Point plotterPos = e.GetPosition(plotter);

			Rect viewBox = b.Viewbox;
			double xoffset = viewBox.Width / 2.0;
			double yoffset = viewBox.Height / 2.0;
			viewBox.X = plotterPos.X - xoffset;
			viewBox.Y = plotterPos.Y - yoffset;
			b.Viewbox = viewBox;
			Canvas.SetLeft(this, pos.X - Width / 2);
			Canvas.SetTop(this, pos.Y - Height / 2);
		}

		private double magnification = 2.0;
		public double Magnification
		{
			get { return magnification; }
			set
			{
				magnification = value;

				UpdateViewbox();
			}
		}

		private void UpdateViewbox()
		{
			if (!IsLoaded)
				return;

			VisualBrush b = (VisualBrush)magnifierEllipse.Fill;
			Rect viewBox = b.Viewbox;
			viewBox.Width = Width / magnification;
			viewBox.Height = Height / magnification;
			b.Viewbox = viewBox;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == WidthProperty || e.Property == HeightProperty)
			{
				UpdateViewbox();
			}
		}

		#region IPlotterElement Members

		Plotter plotter;
		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;
			plotter.ParallelCanvas.Children.Add(this);
			plotter.PreviewMouseMove += plotter_PreviewMouseMove;
			plotter.MouseEnter += new MouseEventHandler(plotter_MouseEnter);
			plotter.MouseLeave += new MouseEventHandler(plotter_MouseLeave);

			VisualBrush b = (VisualBrush)magnifierEllipse.Fill;
			b.Visual = plotter.MainGrid;
		}

		void plotter_MouseLeave(object sender, MouseEventArgs e)
		{
			whiteEllipse.Visibility = Visibility.Collapsed;
			magnifierEllipse.Visibility = Visibility.Collapsed;
		}

		void plotter_MouseEnter(object sender, MouseEventArgs e)
		{
			whiteEllipse.Visibility = Visibility.Visible;
			magnifierEllipse.Visibility = Visibility.Visible;
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			plotter.MouseEnter -= new MouseEventHandler(plotter_MouseEnter);
			plotter.MouseLeave -= new MouseEventHandler(plotter_MouseLeave);

			plotter.PreviewMouseMove -= plotter_PreviewMouseMove;
			plotter.ParallelCanvas.Children.Remove(this);
			this.plotter = null;

			VisualBrush b = (VisualBrush)magnifierEllipse.Fill;
			b.Visual = null;
		}

		public Plotter Plotter
		{
			get { return plotter; ; }
		}

		#endregion
	}
}
