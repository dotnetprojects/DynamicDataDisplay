using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;

namespace VideoWall
{
	class Program
	{
		public const int xNum = 3;
		public const int yNum = 2;

		[STAThread]
		public static void Main()
		{
			Application app = new Application();
			Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
			app.Startup += new StartupEventHandler(app_Startup);
			app.Run();
		}

		private const double sizeDiff = 10;
		private static List<PlotterWindow> windows = new List<PlotterWindow>();
		static void app_Startup(object sender, StartupEventArgs e)
		{
			double windowWidth = (int)(SystemParameters.MaximizedPrimaryScreenWidth / xNum) - sizeDiff;
			double windowHeight = (int)(SystemParameters.MaximizedPrimaryScreenHeight / yNum) - sizeDiff;

			for (int x = 0; x < xNum; x++)
			{
				for (int y = 0; y < yNum; y++)
				{
					PlotterWindow window = new PlotterWindow();
					window.Title = String.Format("{0} - {1}", x, y);

					window.X = x;
					window.Y = y;

					window.Width = windowWidth;
					window.Height = windowHeight;
					window.WindowStartupLocation = WindowStartupLocation.Manual;
					window.WindowStyle = WindowStyle.None;
					//window.WindowState = WindowState.Maximized;
					//window.ShowInTaskbar = x + y == 0;
					window.Left = x * (windowWidth + 0);
					window.Top = y * (windowHeight + 0);

					window.Plotter.Viewport.Visible = ComputeVisible(x, y);
					window.VisibleChanged += new EventHandler(window_VisibleChanged);

					windows.Add(window);

					window.Show();
				}
			}

			Application.Current.MainWindow = windows[0];
		}

		private static DataRect visible = new DataRect(-180, -90, 360, 180);
		private static Rect ComputeVisible(int x, int y)
		{
			double width = visible.Width / xNum;
			double height = visible.Height / yNum;

			y = yNum - 1 - y;
			Point location = visible.Location;
			location.Offset(width * x, height * y);

			return new Rect(location, new Size(width, height));
		}

		static bool inChange = false;
		static void window_VisibleChanged(object sender, EventArgs e)
		{
			if (inChange) return;

			inChange = true;

			PlotterWindow window = (PlotterWindow)sender;

			DataRect vis = window.Plotter.Viewport.Visible;
			visible = ComputeVisibleNewVisible(vis, window.X, window.Y);

			var otherWindows = windows.Where(w => w != window);
			foreach (var win in otherWindows)
			{
				win.Plotter.Viewport.Visible = ComputeVisible(win.X, win.Y);
			}

			inChange = false;
		}

		private static DataRect ComputeVisibleNewVisible(DataRect visible, int x, int y)
		{
			y = yNum - 1 - y;

			double width = visible.Width * xNum;
			double height = visible.Height * yNum;

			Point location = visible.Location;
			location.Offset(-visible.Width * x, -visible.Height * y);

			return new DataRect(location, new Size(width, height));
		}
	}
}
