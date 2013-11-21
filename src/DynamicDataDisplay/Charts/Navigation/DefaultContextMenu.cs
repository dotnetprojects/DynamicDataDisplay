using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
	/// <summary>
	/// Is responsible for displaying and populating default context menu of ChartPlotter
	/// </summary>
	public class DefaultContextMenu : IPlotterElement
	{
		private static readonly BitmapImage helpIcon;
		private static readonly BitmapImage copyScreenshotIcon;
		private static readonly BitmapImage saveScreenshotIcon;
		private static readonly BitmapImage fitToViewIcon;
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
			icon.Freeze();

			return icon;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultContextMenu"/> class.
		/// </summary>
		public DefaultContextMenu() { }

		protected ContextMenu PopulateContextMenu(Plotter target)
		{
			ContextMenu menu = new ContextMenu();
			MenuItem fitToViewMenuItem = new MenuItem
			{
				Header = Strings.UIResources.ContextMenuFitToView,
				ToolTip = Strings.UIResources.ContextMenuFitToViewTooltip,
				Icon = new Image { Source = fitToViewIcon },
				Command = ChartCommands.FitToView,
				CommandTarget = target
			};

			MenuItem savePictureMenuItem = new MenuItem
			{
				Header = Strings.UIResources.ContextMenuSaveScreenshot,
				ToolTip = Strings.UIResources.ContextMenuSaveScreenshotTooltip,
				Icon = new Image { Source = saveScreenshotIcon },
				Command = ChartCommands.SaveScreenshot,
				CommandTarget = target
			};

			MenuItem copyPictureMenuItem = new MenuItem
			{
				Header = Strings.UIResources.ContextMenuCopyScreenshot,
				ToolTip = Strings.UIResources.ContextMenuCopyScreenshotTooltip,
				Icon = new Image { Source = copyScreenshotIcon },
				Command = ChartCommands.CopyScreenshot,
				CommandTarget = target
			};

			MenuItem quickHelpMenuItem = new MenuItem
			{
				Header = Strings.UIResources.ContextMenuQuickHelp,
				ToolTip = Strings.UIResources.ContextMenuQuickHelpTooltip,
				Command = ChartCommands.ShowHelp,
				Icon = new Image { Source = helpIcon },
				CommandTarget = target
			};

			MenuItem reportFeedback = new MenuItem
			{
				Header = Strings.UIResources.ContextMenuReportFeedback,
				ToolTip = Strings.UIResources.ContextMenuReportFeedbackTooltip,
				Icon = (Image)plotter.Resources["SendFeedbackIcon"]
			};
			reportFeedback.Click += reportFeedback_Click;

			staticMenuItems.Add(fitToViewMenuItem);
			staticMenuItems.Add(copyPictureMenuItem);
			staticMenuItems.Add(savePictureMenuItem);
			staticMenuItems.Add(quickHelpMenuItem);
			staticMenuItems.Add(reportFeedback);

			menu.ItemsSource = staticMenuItems;

			return menu;
		}

		private void reportFeedback_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (Process.Start("mailto:" + Strings.UIResources.SendFeedbackEmail + "?Subject=[D3]%20" + typeof(DefaultContextMenu).Assembly.GetName().Version)) { }
			}
			catch (Exception)
			{
				MessageBox.Show(Strings.UIResources.SendFeedbackError + Strings.UIResources.SendFeedbackEmail, "Error while sending feedback", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private readonly ObservableCollection<object> staticMenuItems = new ObservableCollection<object>();

		// hidden because default menu items' command target is plotter, and serializing this will
		// cause circular reference

		/// <summary>
		/// Gets the static menu items.
		/// </summary>
		/// <value>The static menu items.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<object> StaticMenuItems
		{
			get { return staticMenuItems; }
		}

		#region IPlotterElement Members

		private Plotter2D plotter;
		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;

			ContextMenu menu = PopulateContextMenu(plotter);
			plotter.ContextMenu = menu;

			plotter.PreviewMouseRightButtonDown += plotter_PreviewMouseRightButtonDown;
			plotter.PreviewMouseRightButtonUp += plotter_PreviewMouseRightButtonUp;
			plotter.PreviewMouseLeftButtonDown += plotter_PreviewMouseLeftButtonDown;

			plotter.ContextMenu.Closed += ContextMenu_Closed;
		}

		private void plotter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// this will prevent other tools like PointSelector from
			// wrong actuations
			if (contextMenuOpen)
			{
				plotter.Focus();
				e.Handled = true;
			}
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			plotter.ContextMenu.Closed -= ContextMenu_Closed;

			plotter.ContextMenu = null;
			plotter.PreviewMouseRightButtonDown -= plotter_PreviewMouseRightButtonDown;
			plotter.PreviewMouseRightButtonUp -= plotter_PreviewMouseRightButtonUp;

			this.plotter = null;
		}

		private void ContextMenu_Closed(object sender, RoutedEventArgs e)
		{
			contextMenuOpen = false;
			foreach (var item in dynamicMenuItems)
			{
				staticMenuItems.Remove(item);
			}
		}

		private Point mousePos;
		/// <summary>
		/// Gets the mouse position when right mouse button was clicked.
		/// </summary>
		/// <value>The mouse position on click.</value>
		public Point MousePositionOnClick
		{
			get { return mousePos; }
		}

		private void plotter_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			contextMenuOpen = false;
			mousePos = e.GetPosition(plotter);
		}

		private bool contextMenuOpen = false;
		private readonly ObservableCollection<object> dynamicMenuItems = new ObservableCollection<object>();
		private void plotter_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			Point position = e.GetPosition(plotter);
			if (mousePos == position)
			{
				hitResults.Clear();
				VisualTreeHelper.HitTest(plotter, null, CollectAllVisuals_Callback, new PointHitTestParameters(position));

				foreach (var item in dynamicMenuItems)
				{
					staticMenuItems.Remove(item);
				}
				dynamicMenuItems.Clear();
				var dynamicItems = (hitResults.Where(r =>
				{
					IPlotterContextMenuSource menuSource = r as IPlotterContextMenuSource;
					if (menuSource != null)
					{
						menuSource.BuildMenu();
					}

					var items = GetPlotterContextMenu(r);
					return items != null && items.Count > 0;
				}).SelectMany(r =>
				{
					var menuItems = GetPlotterContextMenu(r);

					FrameworkElement chart = r as FrameworkElement;
					if (chart != null)
					{
						foreach (var menuItem in menuItems.OfType<MenuItem>())
						{
							menuItem.DataContext = chart.DataContext;
						}
					}
					return menuItems;
				})).ToList();

				foreach (var item in dynamicItems)
				{
					dynamicMenuItems.Add(item);
					//MenuItem menuItem = item as MenuItem;
					//if (menuItem != null)
					//{
					//    menuItem.CommandTarget = plotter;
					//}
				}

				staticMenuItems.AddMany(dynamicMenuItems);

				plotter.Focus();
				contextMenuOpen = true;

				// in XBAP applications these lines throws a SecurityException, as (as I think)
				// we are opening "new window" here. So in XBAP we are not opening context menu manually, but
				// it will be opened by itself as this is right click.
#if !RELEASEXBAP
				plotter.ContextMenu.IsOpen = true;
				e.Handled = true;
#endif
			}
			else
			{
				// this is to prevent showing menu when RMB was pressed, then moved and now is releasing.
				e.Handled = true;
			}
		}

		#region PlotterContextMenu property

		/// <summary>
		/// Gets the plotter context menu.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static ObjectCollection GetPlotterContextMenu(DependencyObject obj)
		{
			return (ObjectCollection)obj.GetValue(PlotterContextMenuProperty);
		}

		/// <summary>
		/// Sets the plotter context menu.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="value">The value.</param>
		public static void SetPlotterContextMenu(DependencyObject obj, ObjectCollection value)
		{
			obj.SetValue(PlotterContextMenuProperty, value);
		}

		/// <summary>
		/// Identifies the PlotterContextMenu attached property.
		/// </summary>
		public static readonly DependencyProperty PlotterContextMenuProperty = DependencyProperty.RegisterAttached(
		  "PlotterContextMenu",
		  typeof(ObjectCollection),
		  typeof(DefaultContextMenu),
		  new FrameworkPropertyMetadata(null));

		#endregion

		private List<DependencyObject> hitResults = new List<DependencyObject>();
		private HitTestResultBehavior CollectAllVisuals_Callback(HitTestResult result)
		{
			if (result == null || result.VisualHit == null)
				return HitTestResultBehavior.Stop;

			hitResults.Add(result.VisualHit);
			return HitTestResultBehavior.Continue;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion
	}

	/// <summary>
	/// Represents a collection of additional menu items in ChartPlotter's context menu.
	/// </summary>
	public sealed class ObjectCollection : ObservableCollection<Object> { }
}
