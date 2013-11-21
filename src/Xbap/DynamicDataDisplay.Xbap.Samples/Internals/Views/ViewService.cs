using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals.Views
{
	public static class ViewService
	{
		public static Uri GetSelectedValue(DependencyObject obj)
		{
			return (Uri)obj.GetValue(SelectedValueProperty);
		}

		public static void SetSelectedValue(DependencyObject obj, Uri value)
		{
			obj.SetValue(SelectedValueProperty, value);
		}

		public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.RegisterAttached(
		  "SelectedValue",
		  typeof(Uri),
		  typeof(ViewService),
		  new FrameworkPropertyMetadata(null, OnSelectedValueChanged));

		private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Debug.WriteLine(e.OldValue + " -> " + e.NewValue);
		}


	}
}
