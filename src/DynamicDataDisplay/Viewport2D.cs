using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay
{
    /// <summary>
    /// Viewport2D provides virtual coordinates.
    /// </summary>
    public partial class Viewport2D : DependencyObject
    {
        private readonly Plotter2D plotter;
        internal Plotter2D Plotter2D
        {
            get { return plotter; }
        }

        private readonly FrameworkElement hostElement;
        internal FrameworkElement HostElement
        {
            get { return hostElement; }
        }

        protected internal Viewport2D(FrameworkElement host, Plotter2D plotter)
        {
            hostElement = host;
            host.ClipToBounds = true;
            host.SizeChanged += OnHostElementSizeChanged;

            this.plotter = plotter;
            plotter.Children.CollectionChanged += OnPlotterChildrenChanged;

            restrictions = new RestrictionCollection(this);
            restrictions.Add(new MinimalSizeRestriction());
            restrictions.CollectionChanged += restrictions_CollectionChanged;

            fitToViewRestrictions = new RestrictionCollection(this);
            fitToViewRestrictions.CollectionChanged += fitToViewRestrictions_CollectionChanged;

            readonlyContentBoundsHosts = new ReadOnlyObservableCollection<DependencyObject>(contentBoundsHosts);

            UpdateVisible();
            UpdateTransform();
        }

        private void OnHostElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetValue(OutputPropertyKey, new Rect(e.NewSize));
            CoerceValue(VisibleProperty);
        }

        private void fitToViewRestrictions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsFittedToView)
            {
                CoerceValue(VisibleProperty);
            }
        }

        private void restrictions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CoerceValue(VisibleProperty);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Viewport2D viewport = (Viewport2D)d;
            viewport.UpdateTransform();
            viewport.RaisePropertyChangedEvent(e);
        }

        public BindingExpressionBase SetBinding(DependencyProperty property, BindingBase binding)
        {
            return BindingOperations.SetBinding(this, property, binding);
        }

        /// <summary>
        /// Forces viewport to go to fit to view mode - clears locally set value of <see cref="Visible"/> property
        /// and sets it during the coercion process to a value of united content bounds of all charts inside of <see cref="Plotter"/>.
        /// </summary>
        public void FitToView()
        {
            if (!IsFittedToView)
            {
                ClearValue(VisibleProperty);
                CoerceValue(VisibleProperty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether Viewport is fitted to view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if Viewport is fitted to view; otherwise, <c>false</c>.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFittedToView
        {
            get { return ReadLocalValue(VisibleProperty) == DependencyProperty.UnsetValue; }
        }

        DispatcherOperation updateVisibleOperation;
        internal void UpdateVisible()
        {
            //if (updateVisibleOperation == null)
            //{
            //    updateVisibleOperation = Dispatcher.BeginInvoke(() => UpdateVisible(), DispatcherPriority.Normal);
            //    return;
            //}

            //updateVisibleOperation = Dispatcher.BeginInvoke(() =>
            //{
            //    updateVisibleOperation = null;

                if (IsFittedToView)
                {
                    CoerceValue(VisibleProperty);
                }
            //}, DispatcherPriority.Normal);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Plotter2D Plotter
        {
            get { return plotter; }
        }

        private readonly RestrictionCollection restrictions;
        /// <summary>
        /// Gets the collection of <see cref="ViewportRestriction"/>s that are applied each time <see cref="Visible"/> is updated.
        /// </summary>
        /// <value>The restrictions.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RestrictionCollection Restrictions
        {
            get { return restrictions; }
        }


        private readonly RestrictionCollection fitToViewRestrictions;

        /// <summary>
        /// Gets the collection of <see cref="ViewportRestriction"/>s that are applied only when Viewport is fitted to view.
        /// </summary>
        /// <value>The fit to view restrictions.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RestrictionCollection FitToViewRestrictions
        {
            get { return fitToViewRestrictions; }
        }

        #region Output property

        /// <summary>
        /// Gets the rectangle in screen coordinates that is output.
        /// </summary>
        /// <value>The output.</value>
        public Rect Output
        {
            get { return (Rect)GetValue(OutputProperty); }
        }

        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly(
            "Output",
            typeof(Rect),
            typeof(Viewport2D),
            new FrameworkPropertyMetadata(new Rect(0, 0, 1, 1), OnPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Output"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;

        #endregion

        #region UnitedContentBounds property

        /// <summary>
        /// Gets the united content bounds of all the charts.
        /// </summary>
        /// <value>The content bounds.</value>
        public DataRect UnitedContentBounds
        {
            get { return (DataRect)GetValue(UnitedContentBoundsProperty); }
            internal set { SetValue(UnitedContentBoundsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="UnitedContentBounds"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnitedContentBoundsProperty = DependencyProperty.Register(
          "UnitedContentBounds",
          typeof(DataRect),
          typeof(Viewport2D),
          new FrameworkPropertyMetadata(DataRect.Empty, OnUnitedContentBoundsChanged));

        private static void OnUnitedContentBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Viewport2D owner = (Viewport2D)d;
            owner.ContentBoundsChanged.Raise(owner);
        }

        public event EventHandler ContentBoundsChanged;

        #endregion

        #region Visible property

        /// <summary>
        /// Gets or sets the visible rectangle.
        /// </summary>
        /// <value>The visible.</value>
        public DataRect Visible
        {
            get { return (DataRect)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Visible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register("Visible", typeof(DataRect), typeof(Viewport2D),
            new FrameworkPropertyMetadata(
                new DataRect(0, 0, 1, 1),
                OnPropertyChanged,
                OnCoerceVisible),
            ValidateVisibleCallback);

        private static bool ValidateVisibleCallback(object value)
        {
            DataRect rect = (DataRect)value;

            return !rect.IsNaN();
        }

        private void UpdateContentBoundsHosts()
        {
            contentBoundsHosts.Clear();
            foreach (var item in plotter.Children)
            {
                DependencyObject dependencyObject = item as DependencyObject;
                if (dependencyObject != null)
                {
                    bool hasNonEmptyBounds = !Viewport2D.GetContentBounds(dependencyObject).IsEmpty;
                    if (hasNonEmptyBounds && Viewport2D.GetIsContentBoundsHost(dependencyObject))
                    {
                        contentBoundsHosts.Add(dependencyObject);
                    }
                }
            }

            UpdateVisible();
        }

        private readonly ObservableCollection<DependencyObject> contentBoundsHosts = new ObservableCollection<DependencyObject>();
        private readonly ReadOnlyObservableCollection<DependencyObject> readonlyContentBoundsHosts;
        /// <summary>
        /// Gets the collection of all charts that can has its own content bounds.
        /// </summary>
        /// <value>The content bounds hosts.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ReadOnlyObservableCollection<DependencyObject> ContentBoundsHosts
        {
            get { return readonlyContentBoundsHosts; }
        }

        private bool useApproximateContentBoundsComparison = true;
		/// <summary>
		/// Gets or sets a value indicating whether to use approximate content bounds comparison.
		/// This this property to true can increase performance, as Visible will change less often.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if approximate content bounds comparison is used; otherwise, <c>false</c>.
		/// </value>
        public bool UseApproximateContentBoundsComparison
        {
            get { return useApproximateContentBoundsComparison; }
            set { useApproximateContentBoundsComparison = value; }
        }

        private double maxContentBoundsComparisonMistake = 0.02;
        public double MaxContentBoundsComparisonMistake
        {
            get { return maxContentBoundsComparisonMistake; }
            set { maxContentBoundsComparisonMistake = value; }
        }

        private DataRect prevContentBounds = DataRect.Empty;
        protected virtual DataRect CoerceVisible(DataRect newVisible)
        {
            if (Plotter == null)
            {
                return newVisible;
            }

            bool isDefaultValue = newVisible == (DataRect)VisibleProperty.DefaultMetadata.DefaultValue;
            if (isDefaultValue)
            {
                newVisible = DataRect.Empty;
            }

            if (isDefaultValue && IsFittedToView)
            {
                // determining content bounds
                DataRect bounds = DataRect.Empty;

                foreach (var item in contentBoundsHosts)
                {
                    IPlotterElement plotterElement = item as IPlotterElement;
                    if (plotterElement == null)
                        continue;
                    if (plotterElement.Plotter == null)
                        continue;

                    var plotter = (Plotter2D)plotterElement.Plotter;
                    var visual = plotter.VisualBindings[plotterElement];
                    if (visual.Visibility == Visibility.Visible)
                    {
                        DataRect contentBounds = Viewport2D.GetContentBounds(item);
                        if (contentBounds.Width.IsNaN() || contentBounds.Height.IsNaN())
                            continue;

                        bounds.UnionFinite(contentBounds);
                    }
                }

                if (useApproximateContentBoundsComparison)
                {
                    var intersection = prevContentBounds;
                    intersection.Intersect(bounds);

                    double currSquare = bounds.GetSquare();
                    double prevSquare = prevContentBounds.GetSquare();
                    double intersectionSquare = intersection.GetSquare();

                    double squareTopLimit = 1 + maxContentBoundsComparisonMistake;
                    double squareBottomLimit = 1 - maxContentBoundsComparisonMistake;

                    if (intersectionSquare != 0)
                    {
                        double currRatio = currSquare / intersectionSquare;
                        double prevRatio = prevSquare / intersectionSquare;

                        if (squareBottomLimit < currRatio &&
                            currRatio < squareTopLimit &&
                            squareBottomLimit < prevRatio &&
                            prevRatio < squareTopLimit)
                        {
                            bounds = prevContentBounds;
                        }
                    }
                }

                prevContentBounds = bounds;
                UnitedContentBounds = bounds;

                // applying fit-to-view restrictions
                bounds = fitToViewRestrictions.Apply(Visible, bounds, this);

                // enlarging
                if (!bounds.IsEmpty)
                {
                    bounds = CoordinateUtilities.RectZoom(bounds, bounds.GetCenter(), clipToBoundsEnlargeFactor);
                }
                else
                {
                    bounds = (DataRect)VisibleProperty.DefaultMetadata.DefaultValue;
                }
                newVisible.Union(bounds);
            }

            if (newVisible.IsEmpty)
            {
                newVisible = (DataRect)VisibleProperty.DefaultMetadata.DefaultValue;
            }
            else if (newVisible.Width == 0 || newVisible.Height == 0)
            {
                DataRect defRect = (DataRect)VisibleProperty.DefaultMetadata.DefaultValue;
                Size size = newVisible.Size;
                Point loc = newVisible.Location;

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

                newVisible = new DataRect(loc, size);
            }

            // apply domain restriction
            newVisible = domainRestriction.Apply(Visible, newVisible, this);

            // apply other restrictions
            newVisible = restrictions.Apply(Visible, newVisible, this);

            // applying transform's data domain restriction
            if (!transform.DataTransform.DataDomain.IsEmpty)
            {
                var newDataRect = newVisible.ViewportToData(transform);
                newDataRect = DataRect.Intersect(newDataRect, transform.DataTransform.DataDomain);
                newVisible = newDataRect.DataToViewport(transform);
            }

            if (newVisible.IsEmpty) newVisible = new Rect(0, 0, 1, 1);

            return newVisible;
        }

        private static object OnCoerceVisible(DependencyObject d, object newValue)
        {
            Viewport2D viewport = (Viewport2D)d;

            DataRect newRect = viewport.CoerceVisible((DataRect)newValue);

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

        #endregion

        #region Domain

        private readonly DomainRestriction domainRestriction = new DomainRestriction { Domain = Rect.Empty };
        /// <summary>
        /// Gets or sets the domain - rectangle in viewport coordinates that limits maximal size of <see cref="Visible"/> rectangle.
        /// </summary>
        /// <value>The domain.</value>
        public DataRect Domain
        {
            get { return domainRestriction.Domain; }
            set
            {
                if (domainRestriction.Domain != value)
                {
                    domainRestriction.Domain = value;
                    DomainChanged.Raise(this);
                    CoerceValue(VisibleProperty);
                }
            }
        }

        /// <summary>
        /// Occurs when <see cref="Domain"/> property changes.
        /// </summary>
        public event EventHandler DomainChanged;

        #endregion

        private double clipToBoundsEnlargeFactor = 1.10;
        /// <summary>
        /// Gets or sets the viewport enlarge factor.
        /// </summary>
        /// <remarks>
        /// Default value is 1.10.
        /// </remarks>
        /// <value>The clip to bounds factor.</value>
        public double ClipToBoundsEnlargeFactor
        {
            get { return clipToBoundsEnlargeFactor; }
            set
            {
                if (clipToBoundsEnlargeFactor != value)
                {
                    clipToBoundsEnlargeFactor = value;
                    UpdateVisible();
                }
            }
        }

        private void UpdateTransform()
        {
            transform = transform.WithRects(Visible, Output);
        }

        private CoordinateTransform transform = CoordinateTransform.CreateDefault();
        /// <summary>
        /// Gets or sets the coordinate transform of Viewport.
        /// </summary>
        /// <value>The transform.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [NotNull]
        public virtual CoordinateTransform Transform
        {
            get { return transform; }
            set
            {
                value.VerifyNotNull();

                if (value != transform)
                {
                    var oldTransform = transform;

                    transform = value;

                    RaisePropertyChangedEvent("Transform", oldTransform, transform);
                }
            }
        }

        /// <summary>
        /// Occurs when viewport property changes.
        /// </summary>
        public event EventHandler<ExtendedPropertyChangedEventArgs> PropertyChanged;

        private void RaisePropertyChangedEvent(string propertyName, object oldValue, object newValue)
        {
            if (PropertyChanged != null)
            {
                RaisePropertyChanged(new ExtendedPropertyChangedEventArgs { PropertyName = propertyName, OldValue = oldValue, NewValue = newValue });
            }
        }

        private void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                RaisePropertyChanged(new ExtendedPropertyChangedEventArgs { PropertyName = propertyName });
            }
        }

        private void RaisePropertyChangedEvent(DependencyPropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                RaisePropertyChanged(ExtendedPropertyChangedEventArgs.FromDependencyPropertyChanged(e));
            }
        }

        //private DispatcherOperation pendingRaisePropertyChangedOperation;
        //private bool inRaisePropertyChanged = false;
        protected virtual void RaisePropertyChanged(ExtendedPropertyChangedEventArgs args)
        {
            //if (inRaisePropertyChanged)
            //{
            //    if (pendingRaisePropertyChangedOperation != null)
            //        pendingRaisePropertyChangedOperation.Abort();
            //    pendingRaisePropertyChangedOperation = Dispatcher.BeginInvoke(() => RaisePropertyChanged(args), DispatcherPriority.Normal);
            //    return;
            //}

            //pendingRaisePropertyChangedOperation = null;
            //inRaisePropertyChanged = true;

            PropertyChanged.Raise(this, args);

            //inRaisePropertyChanged = false;
        }

        private void OnPlotterChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateContentBoundsHosts();
        }

        #region Panning state

        private Viewport2DPanningState panningState = Viewport2DPanningState.NotPanning;
        public Viewport2DPanningState PanningState
        {
            get { return panningState; }
            set
            {
                var prevState = panningState;

                panningState = value;

                OnPanningStateChanged(prevState, panningState);
            }
        }

        private void OnPanningStateChanged(Viewport2DPanningState prevState, Viewport2DPanningState currState)
        {
            PanningStateChanged.Raise(this, prevState, currState);
            if (currState == Viewport2DPanningState.Panning)
                BeginPanning.Raise(this);
            else if (currState == Viewport2DPanningState.NotPanning)
                EndPanning.Raise(this);
        }

        internal event EventHandler<ValueChangedEventArgs<Viewport2DPanningState>> PanningStateChanged;

        public event EventHandler BeginPanning;
        public event EventHandler EndPanning;

        #endregion // end of Panning state
    }
}
