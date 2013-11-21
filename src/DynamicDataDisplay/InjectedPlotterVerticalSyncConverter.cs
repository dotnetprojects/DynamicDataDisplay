using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Converters;
using System.Globalization;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Represents a converter of Viewport.Visible for InjectedPlotter.
	/// Keeps horizontal part of visible from injectedPlotter and takes vertical part of visible rectangle from hosting Plotter.
	/// </summary>
	public sealed class InjectedPlotterVerticalSyncConverter : GenericValueConverter<DataRect>
	{
		private readonly InjectedPlotter injectedPlotter;
		public InjectedPlotterVerticalSyncConverter(InjectedPlotter plotter)
		{
			if (plotter == null)
				throw new ArgumentNullException("plotter");

			this.injectedPlotter = plotter;
		}

		public override object ConvertCore(DataRect value, Type targetType, object parameter, CultureInfo culture)
		{
			if (injectedPlotter.Plotter == null)
				return DependencyProperty.UnsetValue;

			var outerVisible = value;
			var innerVisible = injectedPlotter.Visible;
			return new DataRect(innerVisible.XMin, outerVisible.YMin, innerVisible.Width, outerVisible.Height);
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is DataRect)
			{
				DataRect innerVisible = (DataRect)value;
				var outerVisible = injectedPlotter.Plotter.Visible;
				return outerVisible;
			}
			else
			{
				return DependencyProperty.UnsetValue;
			}
		}
	}
}
