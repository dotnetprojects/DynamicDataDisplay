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
using Microsoft.Research.DynamicDataDisplay.Samples.Internals.Models;
using Microsoft.Research.DynamicDataDisplay.Samples.Internals.ViewModels;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals.Views
{
	/// <summary>
	/// Interaction logic for FlatDemonstrationsView.xaml
	/// </summary>
	public partial class FlatDemonstrationsView : ViewBase
	{
		public FlatDemonstrationsView()
		{
			InitializeComponent();
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null)
			{
				SamplesCollection collection = (SamplesCollection)e.NewValue;

				var query = from r in collection.Releases
							from d in r.Demonstrations
							select new DemonstrationViewModel { Demonstration = d, Version = r.Version };
				FlatViewModel viewModel = new FlatViewModel(query.ToList());

				itemsControl.DataContext = viewModel.Demonstrations;
			}
		}
	}
}
