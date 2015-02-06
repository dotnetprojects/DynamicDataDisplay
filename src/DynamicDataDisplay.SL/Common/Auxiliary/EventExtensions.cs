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

namespace Microsoft.Research.DynamicDataDisplay
{
    internal static class EventExtensions
    {
        public static void Raise<T>(this EventHandler<T> @event, object sender, T args) where T : EventArgs
        {
            if (@event != null)
            {
                @event(sender, args);
            }
        }

        public static void Raise(this EventHandler @event, object sender)
        {
            if (@event != null)
            {
                @event(sender, EventArgs.Empty);
            }
        }

        public static void Raise(this EventHandler @event, object sender, EventArgs args)
        {
            if (@event != null)
            {
                @event(sender, args);
            }
        }
    }
}
