using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;
using DynamicDataDisplay.Markers.MarkerGenerators;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.MarkupExtensions;
using Microsoft.Research.DynamicDataDisplay.Converters;
using System.Windows.Controls;

namespace DynamicDataDisplay.Markers
{
	/// <summary>
	/// Interaction logic for ForestDisplayGenerator.xaml
	/// </summary>
	public partial class ForestDisplayGenerator : MarkerGenerator
	{
		public static readonly DependencyProperty ForestConverterProperty = DependencyProperty.Register(
			"ForestConverter",
			typeof(ForestConverter),
			typeof(ForestDisplayGenerator),
			new FrameworkPropertyMetadata(null, OnForestConverterChanged));

		private readonly DataTemplate crownTemplate;
		private readonly DataTemplate trunkTemplate;
		private MultiBinding boundsMultiBinding;

		private Binding crownHeightBinding;
		private Binding crownPathDataBinding;
		private Binding crownWidthBinding;
		private Binding crownYBinding;

		private Binding speciesBinding;
		private Binding foregroundBinding;

		private Binding fillBinding;
		private Binding strokeBinding;

		private Binding trunkHeightBinding;
		private Binding trunkWidthBinding;
		private Binding xBinding;

		public ForestDisplayGenerator()
		{
			InitializeComponent();

			CreateBindings();

			trunkTemplate = (DataTemplate)Resources["trunk"];
			crownTemplate = (DataTemplate)Resources["crown"];
		}

		internal ForestFillConverter FillConverter { get; set; }

		public ForestConverter ForestConverter
		{
			get { return (ForestConverter)GetValue(ForestConverterProperty); }
			set { SetValue(ForestConverterProperty, value); }
		}

		private void SetBindings(FrameworkElement marker, ViewportPanel panel)
		{
			panel.SetBinding(ViewportPanel.ViewportBoundsProperty, boundsMultiBinding);

			var crown = (Shape)marker;
			crown.SetBinding(ViewportPanel.ViewportWidthProperty, crownWidthBinding);
			crown.SetBinding(ViewportPanel.ViewportHeightProperty, crownHeightBinding);
			crown.SetBinding(ViewportPanel.YProperty, crownYBinding);
			crown.SetBinding(ViewportPanel.XProperty, xBinding);
			crown.SetBinding(Shape.FillProperty, fillBinding);
			crown.SetBinding(Shape.StrokeProperty, strokeBinding);
			panel.Children.Add(crown);

			var trunk = (Shape)trunkTemplate.LoadContent();
			trunk.SetBinding(ViewportPanel.XProperty, xBinding);
			trunk.SetBinding(ViewportPanel.ViewportWidthProperty, trunkWidthBinding);
			trunk.SetBinding(ViewportPanel.ViewportHeightProperty, trunkHeightBinding);
			trunk.SetBinding(Shape.FillProperty, fillBinding);
			panel.Children.Add(trunk);
		}

		private static void OnForestConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var owner = (ForestDisplayGenerator)d;
			var currConverter = e.NewValue as ForestConverter;
			if (currConverter != null)
			{
				owner.FillConverter = new ForestFillConverter(currConverter);
				owner.CreateBindings();
			}
			else
			{
				owner.FillConverter = null;
			}

			owner.OnIsReadyChanged();
		}

		public void CreateBindings()
		{
			crownWidthBinding = new Binding("CrownWidth");
			crownHeightBinding = new Binding("CrownHeight");
			crownYBinding = new Binding("TrunkHeight");
			xBinding = new Binding("X");
			fillBinding = new Binding("SpeciesID") { Converter = FillConverter };
			strokeBinding = new Binding("SpeciesID") { Converter = ForestConverter };

			trunkHeightBinding = new Binding("TrunkHeight");
			trunkWidthBinding = new Binding("TrunkWidth");

			speciesBinding = new Binding("SpeciesID");
			foregroundBinding = new SelfBinding("Background") { Converter = new BackgroundToForegroundConverter() };

			boundsMultiBinding = new MultiBinding();
			boundsMultiBinding.Bindings.Add(crownHeightBinding);
			boundsMultiBinding.Bindings.Add(trunkHeightBinding);
			boundsMultiBinding.Bindings.Add(crownWidthBinding);
			boundsMultiBinding.Bindings.Add(xBinding);
			boundsMultiBinding.Converter = new ForestBoundsConverter();

			crownPathDataBinding = new Binding("SpeciesID") { Converter = ForestConverter, ConverterParameter = this };
		}

		protected override FrameworkElement CreateMarkerCore(object dataItem)
		{
			var panel = new ViewportPanel { DataContext = dataItem };

			panel.SetBinding(ViewportPanel.ViewportBoundsProperty, boundsMultiBinding);

			var trunk = (Rectangle)trunkTemplate.LoadContent();
			trunk.DataContext = dataItem;
			trunk.SetBinding(Shape.FillProperty, fillBinding);
			trunk.SetBinding(ViewportPanel.XProperty, xBinding);
			trunk.SetBinding(ViewportPanel.ViewportWidthProperty, trunkWidthBinding);
			trunk.SetBinding(ViewportPanel.ViewportHeightProperty, trunkHeightBinding);
			trunk.SetBinding(Shape.FillProperty, fillBinding);
			panel.Children.Add(trunk);

			var crown = (Path)crownTemplate.LoadContent();
			crown.DataContext = dataItem;
			crown.SetBinding(Shape.FillProperty, fillBinding);
			crown.SetBinding(Shape.StrokeProperty, strokeBinding);
			crown.SetBinding(Path.DataProperty, crownPathDataBinding);
			crown.SetBinding(ViewportPanel.ViewportWidthProperty, crownWidthBinding);
			crown.SetBinding(ViewportPanel.ViewportHeightProperty, crownHeightBinding);
			crown.SetBinding(ViewportPanel.YProperty, crownYBinding);
			crown.SetBinding(ViewportPanel.XProperty, xBinding);
			crown.SetBinding(Shape.FillProperty, fillBinding);
			crown.SetBinding(Shape.StrokeProperty, strokeBinding);
			panel.Children.Add(crown);

			panel.SetBinding(ToolTipService.ToolTipProperty, speciesBinding);
			//LiveToolTip toolTip = new LiveToolTip();
			//toolTip.SetBinding(LiveToolTip.BackgroundProperty, fillBinding);
			//toolTip.SetBinding(LiveToolTip.ContentProperty, speciesBinding);
			//toolTip.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);
			//LiveToolTipService.SetToolTip(panel, toolTip);

			return panel;
		}

		#region IsReady

		public override bool IsReady
		{
			get { return ForestConverter != null; }
		}

		private void OnIsReadyChanged()
		{
			if (IsReady)
				RaiseChanged();
		}

		#endregion // end of IsReady property
	}
}