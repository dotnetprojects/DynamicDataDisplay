using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public sealed class ExponentialLabelProvider : NumericLabelProviderBase
    {
        private int allocatedTextBlocksCount = 0;
        private List<TextBlock> allocatedTextBlocksList = new List<TextBlock>();
        private List<FrameworkElement> allocatedFrameworkElementsList = new List<FrameworkElement>();

        LabelTickInfo<double> tickInfo;
        public ExponentialLabelProvider() {
            tickInfo = new LabelTickInfo<double>();
        }

        public override List<FrameworkElement> CreateLabels(ITicksInfo<double> ticksInfo)
        {
            tickInfo.Info = ticksInfo.Info;
            var ticks = ticksInfo.Ticks;

            Init(ticks);

            int numOfTicks=ticks.Length;
            for (int i = 0; i < numOfTicks; i++)
            {
                tickInfo.Tick = ticks[i];
                tickInfo.Index = i;

                string label = GetString(tickInfo);

                if (i < allocatedTextBlocksCount)
                {
                    allocatedTextBlocksList[i].Text=label;
                    }
                else {
                    TextBlock t = new TextBlock() { Text = label };
                    allocatedTextBlocksList.Add(t);
                    allocatedFrameworkElementsList.Add(t);
                }
                
                ApplyCustomView(tickInfo, allocatedTextBlocksList[i]);
            }
            if (allocatedTextBlocksCount > numOfTicks) {
                allocatedTextBlocksList.RemoveRange(ticks.Length, allocatedTextBlocksCount - numOfTicks);
                allocatedFrameworkElementsList.RemoveRange(ticks.Length, allocatedTextBlocksCount - numOfTicks);
            }
            allocatedTextBlocksCount = numOfTicks;

            return allocatedFrameworkElementsList;
        }
    }
}
