using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
    public static class DependencyObjectExtensions
    {
        public static T GetValueSync<T>(this DependencyObject d, DependencyProperty property)
        {
            object value = null;
            d.Dispatcher.Invoke(() => { value = d.GetValue(property); }, DispatcherPriority.Send);

            return (T)value;
        }
    }
}
