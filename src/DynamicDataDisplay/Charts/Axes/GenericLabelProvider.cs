using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
	/// <summary>
	/// Represents default implementation of label provider for specified type.
	/// </summary>
	/// <typeparam name="T">Axis values type.</typeparam>
	public class GenericLabelProvider<T> : LabelProviderBase<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericLabelProvider&lt;T&gt;"/> class.
		/// </summary>
		public GenericLabelProvider() { }

		#region ILabelProvider<T> Members

		/// <summary>
		/// Creates the labels by given ticks info.
		/// </summary>
		/// <param name="ticksInfo">The ticks info.</param>
		/// <returns>
		/// Array of <see cref="UIElement"/>s, which are axis labels for specified axis ticks.
		/// </returns>
		public override UIElement[] CreateLabels(ITicksInfo<T> ticksInfo)
		{
			var ticks = ticksInfo.Ticks;
			var info = ticksInfo.Info;

			LabelTickInfo<T> tickInfo = new LabelTickInfo<T>();
			UIElement[] res = new UIElement[ticks.Length];
			for (int i = 0; i < res.Length; i++)
			{
				tickInfo.Tick = ticks[i];
				tickInfo.Info = info;

				string text = GetString(tickInfo);

				res[i] = new TextBlock
				{
					Text = text,
					ToolTip = ticks[i].ToString()
				};
			}
			return res;
		}

		#endregion
	}
}
