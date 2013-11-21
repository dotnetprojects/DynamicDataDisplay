#define a

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
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace VideoWall
{
	/// <summary>
	/// Interaction logic for LeftTopPlotterWindow.xaml
	/// </summary>
	public partial class PlotterWindow : Window, IPlotterWindow
	{
		public PlotterWindow()
		{
			InitializeComponent();

			plotter.Viewport.PropertyChanged += new EventHandler<ExtendedPropertyChangedEventArgs>(Viewport_PropertyChanged);

			Loaded += new RoutedEventHandler(PlotterWindow_Loaded);
		}

		void PlotterWindow_Loaded(object sender, RoutedEventArgs e)
		{
			int y = Program.yNum - 1 - Y;

			if (y != 0)
			{
#if a
				FrameworkElement element = (FrameworkElement)plotter.MainHorizontalAxis;
				element.Visibility = Visibility.Collapsed;
#else
				plotter.HorizontalAxis = null;
#endif
			}
			if (X != 0) {
#if a
				FrameworkElement element = (FrameworkElement)plotter.MainVerticalAxis;
				element.Visibility = Visibility.Collapsed;
#else
				plotter.VerticalAxis = null;
#endif
			}

			// this is done to prevent visible change because of 
			// accidental window size change.
			plotter.Viewport.Restrictions.Clear();
		}

		void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				VisibleChanged.Raise(this);
			}
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			Focus();
		}

		#region IPlotterWindow Members

		public int X { get; set; }

		public int Y { get; set; }

		public ChartPlotter Plotter
		{
			get { return plotter; }
		}

		public event EventHandler VisibleChanged;

		#endregion
	}
}
