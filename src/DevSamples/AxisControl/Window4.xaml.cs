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
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;

namespace AxisControlSample
{
	/// <summary>
	/// Interaction logic for Window4.xaml
	/// </summary>
	public partial class Window4 : Window
	{
		const double xSize = 0.01;
		const double ySize = 0.01;
		public Window4()
		{
			InitializeComponent();

			NumericAxis axis = (NumericAxis)plotter.MainHorizontalAxis;
			axis.ConvertToDouble = d => -d;
			axis.ConvertFromDouble = d => -d;

			plotter.AxisGrid.DrawHorizontalMinorTicks = true;
			plotter.AxisGrid.DrawVerticalMinorTicks = true;

			var horizLabelProvider = ((HorizontalAxis)plotter.MainHorizontalAxis).LabelProvider;
			horizLabelProvider.LabelStringFormat = "#{0}";

			var vertLabelProvider = ((VerticalAxis)plotter.MainVerticalAxis).LabelProvider;
			vertLabelProvider.CustomView = (i, ui) =>
			{
				TextBlock block = ui as TextBlock;
				if (block != null)
				{
					block.Foreground = Brushes.MidnightBlue;
					block.FontFamily = new FontFamily("Bradley Hand ITC");
					block.FontSize = 72;
					if (i.Tick % 2 == 0)
					{
						//block.FontStyle = FontStyles.Italic;
					}
				}
			};

			vertLabelProvider.SetCustomFormatter(i =>
			{
				if (i.Tick > 0)
					return "$ " + i.Tick;
				return null;
			});

			dateAxis.AxisControl.MajorLabelProvider.SetCustomFormatter(i =>
			{
				return i.Tick.ToString("MMMM yyyy", new CultureInfo("es-es"));

				return null;
			});

			//plotter.Viewport.Restrictions.Add(new PhysicalProportionsRestriction { ProportionRatio = 1 });

			Random rnd = new Random();
			const int N = 10;
			for (int i = 0; i < N; i++)
			{
				Ellipse ellipse = new Ellipse
				{
					Stroke = Brushes.Black,
					StrokeThickness = 0,
					Fill = ColorHelper.RandomBrush//.MakeTransparent(0.1)
				};

				double x1 = rnd.NextDouble();
				double y1 = rnd.NextDouble();
				double x2 = rnd.NextDouble();
				double y2 = rnd.NextDouble();

				ellipse.ToolTip = x1.ToString("G2") + "," + y1.ToString("G2");

				ellipse.MouseEnter += ellipse_MouseEnter;
				ellipse.MouseLeave += ellipse_MouseEnter;
				ellipse.MouseLeftButtonDown += new MouseButtonEventHandler(ellipse_MouseLeftButtonDown);

				panel.Children.Add(ellipse);
			}
		}

		void ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Ellipse ellipse = (Ellipse)sender;

			if (ellipse.Tag == null)
			{
				//Point p1 = ViewportRectPanel.GetPoint1(ellipse);
				//Point p2 = ViewportRectPanel.GetPoint2(ellipse);
				//Rect bounds = new Rect(p1, p2);
				//Point center = bounds.GetCenter();

				//DraggablePoint pt = new DraggablePoint(center);
				//pt.PositionChanged += pt_PositionChanged;
				//pt.Tag = ellipse;
				//ellipse.Tag = pt;

				//plotter.Children.Add(pt);
			}
			else
			{
				DraggablePoint pt = (DraggablePoint)ellipse.Tag;
				pt.Tag = null;
				ellipse.Tag = null;
				pt.PositionChanged -= pt_PositionChanged;

				plotter.Children.Remove(pt);
			}
		}

		void pt_PositionChanged(object sender, PositionChangedEventArgs e)
		{
			DraggablePoint pt = (DraggablePoint)sender;

			Ellipse ellipse = (Ellipse)pt.Tag;
			Rect bounds = RectExtensions.FromCenterSize(e.Position, xSize, ySize);

			//ViewportRectPanel.SetPoint1(ellipse, bounds.TopLeft);
			//ViewportRectPanel.SetPoint2(ellipse, bounds.BottomRight);
		}

		void ellipse_MouseEnter(object sender, MouseEventArgs e)
		{
			Ellipse ellipse = (Ellipse)sender;
			if (e.RoutedEvent == Ellipse.MouseEnterEvent)
			{
				ChangeFill(ellipse, +1);
			}
			else
			{
				ChangeFill(ellipse, -1);
			}
		}

		private static void ChangeFill(Ellipse ellipse, double sign)
		{
			Color fill = ((SolidColorBrush)ellipse.Fill).Color;
			HsbColor hsb = fill.ToHsbColor();
			hsb.Hue = (hsb.Hue + sign * 70) % 360;
			Color newColor = hsb.ToArgbColor();

			if (newColor.A == 0 || (newColor.R == 0 && newColor.G == 0 && newColor.B == 0))
			{
				Debugger.Break();
			}

			var brush = new SolidColorBrush(newColor);
			ellipse.Fill = brush;
		}
	}
}
