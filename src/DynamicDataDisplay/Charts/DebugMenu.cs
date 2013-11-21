using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents a menu that appears in Debug version of DynamicDataDisplay.
	/// </summary>
	public class DebugMenu : IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DebugMenu"/> class.
		/// </summary>
		public DebugMenu()
		{
			Panel.SetZIndex(menu, 1);
		}

		private Plotter plotter;
		private readonly Menu menu = new Menu
		{
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Top,
			Margin = new Thickness(3)
		};
		public Menu Menu
		{
			get { return menu; }
		}

		public MenuItem TryFindMenuItem(string itemName)
		{
			return menu.Items.OfType<MenuItem>().Where(item => item.Header == itemName).FirstOrDefault();
		}

		#region IPlotterElement Members

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;
			plotter.CentralGrid.Children.Add(menu);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			plotter.CentralGrid.Children.Remove(menu);
			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
