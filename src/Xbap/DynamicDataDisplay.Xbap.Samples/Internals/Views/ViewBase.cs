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
	/// Interaction logic for ViewBase.xaml
	/// </summary>
	public class ViewBase : UserControl
	{
		public ViewBase()
		{
		}

		public object SelectedValue
		{
			get { return (object)GetValue(SelectedValueProperty); }
			set { SetValue(SelectedValueProperty, value); }
		}

		public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
		  "SelectedValue",
		  typeof(object),
		  typeof(ViewBase),
		  new FrameworkPropertyMetadata(null, OnSelectedValueChanged));

		private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{

		}
	}
}
