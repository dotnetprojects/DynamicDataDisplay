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
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Navigation;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.FileServers;
using Microsoft.Research.DynamicDataDisplay;

namespace MapSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			leftPlotter.Viewport.SetBinding(Viewport2D.VisibleProperty, new Binding
			{
				Source = rightPlotter.Viewport,
				Path = new PropertyPath("Visible"),
				Mode = BindingMode.TwoWay
			});

			leftCursor.SetBinding(CursorCoordinateGraph.PositionProperty, new Binding
			{
				Source = rightCursor,
				Path = new PropertyPath("Position"),
				Mode = BindingMode.TwoWay
			});

			leftPlotter.Viewport.Visible = new DataRect(37, 55, 1, 1).DataToViewport(leftPlotter.Viewport.Transform);
		}
	}
}
