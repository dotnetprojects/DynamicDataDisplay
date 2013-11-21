using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DynamicDataDisplay.Markers
{
	public static class StockItemStyles
	{
		private static Style candleStick = null;
		public static Style CandleStick
		{
			get
			{
				if (candleStick == null)
				{
					ResourceDictionary genericDict = (ResourceDictionary)Application.LoadComponent(new Uri("/DynamicDataDisplay.Markers;component/Themes/Generic.xaml", UriKind.Relative));
					candleStick = (Style)genericDict["candleStickStyle"];
				}
				return candleStick;
			}
		}

		private static Style defaultStyle = null;
		public static Style Default
		{
			get
			{
				if (defaultStyle == null)
				{
					ResourceDictionary genericDict = (ResourceDictionary)Application.LoadComponent(new Uri("/DynamicDataDisplay.Markers;component/Themes/Generic.xaml", UriKind.Relative));
					defaultStyle = (Style)genericDict[typeof(StockItem)];
				}
				return defaultStyle;
			}
		}
	}
}
