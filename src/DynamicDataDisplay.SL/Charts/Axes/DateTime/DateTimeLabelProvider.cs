using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public class DateTimeLabelProvider : DateTimeLabelProviderBase
    {
        private LabelTickInfo<DateTime> tickInfo;

        private List<TextBlock> allocatedTextBlocksList = new List<TextBlock>();
        private List<FrameworkElement> allocatedFrameworkElementList = new List<FrameworkElement>();
        private int allocatedCount = 0;

        public DateTimeLabelProvider()
        {
            tickInfo = new LabelTickInfo<DateTime>();
        }

        public override List<FrameworkElement> CreateLabels(ITicksInfo<DateTime> ticksInfo)
        {
            object info = ticksInfo.Info;
            var ticks = ticksInfo.Ticks;

            if (info is DifferenceIn)
            {
                DifferenceIn diff = (DifferenceIn)info;
                DateFormat = GetDateFormat(diff);
            }

              tickInfo.Info = info;

            int ticksNum = ticks.Length;

            for (int i = 0; i < ticksNum; i++)
            {
                tickInfo.Tick = ticks[i];
                string tickText = GetString(tickInfo);
                TextBlock label;
                if (i < allocatedCount)
                {
                    label = allocatedTextBlocksList[i];
                }
                else
                {
                    label = new TextBlock();
                    allocatedFrameworkElementList.Add(label);
                    allocatedTextBlocksList.Add(label);
                }
                label.Text = tickText;
                ApplyCustomView(tickInfo, label);
                
            }
            if (allocatedCount > ticksNum)
            {
                allocatedFrameworkElementList.RemoveRange(ticksNum, allocatedCount - ticksNum);
                allocatedTextBlocksList.RemoveRange(ticksNum, allocatedCount - ticksNum);
            }
            allocatedCount = ticksNum;
            
            return allocatedFrameworkElementList;
        }
    }
}
