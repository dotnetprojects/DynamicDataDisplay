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

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public class VerticalAxis : NumericAxis
    {
        public VerticalAxis()
        {
            Placement = AxisPlacement.Left;
        }

        protected override void ValidatePlacement(AxisPlacement newPlacement)
        {
            if (newPlacement == AxisPlacement.Bottom || newPlacement == AxisPlacement.Top)
                throw new ArgumentException();
        }
    }
}
