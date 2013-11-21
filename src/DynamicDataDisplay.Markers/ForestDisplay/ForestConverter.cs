using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DynamicDataDisplay.Markers.ForestDisplay;

namespace DynamicDataDisplay.Markers.MarkerGenerators
{
	/// <summary>
	/// Represents a value converter that uses mappings of tree species names to info about tree's fill or tree's crown geometry.
	/// </summary>
	public sealed class ForestConverter : IValueConverter
	{
		private readonly Dictionary<string, TreeSpeciesInfo> mappings;

		public ForestConverter(Dictionary<string, TreeSpeciesInfo> mappings)
		{
			if (mappings == null)
				throw new ArgumentNullException("mappings");

			this.mappings = mappings;
		}

		public Dictionary<string, TreeSpeciesInfo> Mappings
		{
			get { return mappings; }
		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var name = (string) value;

			TreeSpeciesInfo info;

			if (targetType == typeof (Brush))
			{
				if (mappings.TryGetValue(name, out info))
				{
					return info.Brush;
				}
			}
			else if (targetType == typeof (Geometry))
			{
				var resourceHost = (FrameworkElement) parameter;
				if (mappings.TryGetValue(name, out info))
				{
					return resourceHost.Resources[info.ViewID];
				}
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}