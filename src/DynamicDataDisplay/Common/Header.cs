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
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay
{
	public class Header : ContentControl, IPlotterElement
	{
		public Header()
		{
			FontSize = 18;
			HorizontalAlignment = HorizontalAlignment.Center;
			Margin = new Thickness(0, 0, 0, 3);
		}

		private Plotter plotter;
		public Plotter Plotter
		{
			get { return plotter; }
		}
		
		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;
			plotter.HeaderPanel.Children.Add(this);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter = null;
			plotter.HeaderPanel.Children.Remove(this);
		}
	}
}