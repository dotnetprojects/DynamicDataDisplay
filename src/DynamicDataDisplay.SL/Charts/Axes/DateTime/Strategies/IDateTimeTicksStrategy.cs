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
    public interface IDateTimeTicksStrategy
    {
        DifferenceIn GetDifference(TimeSpan span);
        bool TryGetLowerDiff(DifferenceIn diff, out DifferenceIn lowerDiff);
        bool TryGetBiggerDiff(DifferenceIn diff, out DifferenceIn biggerDiff);
    }
}
