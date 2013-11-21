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

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals.Views
{
	/// <summary>
	/// Interaction logic for FlatDemonstrationView.xaml
	/// </summary>
	public partial class TreeDemonstrationView : ViewBase
	{
		public TreeDemonstrationView()
		{
			InitializeComponent();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			//tree.SetBinding(TreeView.SelectedValueProperty, new Binding { Source = ViewState.State, Path = new PropertyPath("SelectedValue") });
			SetBinding(SelectedValueProperty, new Binding { Source = tree, Path = new PropertyPath("SelectedValue") });
		}

	}
}
