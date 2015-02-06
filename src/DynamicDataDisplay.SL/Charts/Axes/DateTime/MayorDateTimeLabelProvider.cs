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
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public class MayorDateTimeLabelProvider : DateTimeLabelProviderBase
    {
        public override List<FrameworkElement> CreateLabels(ITicksInfo<DateTime> ticksInfo)
        {
            object info = ticksInfo.Info;
            var ticks = ticksInfo.Ticks;
            FrameworkElement[] res = new FrameworkElement[ticks.Length - 1];
            int labelsNum = 3;

            if (info is DifferenceIn)
            {
                DifferenceIn diff = (DifferenceIn)info;
                DateFormat = GetDateFormat(diff);
            }
            else if (info is MayorLabelsInfo)
            {
                MayorLabelsInfo mInfo = (MayorLabelsInfo)info;
                DifferenceIn diff = (DifferenceIn)mInfo.Info;
                DateFormat = GetDateFormat(diff);
                labelsNum = mInfo.MayorLabelsCount + 1;

                //DebugVerify.Is(labelsNum < 100);
            }

            DebugVerify.Is(ticks.Length < 10);

            LabelTickInfo<DateTime> tickInfo = new LabelTickInfo<DateTime>();
            for (int i = 0; i < ticks.Length - 1; i++)
            {
                tickInfo.Info = info;
                tickInfo.Tick = ticks[i];

                string tickText = GetString(tickInfo);

                Grid grid = new Grid
                {
                    Background = new SolidColorBrush(Colors.LightGray)
                };
                Rectangle rect = new Rectangle
                {
                    Stroke = new SolidColorBrush(Colors.Brown),
                    StrokeThickness = 2
                };
                Grid.SetColumn(rect, 0);
                Grid.SetColumnSpan(rect, labelsNum);

                for (int j = 0; j < labelsNum; j++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                grid.Children.Add(rect);

                for (int j = 0; j < labelsNum; j++)
                {
                    var tb = new TextBlock
                    {
                        Text = tickText,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 3, 0, 3)
                    };
                    Grid.SetColumn(tb, j);
                    grid.Children.Add(tb);
                }

                ApplyCustomView(tickInfo, grid);

                res[i] = grid;
            }

            return new List<FrameworkElement>(res);
        }

        protected override string GetDateFormat(DifferenceIn diff)
        {
            string format = null;

            switch (diff)
            {
                case DifferenceIn.Year:
                    format = "yyyy";
                    break;
                case DifferenceIn.Month:
                    format = "MMMM yyyy";
                    break;
                case DifferenceIn.Day:
                    format = "%d MMMM yyyy";
                    break;
                case DifferenceIn.Hour:
                    format = "HH:mm %d MMMM yyyy";
                    break;
                case DifferenceIn.Minute:
                    format = "HH:mm %d MMMM yyyy";
                    break;
                case DifferenceIn.Second:
                    format = "HH:mm:ss %d MMMM yyyy";
                    break;
                case DifferenceIn.Millisecond:
                    format = "fff";
                    break;
                default:
                    break;
            }

            return format;
        }
    }
}
