using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class LiveToolTipAdorner : Adorner
	{
		private Canvas canvas = new Canvas { IsHitTestVisible = false };
		private readonly VisualCollection visualChildren;
		public LiveToolTipAdorner(UIElement adornedElement, LiveToolTip tooltip)
			: base(adornedElement)
		{
			visualChildren = new VisualCollection(this);

			adornedElement.MouseLeave += adornedElement_MouseLeave;
			adornedElement.MouseEnter += adornedElement_MouseEnter;
			adornedElement.PreviewMouseMove += adornedElement_MouseMove;
			//FrameworkElement frAdornedElement = (FrameworkElement)adornedElement;
			//frAdornedElement.SizeChanged += frAdornedElement_SizeChanged;

			this.liveTooltip = tooltip;

			tooltip.Visibility = Visibility.Hidden;

			canvas.Children.Add(liveTooltip);
			AddLogicalChild(canvas);
			visualChildren.Add(canvas);

			Unloaded += LiveTooltipAdorner_Unloaded;
		}

		//void frAdornedElement_SizeChanged(object sender, SizeChangedEventArgs e)
		//{
		//    grid.Width = e.NewSize.Width;
		//    grid.Height = e.NewSize.Height;

		//    InvalidateMeasure();
		//}

		void LiveTooltipAdorner_Unloaded(object sender, RoutedEventArgs e)
		{
			canvas.Children.Remove(liveTooltip);
		}

		void adornedElement_MouseLeave(object sender, MouseEventArgs e)
		{
			liveTooltip.Visibility = Visibility.Hidden;
		}

		void adornedElement_MouseEnter(object sender, MouseEventArgs e)
		{
			liveTooltip.Visibility = Visibility.Visible;
		}

		Point mousePosition;
		private void adornedElement_MouseMove(object sender, MouseEventArgs e)
		{
			liveTooltip.Visibility = Visibility.Visible;
			mousePosition = e.GetPosition(AdornedElement);
			InvalidateMeasure();
		}

		private void ArrangeTooltip()
		{
			Size tooltipSize = liveTooltip.DesiredSize;

			Point location = mousePosition;
			location.Offset(-tooltipSize.Width / 2, -tooltipSize.Height - 1);

			liveTooltip.Arrange(new Rect(location, tooltipSize));
		}

		LiveToolTip liveTooltip;
		public LiveToolTip LiveTooltip
		{
			get { return liveTooltip; }
		}

		#region Overrides

		protected override Visual GetVisualChild(int index)
		{
			return visualChildren[index];
		}

		protected override int VisualChildrenCount
		{
			get { return visualChildren.Count; }
		}

		protected override Size MeasureOverride(Size constraint)
		{
			foreach (UIElement item in visualChildren)
			{
				item.Measure(constraint);
			}

			liveTooltip.Measure(constraint);

			return base.MeasureOverride(constraint);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (UIElement item in visualChildren)
			{
				item.Arrange(new Rect(item.DesiredSize));
			}

			ArrangeTooltip();

			return finalSize;
		}

		#endregion // end of overrides
	}
}
