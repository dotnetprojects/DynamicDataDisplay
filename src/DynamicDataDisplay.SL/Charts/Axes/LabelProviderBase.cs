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
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
    /// <summary>
    /// Contains data for custom generation of tick's label.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class LabelTickInfo<T>
    {
        internal LabelTickInfo() { }

        /// <summary>
        /// Gets or sets the tick.
        /// </summary>
        /// <value>The tick.</value>
        public T Tick { get; internal set; }
        /// <summary>
        /// Gets or sets the additional info about range.
        /// </summary>
        /// <value>The info.</value>
        public object Info { get; internal set; }
        /// <summary>
        /// Gets or sets the index of tick in ticks array.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; internal set; }
    }

    /// <summary>
    /// Base class for all label providers.
    /// Contains a number of properties that can be used to adjust generated labels.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LabelProviderBase<T>
    {
        /// <summary>
        /// Creates the labels by given ticks info.
        /// </summary>
        /// <param name="ticksInfo">The ticks info.</param>
        /// <returns></returns>
        public abstract List<FrameworkElement> CreateLabels(ITicksInfo<T> ticksInfo);

        
        /// <summary>
        /// Gets or sets the label string format.
        /// </summary>
        /// <value>The label string format.</value>
        public string LabelStringFormat { get; set; }
        /// <summary>
        /// Gets or sets the custom formatter - delegate that can be called to create custom string representation of tick.
        /// </summary>
        /// <value>The custom formatter.</value>
        public Func<LabelTickInfo<T>, string> CustomFormatter { get; set; }
        /// <summary>
        /// Gets or sets the custom view - delegate that is used to create a custom, non-default look of axis label.
        /// Can be used to adjust some properties of generated label.
        /// </summary>
        /// <value>The custom view.</value>
        public Action<LabelTickInfo<T>, UIElement> CustomView { get; set; }

        /// <summary>
        /// Sets the custom formatter.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        public void SetCustomFormatter(Func<LabelTickInfo<T>, string> formatter)
        {
            CustomFormatter = formatter;
        }

        /// <summary>
        /// Sets the custom view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void SetCustomView(Action<LabelTickInfo<T>, UIElement> view)
        {
            CustomView = view;
        }

        protected virtual string GetString(LabelTickInfo<T> tickInfo)
        {
            string text = null;
            if (CustomFormatter != null)
            {
                text = CustomFormatter(tickInfo);
            }
            if (text == null)
            {
                text = GetStringCore(tickInfo);

                if (text == null)
                    throw new ArgumentNullException();
            }
            if (LabelStringFormat != null)
            {
                text = String.Format(LabelStringFormat, text);
            }

            return text;
        }

        protected virtual string GetStringCore(LabelTickInfo<T> tickInfo)
        {
            return tickInfo.Tick.ToString();
        }

        protected void ApplyCustomView(LabelTickInfo<T> info, UIElement label)
        {
            if (CustomView != null)
            {
                CustomView(info, label);
            }
        }

        //protected int allocatedLabelsCount;
        //protected List<FrameworkElement> allocatedLabelsList = new List<FrameworkElement>();


        /// <summary>
        /// Occurs when label provider is changed.
        /// Notifies axis to update its view.
        /// </summary>
        public event EventHandler Changed;
        protected void RaiseChanged()
        {
            Changed.Raise(this);
        }
    }
}
