using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
    [ContentProperty("MyContent")]
    public partial class TransparentButton : UserControl
    {
        public String MyContent {
            set {
                TExtBlockMain.Text = value;
            }
            get {
                return TExtBlockMain.Text;
            }
        }

        public void UpdateWidth(double alowedWidth) {

            if (alowedWidth != Double.NaN && alowedWidth != Double.PositiveInfinity && alowedWidth != Width)
                Width = alowedWidth;
        }

        public TransparentButton()
        {
            InitializeComponent();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size toReturn = base.MeasureOverride(availableSize);
            return new Size(availableSize.Width, Math.Min(toReturn.Height,availableSize.Height));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size toReturn = base.ArrangeOverride(finalSize);
            return toReturn;
        }
    }
}
