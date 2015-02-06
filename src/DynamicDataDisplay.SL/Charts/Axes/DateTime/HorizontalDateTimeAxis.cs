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
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay
{
    public class HorizontalDateTimeAxis : DateTimeAxis
    {
        public HorizontalDateTimeAxis()
        {
            Placement = AxisPlacement.Bottom;
        }

        protected override void ValidatePlacement(AxisPlacement newPlacement)
        {
            if (newPlacement == AxisPlacement.Left || newPlacement == AxisPlacement.Right)
                throw new ArgumentException();
        }
    }
}
