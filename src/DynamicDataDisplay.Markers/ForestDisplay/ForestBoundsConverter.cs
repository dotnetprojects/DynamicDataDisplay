using System;
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Converters;

namespace DynamicDataDisplay.Markers.MarkerGenerators
{
	/// <summary>
	/// Represents a converter that converts tree's geometric parameters into tree's bounds in viewport space.
	/// </summary>
	public class ForestBoundsConverter : FourValuesMultiConverter<double, double, double, double>
	{
		protected override object ConvertCore(double value1, double value2, double value3, double value4, Type targetType,
		                                      object parameter, CultureInfo culture)
		{
			double crownHeight = value1;
			double trunkHeight = value2;
			double crownWidth = value3;
			double x = value4;

			double halfWidth = crownWidth/2;

			DataRect result = DataRect.Create(x - halfWidth, 0, x + halfWidth, trunkHeight + crownHeight);
			return result;
		}
	}
}