using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DynamicDataDisplay.Markers;
using Microsoft.Research.DynamicDataDisplay.Charts.Markers;
using Microsoft.Research.DynamicDataDisplay.Markers.Strings;
using Microsoft.Research.DynamicDataDisplay.Navigation;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	/// <summary>
	/// Represents a Plotter element for selecting a number of Points Of Interest (POI).
	/// </summary>
	public partial class PointSelector : FrameworkElement, IPlotterElement
	{
		private readonly SelectorObservableCollection<Point> points = new SelectorObservableCollection<Point>();
		private Plotter2D plotter;
		private DevMarkerChart markerChart = new DevMarkerChart();
		private PointSelectorModeHandler modeHandler = new AddPointHandler();
		private MenuItem addPointMenuItem = new MenuItem { Header = UIResources.PointSelector_AddPoint };

		static PointSelector()
		{
			Type thisType = typeof(PointSelector);
			DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
		}

		#region Commands

		protected readonly LambdaCommand removePointCommand;
		public ICommand RemovePointCommand
		{
			get { return removePointCommand; }
		}

		protected readonly LambdaCommand changeModeCommand;
		public ICommand ChangeModeCommand
		{
			get { return changeModeCommand; }
		}

		protected readonly LambdaCommand addPointCommand;
		public ICommand AddPointCommand
		{
			get { return addPointCommand; }
		}

		#endregion // end of Commands

		public PointSelector()
		{
			markerChart.SetBinding(Panel.ZIndexProperty, new Binding("(Panel.ZIndex)") { Source = this });

			// initializing built-in commands
			removePointCommand = new LambdaCommand((param) => RemovePointExecute(param), RemovePointCanExecute);
			changeModeCommand = new LambdaCommand((param) => ChangeModeExecute(param), ChangeModeCanExecute);
			addPointCommand = new LambdaCommand((param) => AddPointExecute(param), AddPointCanExecute);

			InitializeComponent();

			// adding context menu binding to markers
			markerChart.AddPropertyBinding(DefaultContextMenu.PlotterContextMenuProperty, data =>
			{
				ObjectCollection menuItems = new ObjectCollection();

				MenuItem item = new MenuItem { Header = UIResources.PointSelector_RemovePoint, Command = PointSelectorCommands.RemovePoint, CommandTarget = this };
				item.SetBinding(MenuItem.CommandParameterProperty, new Binding());
				menuItems.Add(item);

				return menuItems;
			});

			// loading marker template
			var markerTemplate = (DataTemplate)Resources["markerTemplate"];
			markerChart.MarkerBuilder = new TemplateMarkerGenerator(markerTemplate);
			markerChart.ItemsSource = points;

			// adding bindings to commands from PointSelectorCommands static class
			CommandBinding removePointBinding = new CommandBinding(
				PointSelectorCommands.RemovePoint,
				RemovePointExecute,
				RemovePointCanExecute
				);
			CommandBindings.Add(removePointBinding);

			CommandBinding changeModeBinding = new CommandBinding(
				PointSelectorCommands.ChangeMode,
				ChangeModeExecute,
				ChangeModeCanExecute);
			CommandBindings.Add(changeModeBinding);

			CommandBinding addPointBinding = new CommandBinding(
				PointSelectorCommands.AddPoint,
				AddPointExecute,
				AddPointCanExecute);
			CommandBindings.Add(addPointBinding);

			// init add point menu item
			addPointMenuItem.Click += addPointMenuItem_Click;

			points.CollectionChanged += points_CollectionChanged;
		}

		void points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			addPointCommand.RaiseCanExecuteChanged();
			removePointCommand.RaiseCanExecuteChanged();
		}

		private void addPointMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (plotter == null)
				return;

			var plotterContextMenu = plotter.Children.OfType<DefaultContextMenu>().FirstOrDefault();
			if (plotterContextMenu == null) return;

			var mousePos = plotterContextMenu.MousePositionOnClick;

			mousePos = plotter.TranslatePoint(mousePos, plotter.CentralGrid);

			var transform = plotter.Transform;
			var viewportPoint = mousePos.ScreenToViewport(transform);

			object parameter = viewportPoint;
			if (addPointCommand.CanExecute(parameter))
			{
				addPointCommand.Execute(parameter);
				e.Handled = true;
			}
		}

		#region Properties

		/// <summary>
		/// Gets the marker chart, used to render points of interest.
		/// </summary>
		/// <value>The marker chart.</value>
		public DevMarkerChart MarkerChart
		{
			get { return markerChart; }
		}

		/// <summary>
		/// Gets points of interest.
		/// </summary>
		/// <value>The points.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ObservableCollection<Point> Points
		{
			get { return points; }
		}

		protected SelectorObservableCollection<Point> ProtectedPoints
		{
			get { return points; }
		}

		/// <summary>
		/// Gets or sets PointSelector's mode.
		/// </summary>
		/// <value>The mode.</value>
		public PointSelectorMode Mode
		{
			get { return (PointSelectorMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
		  "Mode",
		  typeof(PointSelectorMode),
		  typeof(PointSelector),
		  new FrameworkPropertyMetadata(PointSelectorMode.Add, OnModeChanged));

		private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PointSelector owner = (PointSelector)d;
			owner.UpdateModeHandler();
		}

		private void UpdateModeHandler()
		{
			if (modeHandler != null && modeHandler.IsAttached)
				modeHandler.Detach();

			modeHandler = CreateModeHandler();
			if (plotter != null)
				modeHandler.Attach(this, plotter);

			changeModeCommand.RaiseCanExecuteChanged();
		}

		private PointSelectorModeHandler CreateModeHandler()
		{
			switch (Mode)
			{
				case PointSelectorMode.Add:
					return new AddPointHandler();
				case PointSelectorMode.MultipleSelect:
					return new MultipleSelectHandler();
				case PointSelectorMode.Remove:
					return new RemovePointHandler();
				case PointSelectorMode.None:
					return new NoneModeHandler();
				default:
					throw new ArgumentException("Mode");
					break;
			}
		}

		#endregion // end of Properties

		#region RemovePoint command

		private void RemovePointExecute(object target, ExecutedRoutedEventArgs e)
		{
			e.Handled = RemovePointExecute(e.Parameter);
		}

		private bool RemovePointExecute(object parameter)
		{
			if (parameter is Point)
			{
				Point pointToRemove = (Point)parameter;
				var index = points.IndexOf(pointToRemove);
				if (index >= 0)
				{
					points.Remove(pointToRemove);
					//plotter.UndoProvider.AddAction(new LambdaUndoAction(() => points.Remove(pointToRemove), () => points.Insert(index, pointToRemove)));
					return true;
				}
			}
			return false;
		}

		private void RemovePointCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = RemovePointCanExecute(e.Parameter);
		}

		private bool RemovePointCanExecute(object parameter)
		{
			if (parameter is Point)
			{
				//Point pointToRemove = (Point)parameter;
				//if (points.Contains(pointToRemove))
				return true;
			}
			return false;
		}

		#endregion // end of RemovePoint command

		#region ChangeMode command

		private void ChangeModeExecute(object target, ExecutedRoutedEventArgs e)
		{
			e.Handled = ChangeModeExecute(e.Parameter);
		}

		private bool ChangeModeExecute(object parameter)
		{
			if (parameter is string)
			{
				var modeString = (string)parameter;
				var newMode = (PointSelectorMode)Enum.Parse(typeof(PointSelectorMode), modeString);
				Mode = newMode;
				return true;
			}
			return false;
		}

		private void ChangeModeCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.Handled = ChangeModeCanExecute(e.Parameter);
		}

		private bool ChangeModeCanExecute(object parameter)
		{
			if (parameter is string)
			{
				var modeString = (string)parameter;
				var currentModeString = Mode.ToString();
				if (modeString != currentModeString)
					return true;
			}
			return false;
		}

		#endregion // end of ChangeMode command

		#region AddPoint command

		private void AddPointExecute(object target, ExecutedRoutedEventArgs e)
		{
			e.Handled = AddPointExecute(e.Parameter);
		}

		protected virtual bool AddPointExecute(object parameter)
		{
			if (parameter is Point)
			{
				Point pointToAdd = (Point)parameter;
				points.Add(pointToAdd);
				return true;
			}
			else if (parameter is string)
			{
				try
				{
					string pointString = (string)parameter;
					Point point = Point.Parse(pointString);
					points.Add(point);
					return true;
				}
				// these are exceptions thrown by Point.Parse
				catch (FormatException exc) { }
				catch (InvalidOperationException exc) { }
			}
			return false;
		}

		private void AddPointCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = AddPointCanExecute(e.Parameter);
		}

		protected virtual bool AddPointCanExecute(object parameter)
		{
			return parameter is Point || parameter is string;
		}

		#endregion // end of AddPoint command

		#region IPlotterElement Members

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;

			plotter.CentralGrid.Children.Add(this);
			modeHandler.Attach(this, plotter);

			markerChart.OnPlotterAttached(plotter);

			var plotterContextMenu = plotter.Children.OfType<DefaultContextMenu>().FirstOrDefault();
			if (plotterContextMenu != null)
			{
				plotterContextMenu.StaticMenuItems.Add(addPointMenuItem);
			}
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			// todo markerChart.OnPlotterDetached(plotter);

			var plotterContextMenu = plotter.Children.OfType<DefaultContextMenu>().FirstOrDefault();
			if (plotterContextMenu != null)
			{
				plotterContextMenu.StaticMenuItems.Remove(addPointMenuItem);
			}

			modeHandler.Detach();
			plotter.CentralGrid.Children.Remove(this);

			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion

		protected virtual void OnPoint_PositionChanged(object sender, PositionChangedEventArgs e)
		{
			FrameworkElement marker = (FrameworkElement)sender;
			var index = DevMarkerChart.GetIndex(marker);

			marker.DataContext = e.Position;

			if (0 <= index && index < points.Count && !points.Changing)
			{
				points[index] = e.Position;
			}
		}
	}
}
