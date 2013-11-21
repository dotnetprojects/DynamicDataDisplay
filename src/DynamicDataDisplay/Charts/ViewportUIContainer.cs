using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	[ContentProperty("Content")]
	public class ViewportUIContainer : DependencyObject, IPlotterElement
	{
		public ViewportUIContainer() { }

		public ViewportUIContainer(FrameworkElement content)
		{
			this.Content = content;
		}

		private FrameworkElement content;
		[NotNull]
		public FrameworkElement Content
		{
			get { return content; }
			set { content = value; }
		}

		#region IPlotterElement Members

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;

			if (Content == null) return;

			var plotterPanel = GetPlotterPanel(Content);
			//Plotter.SetPlotter(Content, plotter);

			if (plotterPanel == PlotterPanel.MainCanvas)
			{
				// if all four Canvas.{Left|Right|Top|Bottom} properties are not set,
				// and as we are adding by default content to MainCanvas, 
				// and since I like more when buttons are by default in right down corner - 
				// set bottom and right to 10.
				var left = Canvas.GetLeft(content);
				var top = Canvas.GetTop(content);
				var bottom = Canvas.GetBottom(content);
				var right = Canvas.GetRight(content);

				if (left.IsNaN() && right.IsNaN() && bottom.IsNaN() && top.IsNaN())
				{
					Canvas.SetBottom(content, 10.0);
					Canvas.SetRight(content, 10.0);
				}
				plotter.MainCanvas.Children.Add(Content);
			}
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			if (Content != null)
			{
				var plotterPanel = GetPlotterPanel(Content);
				//Plotter.SetPlotter(Content, null);
				if (plotterPanel == PlotterPanel.MainCanvas)
				{
					plotter.MainCanvas.Children.Remove(Content);
				}
			}

			this.plotter = null;
		}

		private Plotter plotter;
		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion

		[AttachedPropertyBrowsableForChildren]
		public static PlotterPanel GetPlotterPanel(DependencyObject obj)
		{
			return (PlotterPanel)obj.GetValue(PlotterPanelProperty);
		}

		public static void SetPlotterPanel(DependencyObject obj, PlotterPanel value)
		{
			obj.SetValue(PlotterPanelProperty, value);
		}

		public static readonly DependencyProperty PlotterPanelProperty = DependencyProperty.RegisterAttached(
		  "PlotterPanel",
		  typeof(PlotterPanel),
		  typeof(ViewportUIContainer),
		  new FrameworkPropertyMetadata(PlotterPanel.MainCanvas));
	}
}
