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
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
    [ContentProperty("MainContent")]
    public partial class ScrollWraper : ContentControl
    {
        private FrameworkElement content = null;

        public FrameworkElement MainContent {
            get {
                return content;
            }
            set {
                if (content != null)
                {
                    LayoutRoot.Children.Remove(content);
                    content.SizeChanged -= content_SizeChanged;
                }
                content = value;
                if(value!=null) {
                    LayoutRoot.Children.Add(value);
                    value.SizeChanged += new SizeChangedEventHandler(content_SizeChanged);
                }
            }
        }

        void content_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            offset = 0;
            InvalidateMeasure();
        }

        private TransparentButton up = new TransparentButton() { MyContent = " ▲ ", Height = 15, HorizontalAlignment = HorizontalAlignment.Center };
        private TransparentButton down = new TransparentButton() { MyContent = " ▼ ", Height = 15, HorizontalAlignment = HorizontalAlignment.Center };

        bool upButtonPresents = false, downButtonPresents = false;

        private double offset = 0;

        public ScrollWraper(FrameworkElement content)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ScrollWraper_Loaded);
            MainContent = content;
        }

        public ScrollWraper() {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ScrollWraper_Loaded);
        }

        void ScrollWraper_Loaded(object sender, RoutedEventArgs e)
        {
            down.MouseLeftButtonDown += new MouseButtonEventHandler(down_MouseLeftButtonDown);
            down.VerticalAlignment = VerticalAlignment.Bottom;
            up.MouseLeftButtonDown += new MouseButtonEventHandler(up_MouseLeftButtonDown);
            up.VerticalAlignment = VerticalAlignment.Top;
        }

        void up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            offset -= 10;
            e.Handled = true;
            InvalidateMeasure();
        }

        void down_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            offset += 10;
            e.Handled = true;
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (MainContent == null) return new Size(20,20);
            UIElement uiElem = MainContent as UIElement;
            uiElem.Measure(new Size(constraint.Width,double.PositiveInfinity));

            if (upButtonPresents)
            {
                up.UpdateWidth(uiElem.DesiredSize.Width);
                up.Measure(new Size(uiElem.DesiredSize.Width, constraint.Height));
            }
            if (downButtonPresents)
                down.UpdateWidth(uiElem.DesiredSize.Width);
                down.Measure(new Size(uiElem.DesiredSize.Width, constraint.Height));

            if (uiElem.DesiredSize.Height > constraint.Height)
            { //clipping needed
                if ((!upButtonPresents) && (offset > 0))
                {
                    offset += up.Height;
                    LayoutRoot.Children.Add(up);
                    up.UpdateWidth(uiElem.DesiredSize.Width);
                    up.Measure(new Size(uiElem.DesiredSize.Width, constraint.Height));
                    upButtonPresents = true;
                    InvalidateMeasure();
                }
                else if ((upButtonPresents) && (offset <= 0))
                {
                    offset = 0;
                    LayoutRoot.Children.Remove(up);
                    upButtonPresents = false;
                    InvalidateMeasure();
                }

                if ((!downButtonPresents) && (constraint.Height  < uiElem.DesiredSize.Height-offset))
                {
                    LayoutRoot.Children.Add(down);
                    down.UpdateWidth(uiElem.DesiredSize.Width);
                    down.Measure(new Size(uiElem.DesiredSize.Width, constraint.Height));
                    downButtonPresents = true;
                    InvalidateMeasure();
                }
                else if ((downButtonPresents) && (constraint.Height >= uiElem.DesiredSize.Height - offset))
                {
                    offset += down.Height;
                    LayoutRoot.Children.Remove(down);
                    downButtonPresents = false;
                    InvalidateMeasure();
                }
            }
            else {
                if (downButtonPresents)
                {
                    offset = 0;
                    downButtonPresents = false;
                    LayoutRoot.Children.Remove(down);
                }
                if (upButtonPresents)
                {
                    offset = 0;
                    upButtonPresents = false;
                    LayoutRoot.Children.Remove(up);
                }
            }
            Size toReturn = uiElem.DesiredSize;
            //if (upButtonPresents) toReturn.Height += up.Height;
            //if (downButtonPresents) toReturn.Height += down.Height;
            return new Size(Math.Min(toReturn.Width,constraint.Width),Math.Min(toReturn.Height,constraint.Height));
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size toClip = arrangeBounds;
            if (LayoutRoot.Children.Contains(up)) {
                Canvas.SetTop(up,0);
                toClip.Height -= up.Height;
            }
            if (LayoutRoot.Children.Contains(down)) {
                Canvas.SetTop(down, arrangeBounds.Height - down.Height);
                toClip.Height -= down.Height;
            }
            Geometry clippingGeometry = new RectangleGeometry() {
                Rect= new Rect(new Point(0,offset), toClip)
            };
            MainContent.Clip = clippingGeometry;
            Canvas.SetTop(MainContent,(upButtonPresents ? (up.Height-offset) : (0)));

            return base.ArrangeOverride(arrangeBounds);
        }
    }
}
