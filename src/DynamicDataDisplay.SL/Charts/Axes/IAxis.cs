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
    public interface IAxis : IPlotterElement
    {
        AxisPlacement Placement { get; }
        event EventHandler TicksChanged;
        double[] ScreenTicks { get; }
        MinorTickInfo<double>[] MinorScreenTicks { get; }
    }
}
