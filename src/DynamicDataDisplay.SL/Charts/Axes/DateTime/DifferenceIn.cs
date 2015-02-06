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
    public enum DifferenceIn
    {
        Biggest = Year,

        Year = 6,
        Month = 5,
        Day = 4,
        Hour = 3,
        Minute = 2,
        Second = 1,
        Millisecond = 0,

        Smallest = Millisecond
    }
}
