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
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay
{
    /// <summary>ViewportElement2D is intended to be a child of Viewport2D. Specifics
    /// of ViewportElement2D is Viewport2D attached property</summary>
    public abstract class ViewportElement2D : PlotterElement, INotifyPropertyChanged, IPlotterElement
    {
        protected ViewportElement2D() {
        }

        #region ContentBounds

        public static Rect GetContentBounds(DependencyObject obj)
        {
            return (Rect)obj.GetValue(ContentBoundsProperty);
        }

        public static void SetContentBounds(DependencyObject obj, Rect value)
        {
            obj.SetValue(ContentBoundsProperty, value);
        }

        public static readonly DependencyProperty ContentBoundsProperty =
            DependencyProperty.RegisterAttached(
                "ContentBounds",
                typeof(Rect),
                typeof(ViewportElement2D),
                new PropertyMetadata(
                    Rect.Empty,
                    OnContentBoundsChanged));

        // todo сделать привязку вьюпорта к контентбаундс.
        private static void OnContentBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViewportElement2D vElement = (ViewportElement2D)d;
            if (vElement.viewport != null)
            {
                // was uncommented
                ((Viewport2D)vElement.viewport).UpdateVisible();
            }
        }

        public Rect ContentBounds
        {
            get { return (Rect)GetValue(ContentBoundsProperty); }
            set
            {
                // todo probably use SetValueAsync here
                SetValue(ContentBoundsProperty, value);
            }
        }

        #endregion

        protected virtual Panel GetHostPanel(Plotter plotter)
        {
            return plotter.CentralGrid;
        }

        public override void OnPlotterAttached(Plotter plotter)
        {
            if (Plotter2D == null)
            {
                base.OnPlotterAttached(plotter);

                plotter2D = (Plotter2D)plotter;
                viewport = plotter2D.Viewport;
                viewport.OutputChanged += new EventHandler<RectChangedEventArgs>(viewport_OutputChanged);
                viewport.VisibleChanged += new EventHandler<RectChangedEventArgs>(viewport_VisibleChanged);
            }
        }

        void viewport_VisibleChanged(object sender, RectChangedEventArgs e)
        {
            OnVisibleChanged(e.newRect, e.oldRect);
        }

        void viewport_OutputChanged(object sender, RectChangedEventArgs e)
        {
            OnOutputChanged(e.newRect, e.oldRect);
        }


        public override void OnPlotterDetaching(Plotter plotter)
        {
            if (Plotter2D != null)
            {
                base.OnPlotterDetaching(plotter);

                viewport.OutputChanged -= viewport_OutputChanged;
                viewport.VisibleChanged -= viewport_VisibleChanged;
                viewport = null;
                //GetHostPanel(plotter).Children.Remove(this);
                plotter2D = null;
            }
        }

        private Plotter2D plotter2D = null;
        protected Plotter2D Plotter2D
        {
            get { return plotter2D; }
        }

        //public int ZIndex
        //{
        //    get { return Panel.GetZIndex(this); }
        //    set { Panel.SetZIndex(this, value); }
        //}

        #region Viewport

        private Viewport2D viewport;
        protected Viewport2D Viewport
        {
            get { return viewport; }
        }

        #endregion

        protected virtual void OnVisibleChanged(Rect newRect, Rect oldRect)
        {
            UpdateCore();
        }

        protected virtual void OnOutputChanged(Rect newRect, Rect oldRect)
        {
            UpdateCore();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is translated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is translated; otherwise, <c>false</c>.
        /// </value>
       

        #region IsLevel

        public bool IsLayer
        {
            get { return (bool)GetValue(IsLayerProperty); }
            set { SetValue(IsLayerProperty, value); }
        }

        public static readonly DependencyProperty IsLayerProperty =
            DependencyProperty.Register(
            "IsLayer",
            typeof(bool),
            typeof(ViewportElement2D),
            new PropertyMetadata(
                false
                ));

        #endregion

        #region Rendering & caching options

        /*protected object GetValueSync(DependencyProperty property)
        {
            return Dispatcher.Invoke(
                          DispatcherPriority.Send,
                           (DispatcherOperationCallback)delegate { return GetValue(property); },
                            property);
        }

        protected void SetValueAsync(DependencyProperty property, object value)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                (SendOrPostCallback)delegate { SetValue(property, value); },
                value);
        }*/

        private bool manualClip;
        /// <summary>
        /// Gets or sets a value indicating whether descendant graph class 
        /// relies on autotic clipping by Viewport.Output or
        /// does its own clipping.
        /// </summary>
        public bool ManualClip
        {
            get { return manualClip; }
            set { manualClip = value; }
        }

        private bool manualTranslate;
        /// <summary>
        /// Gets or sets a value indicating whether descendant graph class
        /// relies on automatic translation of it, or does its own.
        /// </summary>
        public bool ManualTranslate
        {
            get { return manualTranslate; }
            set { manualTranslate = value; }
        }

        private RenderTo renderTarget = RenderTo.Screen;
        /// <summary>
        /// Gets or sets a value indicating whether descendant graph class 
        /// uses cached rendering of its content to image, or not.
        /// </summary>
        public RenderTo RenderTarget
        {
            get { return renderTarget; }
            set { renderTarget = value; }
        }

        private enum ImageKind
        {
            Real,
            BeingRendered,
            Empty
        }

        #endregion

        private RenderState CreateRenderState(Rect renderVisible, RenderTo renderingType)
        {
            Rect output = Viewport.Output;

            return new RenderState(renderVisible, Viewport.Visible,
                output,
                renderingType);
        }

       
        

        protected void InvalidateVisual()
        {

        }

        protected virtual void UpdateCore() { }

        protected void TranslateVisual()
        {
            InvalidateVisual();
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
