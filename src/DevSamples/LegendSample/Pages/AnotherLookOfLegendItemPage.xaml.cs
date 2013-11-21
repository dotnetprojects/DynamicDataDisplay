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

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace LegendSample.Pages
{
	/// <summary>
	/// Interaction logic for AnotherLookOfLegendItemPage.xaml
	/// </summary>
	public partial class AnotherLookOfLegendItemPage : Page
	{
		public AnotherLookOfLegendItemPage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			plotter.AddLineGraph(DataSourceHelper.CreateDataSource(x => x != 0 ? 10 * Math.Sin(x) / x : 1));


			var line = plotter.AddLineGraph(DataSourceHelper.CreateDataSource(x => Math.Sqrt(x)));
			// this will set different look to line chart's legend item - text from the left and line segment from the right
			NewLegend.SetLegendItemsBuilder(line, new LegendItemsBuilder(CreateDifferentlyLookingLegendItem));

			// creating and adding to plotter line chart's description editor
			StackPanel textEditingPanel = new StackPanel { Orientation = Orientation.Horizontal, Background = Brushes.LightGray };
			TextBlock label = new TextBlock { Text = "Chart's legend description:", Margin = new Thickness(5, 10, 0, 10), VerticalAlignment = VerticalAlignment.Center };
			TextBox editor = new TextBox { DataContext = line, Margin = new Thickness(5, 10, 5, 10) };
			editor.SetBinding(TextBox.TextProperty, new Binding { Path = new PropertyPath("(0)", NewLegend.DescriptionProperty), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

			textEditingPanel.Children.Add(label);
			textEditingPanel.Children.Add(editor);
			plotter.Children.Add(textEditingPanel);
		}

		private IEnumerable<FrameworkElement> CreateDifferentlyLookingLegendItem(IPlotterElement plotterElement)
		{
			StackPanel host = new StackPanel { Orientation = Orientation.Horizontal };

			TextBlock textBlock = new TextBlock();
			textBlock.SetBinding(TextBlock.TextProperty, BindingHelper.CreateAttachedPropertyBinding(NewLegend.DescriptionProperty));
			host.Children.Add(textBlock);

			Line line = new Line { X1 = 0, Y1 = 0, X2 = 20, Y2 = 10, Stretch = Stretch.Fill };
			line.SetBinding(Line.StrokeProperty, new Binding("Stroke"));
			line.SetBinding(Line.StrokeThicknessProperty, new Binding("StrokeThickness"));
			host.Children.Add(line);

			host.DataContext = plotterElement;

			yield return host;
		}
	}
}
