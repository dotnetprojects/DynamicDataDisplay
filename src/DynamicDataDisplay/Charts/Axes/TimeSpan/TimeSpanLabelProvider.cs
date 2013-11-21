using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class TimeSpanLabelProvider : LabelProviderBase<TimeSpan>
	{
		public override UIElement[] CreateLabels(ITicksInfo<TimeSpan> ticksInfo)
		{
			object info = ticksInfo.Info;
			var ticks = ticksInfo.Ticks;

			LabelTickInfo<TimeSpan> tickInfo = new LabelTickInfo<TimeSpan>();

			UIElement[] res = new UIElement[ticks.Length];
			for (int i = 0; i < ticks.Length; i++)
			{
				tickInfo.Tick = ticks[i];
				tickInfo.Info = info;

				string tickText = GetString(tickInfo);
				UIElement label = new TextBlock { Text = tickText, ToolTip = ticks[i] };
				res[i] = label;
			}
			return res;
		}
	}
}
