using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    /// <summary>
    /// Represents a label provider for major ticks of <see cref="System.DateTime"/> type.
    /// </summary>
    public class MajorDateTimeLabelProvider : DateTimeLabelProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MajorDateTimeLabelProvider"/> class.
        /// </summary>
        public MajorDateTimeLabelProvider() { }

        public override UIElement[] CreateLabels(ITicksInfo<DateTime> ticksInfo)
        {
            object info = ticksInfo.Info;
            var ticks = ticksInfo.Ticks;
            UIElement[] res = new UIElement[ticks.Length - 1];
            int labelsNum = 3;

            if (info is DifferenceIn)
            {
                DifferenceIn diff = (DifferenceIn)info;
                DateFormat = GetDateFormat(diff);
            }
            else if (info is MajorLabelsInfo)
            {
                MajorLabelsInfo mInfo = (MajorLabelsInfo)info;
                DifferenceIn diff = (DifferenceIn)mInfo.Info;
                DateFormat = GetDateFormat(diff);
                labelsNum = mInfo.MajorLabelsCount + 1;

                //DebugVerify.Is(labelsNum < 100);
            }

            DebugVerify.Is(ticks.Length < 10);

            LabelTickInfo<DateTime> tickInfo = new LabelTickInfo<DateTime>();
            for (int i = 0; i < ticks.Length - 1; i++)
            {
                tickInfo.Info = info;
                tickInfo.Tick = ticks[i];

                string tickText = GetString(tickInfo);

                Grid grid = new Grid { };

                // doing binding as described at http://sdolha.spaces.live.com/blog/cns!4121802308C5AB4E!3724.entry?wa=wsignin1.0&sa=835372863

                grid.SetBinding(Grid.BackgroundProperty, new Binding { Path = new PropertyPath("(0)", DateTimeAxis.MajorLabelBackgroundBrushProperty), RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) { AncestorType = typeof(AxisControlBase) } });
                Rectangle rect = new Rectangle
                {
                    StrokeThickness = 2
                };
                rect.SetBinding(Rectangle.StrokeProperty, new Binding { Path = new PropertyPath("(0)", DateTimeAxis.MajorLabelRectangleBorderPropertyProperty), RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) { AncestorType = typeof(AxisControlBase) } });

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

            return res;
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
