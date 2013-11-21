using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents an axis label provider for double ticks, generating labels with numbers in exponential form when it is appropriate.
	/// </summary>
	public sealed class ExponentialLabelProvider : NumericLabelProviderBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExponentialLabelProvider"/> class.
		/// </summary>
		public ExponentialLabelProvider() { }

		/// <summary>
		/// Creates labels by given ticks info.
		/// Is not intended to be called from your code.
		/// </summary>
		/// <param name="ticksInfo">The ticks info.</param>
		/// <returns>
		/// Array of <see cref="UIElement"/>s, which are axis labels for specified axis ticks.
		/// </returns>
		public override UIElement[] CreateLabels(ITicksInfo<double> ticksInfo)
		{
			var ticks = ticksInfo.Ticks;

			Init(ticks);

			UIElement[] res = new UIElement[ticks.Length];

			LabelTickInfo<double> tickInfo = new LabelTickInfo<double> { Info = ticksInfo.Info };

			for (int i = 0; i < res.Length; i++)
			{
				var tick = ticks[i];
				tickInfo.Tick = tick;
				tickInfo.Index = i;

				string labelText = GetString(tickInfo);

				TextBlock label;
				if (labelText.Contains('E'))
				{
					string[] substrs = labelText.Split('E');
					string mantissa = substrs[0];
					string exponenta = substrs[1];
					exponenta = exponenta.TrimStart('+');
					Span span = new Span();
					span.Inlines.Add(String.Format(CultureInfo.CurrentCulture, "{0}·10", mantissa));
					Span exponentaSpan = new Span(new Run(exponenta));
					exponentaSpan.BaselineAlignment = BaselineAlignment.Superscript;
					exponentaSpan.FontSize = 8;
					span.Inlines.Add(exponentaSpan);

					label = new TextBlock(span);
					LabelProviderProperties.SetExponentialIsCommonLabel(label, false);
				}
				else
				{
					label = (TextBlock)GetResourceFromPool();
					if (label == null)
					{
						label = new TextBlock();
					}

					label.Text = labelText;
				}
				res[i] = label;
				label.ToolTip = tick.ToString(CultureInfo.CurrentCulture);

				ApplyCustomView(tickInfo, label);
			}

			return res;
		}

		protected override bool ReleaseCore(UIElement label)
		{
			bool isNotExponential = LabelProviderProperties.GetExponentialIsCommonLabel(label);
			return isNotExponential && CustomView == null;
		}
	}
}
