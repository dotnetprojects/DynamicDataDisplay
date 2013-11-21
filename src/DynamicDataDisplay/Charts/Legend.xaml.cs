using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Legend - shows list of annotations to charts.
	/// </summary>
	public partial class Legend : ContentControl, IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Legend"/> class.
		/// </summary>
		public Legend()
		{
			InitializeComponent();

#if !RELEASEXBAP
			shadowRect.Effect = new DropShadowEffect { Direction = 300, ShadowDepth = 3, Opacity = 0.4 };
#endif
		}

		#region Position properties

		public double LegendLeft
		{
			get { return (double)GetValue(LegendLeftProperty); }
			set { SetValue(LegendLeftProperty, value); }
		}

		public static readonly DependencyProperty LegendLeftProperty = DependencyProperty.Register(
		  "LegendLeft",
		  typeof(double),
		  typeof(Legend),
		  new FrameworkPropertyMetadata(Double.NaN));

		public double LegendRight
		{
			get { return (double)GetValue(LegendRightProperty); }
			set { SetValue(LegendRightProperty, value); }
		}

		public static readonly DependencyProperty LegendRightProperty = DependencyProperty.Register(
		  "LegendRight",
		  typeof(double),
		  typeof(Legend),
		  new FrameworkPropertyMetadata(10.0));

		public double LegendBottom
		{
			get { return (double)GetValue(LegendBottomProperty); }
			set { SetValue(LegendBottomProperty, value); }
		}

		public static readonly DependencyProperty LegendBottomProperty = DependencyProperty.Register(
		  "LegendBottom",
		  typeof(double),
		  typeof(Legend),
		  new FrameworkPropertyMetadata(Double.NaN));

		public double LegendTop
		{
			get { return (double)GetValue(LegendTopProperty); }
			set { SetValue(LegendTopProperty, value); }
		}

		public static readonly DependencyProperty LegendTopProperty = DependencyProperty.Register(
		  "LegendTop",
		  typeof(double),
		  typeof(Legend),
		  new FrameworkPropertyMetadata(10.0));

		#endregion

		public override bool ShouldSerializeContent()
		{
			return false;
		}

		#region Plotter attached & detached

		private Plotter plotter;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Plotter Plotter
		{
			get { return plotter; }
		}

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;
			plotter.Children.CollectionChanged += OnPlotterChildrenChanged;
			plotter.CentralGrid.Children.Add(this);

			SubscribeOnEvents();
			PopulateLegend();
		}

		private void SubscribeOnEvents()
		{
			foreach (var item in plotter.Children.OfType<INotifyPropertyChanged>())
			{
				item.PropertyChanged += OnChartPropertyChanged;
			}
		}

		private void OnPlotterChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ManageEvents(e);

			PopulateLegend();
		}

		private void ManageEvents(NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems.OfType<INotifyPropertyChanged>())
				{
					item.PropertyChanged -= OnChartPropertyChanged;
				}
			}
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems.OfType<INotifyPropertyChanged>())
				{
					item.PropertyChanged += OnChartPropertyChanged;
				}
			}
		}

		private void OnChartPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Description")
			{
				ViewportElement2D chart = sender as ViewportElement2D;
				if (chart != null && cachedLegendItems.ContainsKey(chart))
				{
					// todo dirty, but quick to code.
					PopulateLegend();
				}
			}
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			UnsubscribeFromEvents();
			plotter.CentralGrid.Children.Remove(this);
			plotter.Children.CollectionChanged -= OnPlotterChildrenChanged;

			this.plotter = null;

			PopulateLegend();
		}

		private void UnsubscribeFromEvents()
		{
			foreach (var item in plotter.Children.OfType<INotifyPropertyChanged>())
			{
				item.PropertyChanged -= OnChartPropertyChanged;
			}
		}

		#endregion

		public Grid ContentGrid
		{
			get { return grid; }
		}

		public Panel ContentPanel
		{
			get { return stackPanel; }
		}

		private bool autoShowAndHide = true;
		/// <summary>
		/// Gets or sets a value indicating whether legend automatically shows or hides itself
		/// when chart collection changes.
		/// </summary>
		/// <value><c>true</c> if legend automatically shows and hides itself when chart collection changes; otherwise, <c>false</c>.</value>
		public bool AutoShowAndHide
		{
			get { return autoShowAndHide; }
			set { autoShowAndHide = value; }
		}

		/// <summary>
		/// Adds new legend item.
		/// </summary>
		/// <param name="legendItem">The legend item.</param>
		public void AddLegendItem(LegendItem legendItem)
		{
			stackPanel.Children.Add(legendItem);
			UpdateVisibility();
		}

		/// <summary>
		/// Removes the legend item.
		/// </summary>	
		/// <param name="legendItem">The legend item.</param>
		public void RemoveLegendItem(LegendItem legendItem)
		{
			stackPanel.Children.Remove(legendItem);
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			if (stackPanel.Children.Count > 0 && ReadLocalValue(VisibilityProperty) == DependencyProperty.UnsetValue && autoShowAndHide == true)
			{
				Visibility = Visibility.Visible;
			}
			else if (stackPanel.Children.Count == 0 && ReadLocalValue(VisibilityProperty) != DependencyProperty.UnsetValue && autoShowAndHide == true)
			{
				Visibility = Visibility.Hidden;
			}
		}

		private readonly Dictionary<ViewportElement2D, LegendItem> cachedLegendItems = new Dictionary<ViewportElement2D, LegendItem>();

		private void ParentChartPlotter_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			stackPanel.Children.Clear();
			PopulateLegend();
		}

		private void graph_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Description")
			{
				ViewportElement2D graph = (ViewportElement2D)sender;
				LegendItem oldLegendItem = cachedLegendItems[graph];
				int index = stackPanel.Children.IndexOf(oldLegendItem);
				stackPanel.Children.RemoveAt(index);

				LegendItem newLegendItem = graph.Description.LegendItem;
				cachedLegendItems[graph] = newLegendItem;
				stackPanel.Children.Insert(index, newLegendItem);
			}
		}

		public void PopulateLegend()
		{
			stackPanel.Children.Clear();

			if (plotter == null)
			{
				return;
			}

			cachedLegendItems.Clear();
			foreach (var graph in plotter.Children.OfType<ViewportElement2D>())
			{
				if (GetVisibleInLegend(graph))
				{
					LegendItem legendItem = graph.Description.LegendItem;
					cachedLegendItems.Add(graph, legendItem);
					AddLegendItem(legendItem);
				}
			}

			UpdateVisibility();
		}

		#region VisibleInLegend attached dependency property

		public static bool GetVisibleInLegend(DependencyObject obj)
		{
			return (bool)obj.GetValue(VisibleInLegendProperty);
		}

		public static void SetVisibleInLegend(DependencyObject obj, bool value)
		{
			obj.SetValue(VisibleInLegendProperty, value);
		}

		public static readonly DependencyProperty VisibleInLegendProperty =
			DependencyProperty.RegisterAttached(
			"VisibleInLegend",
			typeof(bool),
			typeof(Legend), new FrameworkPropertyMetadata(false, OnVisibleInLegendChanged));

		private static void OnVisibleInLegendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ChartPlotter plotter = Plotter.GetPlotter(d) as ChartPlotter;
			if (plotter != null)
			{
				plotter.Legend.PopulateLegend();
			}
		}

		#endregion
	}
}
