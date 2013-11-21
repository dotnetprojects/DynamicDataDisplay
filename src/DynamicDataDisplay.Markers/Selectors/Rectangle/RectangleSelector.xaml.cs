using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	public partial class RectangleSelector : FrameworkElement, IPlotterElement
	{
		private Plotter2D plotter;
		private ViewportHostPanel renderPanel = new ViewportHostPanel();
		private RectangleSelectorModeHandler modeHandler = new TwoClicksHandler();
		private FrameworkElement rectangleControl = new Rectangle { Fill = Brushes.LightGreen };

		static RectangleSelector()
		{
			Type thisType = typeof(RectangleSelector);
			DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
		}

		public RectangleSelector()
		{
			changeModeCommand = new LambdaCommand((param) => ChangeModeExecute(param), ChangeModeCanExecute);

			renderPanel.Children.Add(rectangleControl);

			rectangleControl.SetBinding(ViewportPanel.ViewportBoundsProperty, new Binding("SelectedRectangle") { Source = this });

			InitializeComponent();
		}

		#region Commands

		private readonly LambdaCommand changeModeCommand;
		protected ICommand ChangeModeCommand
		{
			get { return changeModeCommand; }
		}

		#endregion // end of Commands

		#region Properties

		#region RectangleTemplate

		public ControlTemplate RectangleTemplate
		{
			get { return (ControlTemplate)GetValue(RectangleTemplateProperty); }
			set { SetValue(RectangleTemplateProperty, value); }
		}

		public static readonly DependencyProperty RectangleTemplateProperty = DependencyProperty.Register(
		  "RectangleTemplate",
		  typeof(ControlTemplate),
		  typeof(RectangleSelector),
		  new FrameworkPropertyMetadata(null, OnRectangleTemplateChanged));

		private static void OnRectangleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RectangleSelector owner = (RectangleSelector)d;
			owner.OnTemplateChanged();
		}

		private void OnTemplateChanged()
		{
			BindingOperations.ClearAllBindings(rectangleControl);
			renderPanel.Children.Remove(rectangleControl);

			var template = RectangleTemplate;
			if (template != null)
			{
				rectangleControl = (FrameworkElement)template.LoadContent();
				rectangleControl.SetBinding(ViewportPanel.ViewportBoundsProperty, new Binding("SelectedRectangle") { Source = this });
				renderPanel.Children.Add(rectangleControl);
			}
		}

		#endregion // end of RectangleTemplate

		#region SelectedRectangle

		public DataRect SelectedRectangle
		{
			get { return (DataRect)GetValue(SelectedRectangleProperty); }
			set { SetValue(SelectedRectangleProperty, value); }
		}

		public static readonly DependencyProperty SelectedRectangleProperty = DependencyProperty.Register(
		  "SelectedRectangle",
		  typeof(DataRect),
		  typeof(RectangleSelector),
		  new FrameworkPropertyMetadata(DataRect.Empty, OnSelectedRectangleChanged));

		private static void OnSelectedRectangleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RectangleSelector owner = (RectangleSelector)d;
			// todo 
		}

		#endregion // end of SelectedRectangle

		#region Mode

		public RectangleSelectorMode Mode
		{
			get { return (RectangleSelectorMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
		  "Mode",
		  typeof(RectangleSelectorMode),
		  typeof(RectangleSelector),
		  new FrameworkPropertyMetadata(RectangleSelectorMode.TwoClicksSelect, OnModeChanged));

		private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RectangleSelector owner = (RectangleSelector)d;
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

		private RectangleSelectorModeHandler CreateModeHandler()
		{
			switch (Mode)
			{
				case RectangleSelectorMode.ClickAndDragSelect:
					return new ClickAndDragHandler();
				case RectangleSelectorMode.None:
					return new NoneHandler();
				case RectangleSelectorMode.TwoClicksSelect:
					return new TwoClicksHandler();
				default:
					throw new InvalidOperationException();
			}
		}

		#endregion // end of Mode

		#endregion // end of Properties

		#region Command handlers

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
				var newMode = (RectangleSelectorMode)Enum.Parse(typeof(RectangleSelectorMode), modeString);
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

		#endregion // end of Command handlers

		#region IPlotterElement Members

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.Children.BeginAdd(renderPanel);
			modeHandler.Attach(this, plotter);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			modeHandler.Detach();
			plotter.Children.BeginRemove(renderPanel);
			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
