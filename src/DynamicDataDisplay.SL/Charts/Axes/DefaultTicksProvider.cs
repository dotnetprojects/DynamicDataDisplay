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
    internal static class DefaultTicksProvider
    {
        internal static readonly int DefaultTicksCount = 10;

        internal static ITicksInfo<T> GetTicks<T>(this ITicksProvider<T> provider, Range<T> range)
        {
            return provider.GetTicks(range, DefaultTicksCount);
        }
    }
}
