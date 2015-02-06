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
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay
{
    [ContentProperty("AxisContent")]
    public class VerticalAxisTitle : ContentControl, IPlotterElement
    {
        private Plotter parentPlotter;
        private Canvas canvas;
        private Size measuredSize;
        private TextBlock content;
        private bool alreadyLoaded = false;

        protected override Size ArrangeOverride(Size finalSize)
        {
            FrameworkElement elem = content as FrameworkElement;
            TransformGroup transformation = new TransformGroup();
            RotateTransform rotation = new RotateTransform();
            TranslateTransform translation = new TranslateTransform();
            translation.Y = DesiredSize.Height;
            rotation.Angle = -90;
            transformation.Children.Add(rotation);
            transformation.Children.Add(translation);
            canvas.Arrange(new Rect(new Point(0,0), DesiredSize));
            RenderTransform = transformation;
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size returnSize = new Size();
            content.Measure(availableSize);
            returnSize.Width = Math.Min(availableSize.Width, content.ActualHeight);
            returnSize.Height = Math.Min(availableSize.Height, content.ActualWidth);
            measuredSize = returnSize;
            return returnSize;
        }

        public TextBlock AxisContent
        {
            get {
                return content;
            }
            set {
                content = value;
                Content = canvas;
            }
        }

        public VerticalAxisTitle() {
            canvas = new Canvas();
            VerticalAlignment = VerticalAlignment.Center;            
        }

        /// <summary>
        /// Create VerticalAxisTitle with specific <paramref name="cintent"/> textblock ax a content
        /// </summary>
        /// <param name="content">TextBlock which will be used as a title content</param>
        public VerticalAxisTitle(TextBlock content) {
            
            canvas = new Canvas();
            VerticalAlignment = VerticalAlignment.Center;
            AxisContent = content;
        }


        #region IPlotterElement Members

        public void OnPlotterAttached(Plotter plotter)
        {
            if (parentPlotter == null)
            {
                if (content == null)
                    throw new Exception("Content of the Vertical axis should be set befor adding vertical axis to the plotter");
                if (!alreadyLoaded)
                {
                    canvas.Children.Add(content);
                    alreadyLoaded = true;
                }
                parentPlotter = plotter;
                plotter.LeftPanel.Children.Insert(0,this);
            }
        }

        public void OnPlotterDetaching(Plotter plotter)
        {
            if (parentPlotter != null)
            {
                parentPlotter = null;
                plotter.LeftPanel.Children.Remove(this);
            }
        }

        public Plotter Plotter
        {
            get { return parentPlotter; }
        }

        #endregion
    }
}
