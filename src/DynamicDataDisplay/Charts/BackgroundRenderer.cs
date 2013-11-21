using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public static class BackgroundRenderer
    {
        public static readonly RoutedEvent RenderingFinished = EventManager.RegisterRoutedEvent(
            "RenderingFinished",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BackgroundRenderer));

        public static void RaiseRenderingFinished(FrameworkElement eventSource)
        {
            eventSource.RaiseEvent(new RoutedEventArgs(RenderingFinished));
        }

        public static readonly RoutedEvent UpdateRequested = EventManager.RegisterRoutedEvent(
            "UpdateRequested",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BackgroundRenderer));

        public static void RaiseUpdateRequested(FrameworkElement eventSource)
        {
            eventSource.RaiseEvent(new RoutedEventArgs(UpdateRequested));
        }

        #region UsesBackgroundRendering

        public static bool GetUsesBackgroundRendering(DependencyObject obj)
        {
            return (bool)obj.GetValue(UsesBackgroundRenderingProperty);
        }

        public static void SetUsesBackgroundRendering(DependencyObject obj, bool value)
        {
            obj.SetValue(UsesBackgroundRenderingProperty, value);
        }

        public static readonly DependencyProperty UsesBackgroundRenderingProperty = DependencyProperty.RegisterAttached(
          "UsesBackgroundRendering",
          typeof(bool),
          typeof(BackgroundRenderer),
          new FrameworkPropertyMetadata(false));

        #endregion // end of UsesBackgroundRendering
    }
}
