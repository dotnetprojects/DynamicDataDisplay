using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DynamicDataDisplay.Markers;
using DynamicDataDisplay.Markers.DataSources;
using DynamicDataDisplay.Markers.DataSources.ValueConverters;
using DynamicDataDisplay.Markers.DataSources.ValueConvertersFactories;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Markers
{
	[ContentProperty("MarkerBuilder")]
	public partial class DevMarkerChart : PointChartBase
	{
		public DevMarkerChart()
		{
			propertyMappings[IndependentValuePathProperty] = ViewportPanel.XProperty;
			propertyMappings[DependentValuePathProperty] = ViewportPanel.YProperty;
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			DrawAllMarkers(false);
		}

		#region DataSource

		protected override void OnDataSourceChanged(PointDataSourceBase prevSource, PointDataSourceBase currSource)
		{
			base.OnDataSourceChanged(prevSource, currSource);

			if (currSource != null)
			{
				MakeStandartPredictions(currSource);
				DrawAllMarkers(true);
			}
			else
			{
				// todo clear
			}

			// this is for PieChart in legend.
			RaiseDataSourceChanged(prevSource, currSource);
		}

		private void MakeStandartPredictions(PointDataSourceBase currSource)
		{
			Type dataType = currSource.GetDataType() as Type;
			if (dataType != null)
			{
				if (dataType == typeof(Point) && DependentValuePath == null && IndependentValuePath == null)
				{
					DependentValuePath = "Y";
					IndependentValuePath = "X";
				}
			}
		}

		private bool showMarkersConsequently = true;
		public bool ShowMarkersConsequently
		{
			get { return showMarkersConsequently; }
			set { showMarkersConsequently = value; }
		}

		private bool getDataAsyncronously = false;
		public bool GetDataAsyncronously
		{
			get { return getDataAsyncronously; }
			set { getDataAsyncronously = value; }
		}

		protected virtual void DrawAllMarkers(bool reuseExisting)
		{
			DrawAllMarkers(reuseExisting, false);
		}

		protected override void DataSource_OnDataPrepaired(object sender, EventArgs e)
		{
			// invoking operation because this event may be raised in non-ui thread
			Dispatcher.BeginInvoke(() => DrawAllMarkers(true, true));
		}

		private bool IsReadyToDrawMarkers()
		{
			if (Plotter == null && DynamicDataDisplay.Plotter.GetPlotter(this) == null)
				return false;

			if (DataSource == null)
				return false;

			markerGenerator = MarkerBuilder;
			if (markerGenerator == null)
			{
				if (MarkerTemplate != null)
				{
					markerGenerator = MarkerBuilder = new TemplateMarkerGenerator(MarkerTemplate);
					return true;
				}
				else
				{
					return false;
				}
			}

			if (!markerGenerator.IsReady) return false;

			return true;
		}

		private int startIndex = 0;
		private int lastIndex = 0;
		private MarkerGenerator markerGenerator;
		protected MarkerGenerator CachedMarkerGenerator
		{
			get { return markerGenerator; }
		}
		protected virtual void DrawAllMarkers(bool reuseExisting, bool continueAfterDataPrepaired)
		{
			if (!IsReadyToDrawMarkers())
				return;

			var dataSource = DataSource;
			dataSource.Environment = new DataSourceEnvironment { Plotter = this.Plotter, FirstDraw = true };

			if (!continueAfterDataPrepaired)
			{
				dataSource.PrepairData(getDataAsyncronously);
				if (getDataAsyncronously)
				{
					LongOperationsIndicator.BeginLongOperation(this);
					return;
				}
			}

			BuildCommonBindings();

			startIndex = 0;
			lastIndex = startIndex;

			IndividualArrangePanel panel = CurrentItemsPanel as IndividualArrangePanel;
			if (panel != null)
				panel.BeginBatchAdd();

			ViewportHostPanel viewportPanel = CurrentItemsPanel as ViewportHostPanel;
			if (viewportPanel != null)
				viewportPanel.OverallViewportBounds = DataRect.Empty;

			foreach (var dataItem in dataSource.GetData())
			{
				CreateAndAddMarker(dataItem, lastIndex);
				lastIndex++;

				if (showMarkersConsequently && lastIndex % 100 == 0)
				{
					// make dispatcher execute all operations in its queue;
					// so that markers will appear on the screen step-by-step,
					// making application more responsive.
					Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
				}
			}
			if (panel != null)
				panel.EndBatchAdd();

			int len = CurrentItemsPanel.Children.Count;
			if (lastIndex < len)
			{
				for (int i = lastIndex; i < len; i++)
				{
					UIElement element = CurrentItemsPanel.Children[i];
					element.Visibility = Visibility.Collapsed;
				}
			}

			LongOperationsIndicator.EndLongOperation(this);
		}

		protected virtual void CreateAndAddMarker(object dataItem, int actualChildIndex)
		{
			FrameworkElement marker = null;
			if (actualChildIndex >= CurrentItemsPanel.Children.Count)
			{
				marker = markerGenerator.CreateMarker(dataItem);

				SetIndex(marker, actualChildIndex);

				if (MarkerStyle != null)
				{
					marker.Style = MarkerStyle;
				}

				AddCommonBindings(marker);
				AddNamedBindings(marker);

				CurrentItemsPanel.Children.Add(marker);
			}
			else
			{
				marker = (FrameworkElement)CurrentItemsPanel.Children[actualChildIndex];
				marker.Visibility = Visibility.Visible;
				marker.DataContext = dataItem;
			}
		}

		#region MarkerStyle property

		public Style MarkerStyle
		{
			get { return (Style)GetValue(MarkerStyleProperty); }
			set { SetValue(MarkerStyleProperty, value); }
		}

		public static readonly DependencyProperty MarkerStyleProperty = DependencyProperty.Register(
		  "MarkerStyle",
		  typeof(Style),
		  typeof(PointChartBase),
		  new FrameworkPropertyMetadata(null, OnMarkerStyleChanged));

		private static void OnMarkerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DevMarkerChart owner = (DevMarkerChart)d;
			owner.OnMarkerStyleChanged((Style)e.NewValue, (Style)e.OldValue);
		}

		private void OnMarkerStyleChanged(Style currStyle, Style prevStyle)
		{
			if (currStyle != null)
			{
				foreach (FrameworkElement marker in CurrentItemsPanel.Children)
				{
					marker.Style = currStyle;
				}
			}
			else
			{
				foreach (FrameworkElement marker in CurrentItemsPanel.Children)
				{
					marker.Style = (Style)TryFindResource(marker.GetType());
				}
			}
		}

		#endregion // end of MarkerStyle property

		#region MarkerTemplate property

		public DataTemplate MarkerTemplate
		{
			get { return (DataTemplate)GetValue(MarkerTemplateProperty); }
			set { SetValue(MarkerTemplateProperty, value); }
		}

		public static readonly DependencyProperty MarkerTemplateProperty = DependencyProperty.Register(
		  "MarkerTemplate",
		  typeof(DataTemplate),
		  typeof(DevMarkerChart),
		  new FrameworkPropertyMetadata(null, OnMarkerTemplateChanged));

		private static void OnMarkerTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DevMarkerChart owner = (DevMarkerChart)d;
			owner.OnMarkerTemplateChanged((DataTemplate)e.NewValue, (DataTemplate)e.OldValue);
		}

		private void OnMarkerTemplateChanged(DataTemplate currTemplate, DataTemplate prevTemplate)
		{
			if (currTemplate != null)
			{
				MarkerBuilder = new TemplateMarkerGenerator(currTemplate);
			}
		}

		#endregion // end of MarkerTemplate property

		protected override void DataSource_OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					addUpdater.Work(this, e);
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				case NotifyCollectionChangedAction.Remove:
					removeUpdater.Work(this, e);
					break;
				case NotifyCollectionChangedAction.Replace:
					break;
				case NotifyCollectionChangedAction.Reset:
					resetUpdater.Work(this, e);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private readonly List<NamedBindingInfo> namedBindings = new List<NamedBindingInfo>();
		protected sealed class NamedBindingInfo
		{
			public DependencyProperty Property { get; set; }
			public string PartName { get; set; }
			public BindingBase Binding { get; set; }
		}

		private readonly Dictionary<DependencyProperty, BindingBase> bindingInfos = new Dictionary<DependencyProperty, BindingBase>();
		protected Dictionary<DependencyProperty, BindingBase> BindingInfos
		{
			get { return bindingInfos; }
		}

		private readonly Dictionary<DependencyProperty, DependencyProperty> propertyMappings = new Dictionary<DependencyProperty, DependencyProperty>();
		protected Dictionary<DependencyProperty, DependencyProperty> PropertyMappings
		{
			get { return propertyMappings; }
		}

		bool commonBindingsAreBuilt = false;
		protected virtual void BuildCommonBindings()
		{
			if (commonBindingsAreBuilt) return;

			foreach (var pair in propertyMappings)
			{
				BuildCommonBinding(pair.Key, pair.Value);
			}

			commonBindingsAreBuilt = true;
		}

		protected void BuildCommonBinding(DependencyProperty sourceProperty, DependencyProperty targetProperty)
		{
			string mappingString = (string)GetValue(sourceProperty);

			if (String.IsNullOrEmpty(mappingString))
				return;

			PropertyDescriptor propDescriptor = CreatePropertyDescriptor(mappingString);

			IValueConverter converter = null;

			// trying to find appropriate value converter
			if (propDescriptor != null && propDescriptor.PropertyType != typeof(double))
			{
				ValueConverterFactoriesStore store = ValueConverterFactoriesStore.Current;
				converter = store.TryBuildConverter(propDescriptor.PropertyType, Plotter);
			}

			// if ValueMapping has a value
			if ((string)GetValue(sourceProperty) != (string)sourceProperty.DefaultMetadata.DefaultValue)
			{
				// try build a binding for value using ValueMapping
				var existingBinding = BindingOperations.GetBinding(this, targetProperty);
				if (true || existingBinding != null)
				{
					Binding binding = new Binding { Path = new PropertyPath((string)GetValue(sourceProperty)) };
					if (converter != null)
					{
						binding.Converter = converter;
					}

					bindingInfos[targetProperty] = binding;
				}
			}
		}

		private PropertyDescriptor CreatePropertyDescriptor(string mappingString)
		{
			var dataSource = DataSource;
			var data = dataSource.GetDataType();

			PropertyDescriptorCollection propertyDescriptors;
			Type dataType = data as Type;
			if (dataType != null)
			{
				propertyDescriptors = TypeDescriptor.GetProperties(dataType);
			}
			else
			{
				propertyDescriptors = TypeDescriptor.GetProperties(data);
			}
			PropertyDescriptor propDescriptor = propertyDescriptors.Find(mappingString, true);

			if (propDescriptor == null)
			{
				throw new InvalidOperationException(String.Format("Cannot find property \"{0}\" on ItemsSource.", mappingString));
			}
			return propDescriptor;
		}

		// todo create RemoveNamedBindings method
		protected virtual void AddNamedBindings(FrameworkElement marker)
		{
			foreach (var bindingInfo in namedBindings)
			{
				FrameworkElement namedPart = marker.FindName(bindingInfo.PartName) as FrameworkElement;
				if (namedPart != null)
				{
					namedPart.SetBinding(bindingInfo.Property, bindingInfo.Binding);
				}
			}
		}

		protected virtual void AddCommonBindings(FrameworkElement marker)
		{
			foreach (var item in bindingInfos)
			{
				marker.SetBinding(item.Key, item.Value);
			}
		}

		protected virtual void RemoveCommonBindings(FrameworkElement marker)
		{
			foreach (var item in bindingInfos)
			{
				BindingOperations.ClearBinding(marker, item.Key);
			}
		}

		public void AddPropertyBinding(DependencyProperty property, Func<object, object> expression)
		{
			Binding binding = new Binding { Path = new PropertyPath("."), Converter = new LambdaConverter(expression) };
			bindingInfos[property] = binding;
		}

		public void AddPropertyBinding<T>(DependencyProperty property, Func<T, object> expression)
		{
			Binding binding = new Binding(".") { Converter = new GenericLambdaConverter<T, object>(expression) };
			bindingInfos[property] = binding;
		}

		public void AddPropertyBinding<T>(DependencyProperty targetProperty, Func<T, object> expression, string sourcePropertyName)
		{
			Binding binding = new Binding(sourcePropertyName) { Converter = new GenericLambdaConverter<T, object>(expression) };
			bindingInfos[targetProperty] = binding;
		}

		public void AddPropertyBindingForNamedPart<T>(string partName, DependencyProperty targetProperty, Func<T, object> expression)
		{
			Binding binding = new Binding(".") { Converter = new GenericLambdaConverter<T, object>(expression) };
			namedBindings.Add(new NamedBindingInfo { PartName = partName, Property = targetProperty, Binding = binding });
		}

		public void AddMultiPropertyBinding(DependencyProperty targetProperty, Func<object[], object> expression, params string[] propertyNames)
		{
			MultiBinding multiBinding = new MultiBinding
			{
				Converter = new LambdaMultiConverter(expression)
			};

			foreach (var propertyName in propertyNames)
			{
				Binding binding = new Binding(propertyName);
				multiBinding.Bindings.Add(binding);
			}

			bindingInfos[targetProperty] = multiBinding;
		}

		#endregion // DataSource

		private void RepopulateChildren()
		{
			var dataSource = ItemsSource;
			if (dataSource == null)
				return;
		}

		#region Properties

		public DataTemplate MarkerBuilderTemplate
		{
			get { return (DataTemplate)GetValue(MarkerBuilderTemplateProperty); }
			set { SetValue(MarkerBuilderTemplateProperty, value); }
		}

		public static readonly DependencyProperty MarkerBuilderTemplateProperty = DependencyProperty.Register(
		  "MarkerBuilderTemplate",
		  typeof(DataTemplate),
		  typeof(DevMarkerChart),
		  new FrameworkPropertyMetadata(null, OnMarkerBuilderTemplateChanged));

		private static void OnMarkerBuilderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DevMarkerChart owner = (DevMarkerChart)d;
			// owner.MarkerBuilder = new TemplateMarkerGenerator((DataTemplate)e.NewValue);
		}

		public MarkerGenerator MarkerBuilder
		{
			get { return (MarkerGenerator)GetValue(MarkerBuilderProperty); }
			set { SetValue(MarkerBuilderProperty, value); }
		}

		public static readonly DependencyProperty MarkerBuilderProperty = DependencyProperty.Register(
		  "MarkerBuilder",
		  typeof(MarkerGenerator),
		  typeof(DevMarkerChart),
		  new FrameworkPropertyMetadata(null, OnMarkerBuilderChanged));

		private static void OnMarkerBuilderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DevMarkerChart owner = (DevMarkerChart)d;
			owner.OnMarkerGeneratorChanged((MarkerGenerator)e.OldValue, (MarkerGenerator)e.NewValue);

			owner.MarkerBuilderChanged.Raise(owner, e.OldValue, e.NewValue);
		}

		public event EventHandler<ValueChangedEventArgs<MarkerGenerator>> MarkerBuilderChanged;

		protected virtual void OnMarkerGeneratorChanged(MarkerGenerator prevGenerator, MarkerGenerator currGenerator)
		{
			if (prevGenerator != null)
			{
				RemoveLogicalChild(prevGenerator);
				prevGenerator.Dispose();
				prevGenerator.Changed -= MarkerBuilder_OnChanged;
			}
			if (currGenerator != null)
			{
				AddLogicalChild(currGenerator);
				currGenerator.Changed += MarkerBuilder_OnChanged;

				CurrentItemsPanel.Children.Clear();
				DrawAllMarkers(false);
			}
		}

		protected virtual void MarkerBuilder_OnChanged(object sender, EventArgs e)
		{
			CurrentItemsPanel.Children.Clear();
			DrawAllMarkers(false);
		}

		#endregion
	}
}
