#define _template

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Reflection;
using System;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
	/// <summary>This class is responsible for displaying and populating default context menu</summary>
	public partial class DefaultContextMenu : NavigationBase
	{
		static BitmapImage helpIcon;
		static BitmapImage copyScreenshotIcon;
		static BitmapImage saveScreenshotIcon;
		static BitmapImage fitToViewIcon;
		static DefaultContextMenu()
		{
			helpIcon = LoadIcon("HelpIcon");
			saveScreenshotIcon = LoadIcon("SaveIcon");
			copyScreenshotIcon = LoadIcon("CopyScreenshotIcon");
			fitToViewIcon = LoadIcon("FitToViewIcon");
		}

		private static BitmapImage LoadIcon(string name)
		{
			Assembly currentAssembly = typeof(DefaultContextMenu).Assembly;

			BitmapImage icon = new BitmapImage();
			icon.BeginInit();
			icon.StreamSource = currentAssembly.GetManifestResourceStream("Microsoft.Research.DynamicDataDisplay.Resources." + name + ".png");
			icon.EndInit();
			return icon;
		}

		public DefaultContextMenu()
		{
			InitContextMenu();
		}

		public void InitContextMenu()
		{
			ContextMenu menu = new ContextMenu();
			MenuItem fitToViewMenuItem = new MenuItem
			{
				Header = "Fit to view",
				ToolTip = "Make visible area fit to display entire contents",
				Icon = new Image { Source = fitToViewIcon },
				Command = ChartCommands.FitToView
			};

			MenuItem savePictureMenuItem = new MenuItem
			{
				Header = "Save picture",
				ToolTip = "Specify name of image file to save screenshot to",
				Icon = new Image { Source = saveScreenshotIcon },
				Command = ChartCommands.SaveScreenshot
			};

			MenuItem copyPictureMenuItem = new MenuItem
			{
				Header = "Copy picture",
				ToolTip = "Copy screenshot of charts to clipboard",
				Icon = new Image { Source = copyScreenshotIcon },
				Command = ChartCommands.CopyScreenshot
			};

			MenuItem quickHelpMenuItem = new MenuItem
			{
				Header = "Quick Help",
				ToolTip = "View brief instructions",
				Command = ChartCommands.ShowHelp,
				Icon = new Image { Source = helpIcon }
			};

			List<MenuItem> menuItems = new List<MenuItem> {
				fitToViewMenuItem, 
				copyPictureMenuItem, 
				savePictureMenuItem,
                quickHelpMenuItem,
			};
			menu.ItemsSource = menuItems;

			ContextMenu = menu;
		}
	}
}
