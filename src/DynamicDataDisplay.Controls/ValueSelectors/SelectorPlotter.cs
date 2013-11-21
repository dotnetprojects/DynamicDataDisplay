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
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	/// <summary>
	/// Interaction logic for SelectionPlotter.xaml
	/// </summary>
	public class SelectorPlotter : ChartPlotter
	{
		static SelectorPlotter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectorPlotter), new FrameworkPropertyMetadata(typeof(SelectorPlotter)));
		}

		private RectangleHighlight leftHighlight;
		protected RectangleHighlight LeftHighlight
		{
			get { return leftHighlight; }
		}

		private RectangleHighlight rightHighlight;
		protected RectangleHighlight RightHighlight
		{
			get { return rightHighlight; }
		}

		private DraggablePoint marker = new TemplateableDraggablePoint();
		public DraggablePoint Marker
		{
			get { return marker; }
		}

		private Brush highlightFill = Brushes.LightGray.MakeTransparent(0.5);

		private SelectorAxisNavigation selectorNavigation = new SelectorAxisNavigation { Placement = AxisPlacement.Bottom };
		protected SelectorAxisNavigation SelectorNavigation
		{
			get { return selectorNavigation; }
		}

		public SelectorPlotter()
		{
			Viewport.Restrictions.Add(new FixedXRestriction());
			Viewport.Domain = DataRect.Create(0, 0, 1, 1);

			Children.Remove(MainHorizontalAxis);
			Children.Remove(MainVerticalAxis);
			Children.Remove(HorizontalAxisNavigation);
			Children.Remove(VerticalAxisNavigation);
			Children.Remove(AxisGrid);
			Children.Remove(DefaultContextMenu);
			Children.Remove(MouseNavigation);
			Children.Remove(Legend);
			Children.Remove(NewLegend);

			leftHighlight = new RectangleHighlight { Fill = highlightFill, Stroke = Brushes.DarkGray, Name = "Left_Highlight" };
			Children.Add(leftHighlight);
			rightHighlight = new RectangleHighlight { Fill = highlightFill, Stroke = Brushes.DarkGray, Name = "Right_Highlight" };
			Children.Add(rightHighlight);

			ResourceDictionary dict = new ResourceDictionary
			{
				Source = new Uri("/DynamicDataDisplay.Controls;component/Themes/Generic.xaml", UriKind.Relative)
			};

			Style plotterStyle = (Style)dict[typeof(SelectorPlotter)];
			Style = plotterStyle;
			Template = (ControlTemplate)(((Setter)plotterStyle.Setters[1]).Value);
			ApplyTemplate();

			Style draggablePointstyle = (Style)dict[typeof(TemplateableDraggablePoint)];
			marker.Style = draggablePointstyle;

			Panel content = (Panel)Content;
			content.MouseLeftButtonDown += CentralGrid_MouseLeftButtonDown;
			content.MouseLeftButtonUp += CentralGrid_MouseLeftButtonUp;

			selectorNavigation.MouseLeftButtonClick += new MouseButtonEventHandler(selectorNavigation_MouseLeftButtonClick);
			Children.Add(selectorNavigation);

			marker.PositionChanged += marker_PositionChanged;
			marker.PositionCoerceCallbacks.Add(CoerceMarkerPosition);
			marker.InvalidateProperty(PositionalViewportUIContainer.PositionProperty);
			Children.Add(marker);

			Viewport.PropertyChanged += Viewport_PropertyChanged;
			UpdateDomain();
		}

		void selectorNavigation_MouseLeftButtonClick(object sender, MouseButtonEventArgs e)
		{
			Point screenPos = e.GetPosition(this);
			Point viewportPos = screenPos.ScreenToViewport(Transform);
			marker.Position = viewportPos;
			e.Handled = true;
		}

		private Point lmbPosition;
		private void CentralGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			lmbPosition = e.GetPosition(this);
		}

		private void CentralGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Point screenPos = e.GetPosition(this);
			if (screenPos == lmbPosition)
			{
				Point viewportPos = screenPos.ScreenToViewport(Transform);
				marker.Position = viewportPos;
				e.Handled = true;
			}
		}

		protected virtual void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				DataRect prevVisible = (DataRect)e.OldValue;
				DataRect currVisible = (DataRect)e.NewValue;

				if (prevVisible.Size != currVisible.Size)
				{
					UpdateDomain();
				}
			}
		}

		private void UpdateDomain()
		{
			double width = Math.Min(Viewport.Visible.Width / 2, domain.Max - domain.Min);
			double min = Domain.Min - width;
			double max = Domain.Max + width;
			leftHighlight.Bounds = DataRect.Create(min, -0.1, Domain.Min, 1.1);
			rightHighlight.Bounds = DataRect.Create(Domain.Max, -0.1, max, 1.1);

			Viewport.Domain = DataRect.Create(min, 0, max, 1);
		}

		private void marker_PositionChanged(object sender, PositionChangedEventArgs e)
		{
			OnMarkerPositionChanged(e);
		}

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			Focus();
		}

		protected virtual void OnMarkerPositionChanged(PositionChangedEventArgs e)
		{
		}

		protected virtual Point CoerceMarkerPosition(PositionalViewportUIContainer marker, Point position)
		{
			position.Y = 1;
			double x = position.X;
			position.X = Math.Min(Domain.Max, Math.Max(x, Domain.Min));
			return position;
		}

		private Range<double> domain = new Range<double>(0, 1);
		protected Range<double> Domain
		{
			get { return domain; }
			set
			{
				domain = value;
				UpdateDomain();
			}
		}
	}
}
