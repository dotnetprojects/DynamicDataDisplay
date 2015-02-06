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
using System.Collections.Specialized;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common.Stubs;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay
{
    public class Viewport2D : Canvas, IViewport2D,IPlotterElement
    {
        private Rect defaultRect = new Rect(0, 0, 1, 1);

        public Viewport2D()
        {
         //   ClipToBounds = true;

            Grid.SetColumn(this, 1);
            Grid.SetRow(this, 1);
            visible = new Rect(new Point(0, 0), new Point(1, 1));
            SizeChanged += new SizeChangedEventHandler(Viewport2D_SizeChanged);
            UpdateTransform();
        }

        public event EventHandler<RectChangedEventArgs> OutputChanged;
        public event EventHandler<RectChangedEventArgs> VisibleChanged;
        public event EventHandler<EventArgs> TransformChanged;

        void Viewport2D_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Rect r = new Rect(new Point(0,0),e.NewSize);
            SetValue(OutputProperty,r);
            visible = CoerceVisible(visible);
        }


        #region Output property

        public Rect Output
        {
            get { Rect r = (Rect)GetValue(OutputProperty);
            return r;
            }
        }

        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register(
            "Output",
            typeof(Rect),
            typeof(Viewport2D),
            new PropertyMetadata(new Rect(0, 0, 0, 0), OnOutputChanged));

        private static void OnOutputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rect oldRect = (Rect)e.OldValue;
            Viewport2D viewport = (Viewport2D)d;
            viewport.UpdateTransform();
            if(viewport.OutputChanged!=null) viewport.OutputChanged(viewport, new RectChangedEventArgs(viewport.Output,oldRect));
        }

        #endregion

        private void UpdateTransform()
        {
            PerformanceCounter.startStopwatch("Updating transform");
            transform = transform.WithRects(Visible, Output);
            PerformanceCounter.stopStopwatch("Updating transform");
        }

        private double clipToBoundsFactor = 1.10;

        public bool IsFittedToView
        {
            get {
                //return true;
                return AutoFitToView && visible == null;
            }
        }

        private Rect visible;

        public Rect Visible
        {
            get { return visible; }
            set {
                Rect oldRect = visible;
                Rect newRect = this.CoerceVisible(value);
                //if (newRect.Width == 0 || newRect.Height == 0)
                //{
                    
                //    throw new Exception("Unset Value!");
                //}
                //else
                //{
                    visible = value;    
                //}
                UpdateTransform();
                if (VisibleChanged != null) VisibleChanged(this, new RectChangedEventArgs(visible,oldRect));
            }
        }

        private Rect CoerceVisible(Rect newVisible)
        {
            PerformanceCounter.startStopwatch("Coercing visible");
            if (Plotter == null)
            {
                return newVisible;
            }

            bool isDefaultValue = (newVisible == defaultRect);
            if (isDefaultValue)
            {
                newVisible = Rect.Empty;
            }

            //if (isDefaultValue && IsFittedToView)
            if(isDefaultValue)
            {
                Rect bounds = Rect.Empty;
                foreach (var g in Plotter.Children)
                {
                    var graph = g as DependencyObject;
                    if (graph != null)
                    {
                        var uiElement = g as UIElement;
                        if (uiElement == null || (uiElement != null && uiElement.Visibility == Visibility.Visible))
                        {
                            bounds.Union((Rect)graph.GetValue(ViewportElement2D.ContentBoundsProperty));
                        }
                    }
                }
                Rect viewportBounds = bounds;

                if (!bounds.IsEmpty)
                {
                    bounds = CoordinateUtilities.RectZoom(bounds, bounds.GetCenter(), clipToBoundsFactor);
                }
                else
                {
                    bounds = defaultRect;
                }
                newVisible.Union(bounds);
            }

            if (newVisible.IsEmpty)
            {
                newVisible = defaultRect;
            }
            else if (newVisible.Width == 0 || newVisible.Height == 0)
            {
                Rect defRect = defaultRect;
                Size size = new Size(newVisible.Width,newVisible.Height);
                Point loc = new Point(newVisible.X,newVisible.Y);

                if (newVisible.Width == 0)
                {
                    size.Width = defRect.Width;
                    loc.X -= size.Width / 2;
                }
                if (newVisible.Height == 0)
                {
                    size.Height = defRect.Height;
                    loc.Y -= size.Height / 2;
                }

                newVisible = new Rect(loc, size);
            }

            newVisible = ApplyRestrictions(Visible, newVisible);

            // applying transform's data domain restriction
            if (!transform.DataTransform.DataDomain.IsEmpty)
            {
                var newDataRect = newVisible.ViewportToData(transform);
                newDataRect = RectExt.Intersect(newDataRect, transform.DataTransform.DataDomain);
                newVisible = newDataRect.DataToViewport(transform);
            }

            if (newVisible.IsEmpty) return new Rect(0, 0, 1, 1);

            //RaisePropertyChangedEvent();
            PerformanceCounter.stopStopwatch("Coercing visible");
            return newVisible;
        }

        private Rect ApplyRestrictions(Rect oldVisible, Rect newVisible)
        {
            Rect res = newVisible;
         /*   foreach (var restriction in restrictions)
            {
                res = restriction.Apply(oldVisible, res, this);
            }*/

            //TODO resrictions!
            return res;
        }

        private static object OnCoerceVisible(DependencyObject d, object newValue)
        {
            Viewport2D viewport = (Viewport2D)d;

            Rect newRect = viewport.CoerceVisible((Rect)newValue);

            if (newRect.Width == 0 || newRect.Height == 0)
            {
                // doesn't apply rects with zero square
                return DependencyProperty.UnsetValue;
            }
            else
            {
                return newRect;
            }
        }

        #region Viewport changed event

        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    base.OnPropertyChanged(e);

        //    // todo добавить сюда еще свойств.
        //    if (e.Property == ActualHeightProperty ||
        //        e.Property == ActualWidthProperty)
        //    {
        //        SetValue(OutputPropertyKey, new Rect(new Size(ActualWidth, ActualHeight)));
        //        // todo uncomment or create more sophysticated logic
        //        CoerceValue(VisibleProperty);
        //    }
        //    else if (e.Property == OutputProperty || e.Property == VisibleProperty)
        //    {
        //        UpdateTransform();
        //        RaisePropertyChangedEvent(e);
        //    }
        //}

        #endregion

        public void UpdateVisible()
        {
            if (IsFittedToView)
            {
                visible =CoerceVisible(visible);
            }
        }

        public void FitToView()
        {
            Visible = CoerceVisible(defaultRect);
        }

        private CoordinateTransform transform = CoordinateTransform.CreateDefault();
        public CoordinateTransform Transform
        {
            get {
                return transform;
            }
            set
            {
                value.VerifyNotNull();

                if (value != transform)
                {
                    var oldTransform = transform;

                    transform = value;

                    if (TransformChanged != null) TransformChanged(this, null);
                }
            }
        }

        private bool autoFitToView = true;

        /// <summary>Gets or sets a value indicating whether viewport automatically clips 
        /// in its initial visible rect to bounds of graphs.</summary>
        [DefaultValue(true)]
        public bool AutoFitToView
        {
            get { return autoFitToView; }
            set
            {
                if (autoFitToView != value)
                {
                    autoFitToView = value;
                    if (value)
                    {
                        UpdateVisible();
                    }
                }
            }
        }

        private Plotter2D plotter = null;

        protected virtual Panel GetHostPanel(Plotter plotter)
        {
            return plotter.MainGrid;
        }

        #region IPlotterElement Members

        
        public void OnPlotterAttached(Plotter plotter)
        {
            if (this.plotter == null)
            {
                Plotter2D p = (Plotter2D)plotter;
                this.plotter = p;
                plotter.Children.CollectionChanged += OnPlotterChildrenChanged;
                GetHostPanel(plotter).Children.Add(this);
            }
            UpdateVisible();
        }

        private void OnPlotterChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateVisible();
        }

        public void OnPlotterDetaching(Plotter plotter)
        {
            if (this.plotter != null)
            {
                plotter.Children.CollectionChanged -= OnPlotterChildrenChanged;
                GetHostPanel(plotter).Children.Remove(this);
                this.plotter = null;
            }
        }

        Plotter IPlotterElement.Plotter { get { return plotter; } }

        public Plotter2D Plotter
        {
            get { return plotter; }
        }


        #endregion
    }

    public class RectChangedEventArgs : EventArgs
    {
        private Rect nr,or;       
        public RectChangedEventArgs(Rect newRect,Rect oldRect) {
            nr = newRect;
            or = oldRect;
        }
        public Rect oldRect {
            get {
                return or;
            }
        }

        public Rect newRect {
            get
            {
                return nr;
            }
        }
    }
}
