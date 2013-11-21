using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public static class LegendStyles
	{
		private static Style defaultStyle;
		public static Style Default
		{
			get
			{
				if (defaultStyle == null)
				{
					var legendStyles = GetLegendStyles();
					defaultStyle = (Style)legendStyles[typeof(NewLegend)];
				}

				return defaultStyle;
			}
		}

		private static Style noScrollStyle;
		public static Style NoScroll
		{
			get
			{
				if (noScrollStyle == null)
				{
					var legendStyles = GetLegendStyles();
					noScrollStyle = (Style)legendStyles["NoScrollLegendStyle"];
				}

				return noScrollStyle;
			}
		}

		private static ResourceDictionary GetLegendStyles()
		{
			var legendStyles = (ResourceDictionary)Application.LoadComponent(new Uri("/DynamicDataDisplay;component/Charts/Legend items/LegendResources.xaml", UriKind.Relative));
			return legendStyles;
		}
	}
}
