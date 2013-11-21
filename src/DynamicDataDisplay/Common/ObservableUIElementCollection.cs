using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay
{
    public sealed class UIElementCollectionChangedEventArgs : EventArgs
    {
        private readonly UIElement element;

        public UIElementCollectionChangedEventArgs(UIElement element)
        {
            this.element = element;
        }

        public UIElement Element
        {
            get
            {
                return element;
            }
        }
    }
}