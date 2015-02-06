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
    public class LabelCreationEventArgs<T> : EventArgs
    {
        public UIElement Label { get; set; }
        public T Tick { get; set; }
        public T[] Ticks { get; set; }
        public object Info { get; set; }
    }

    public interface ILabelProvider<T>
    {
        UIElement[] CreateLabels(ITicksInfo<T> ticksInfo);
        event EventHandler<LabelCreationEventArgs<T>> PreviewLabelCreated;
        string CustomFormat { get; set; }
    }
}
