using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
    public class buttonsNavigation : ContentControl
    {
       private Canvas drawingCanvas = new Canvas();
       private Rectangle plus = new Rectangle();
       private Rectangle minus = new Rectangle();

       private Color borderColor = Colors.Gray;

       private List<Line> lines = new List<Line>();

       private Plotter associatedPlotter;

       public Plotter AssciatedPlotter {
           get {
               return associatedPlotter;
           }
       }

        public buttonsNavigation(Plotter associatingPlotter) {
            associatedPlotter = associatingPlotter;
            Initiale();
        }

        private void Initiale() {
            plus.RadiusX = 3;
            plus.RadiusY = 3;
            plus.Width = 30;
            plus.Height = 30;
            Color FillColor = new Color();
            FillColor.A = 20;
            FillColor.G = 0;
            FillColor.B = 0;
            FillColor.R = 0;

            minus.RadiusX = 3;
            minus.RadiusY = 3;
            minus.Width = 30;
            minus.Height = 30;

            plus.Fill = new SolidColorBrush(FillColor);
            plus.Stroke = new SolidColorBrush(borderColor);
            minus.Fill = new SolidColorBrush(FillColor);
            minus.Stroke = new SolidColorBrush(borderColor);

            Content = drawingCanvas;

            Loaded += new RoutedEventHandler(buttonsNavigation_Loaded);
        }

        void buttonsNavigation_Loaded(object sender, RoutedEventArgs e)
        {
            Width = 50;
            Height = 90;

            drawingCanvas.Width = 50;
            drawingCanvas.Height = 90;

            minus.SetValue(Canvas.LeftProperty, 10.0);
            minus.SetValue(Canvas.TopProperty, 10.0);
            Canvas.SetZIndex(minus, Int16.MaxValue - 1);

            plus.SetValue(Canvas.LeftProperty, 10.0);
            plus.SetValue(Canvas.TopProperty, 50.0);
            Canvas.SetZIndex(plus, Int16.MaxValue - 1);

            Line p1 = new Line();
            p1.X1 = 35;
            p1.X2 = 15;
            p1.Y1 = 25.0;
            p1.Y2 = 25.0;
            Line p2 = new Line();
            p2.X1 = 35;
            p2.X2 = 15;
            p2.Y1 = 65.0;
            p2.Y2 = 65.0;
            Line p3 = new Line();
            p3.X1 = 25;
            p3.X2 = 25;
            p3.Y1 = 15.0;
            p3.Y2 = 35.0;

            lines.Add(p1);
            lines.Add(p2);
            lines.Add(p3);
            foreach (Line l in lines)
            {
                l.Stroke = new SolidColorBrush(borderColor);
                Canvas.SetZIndex(l, Int16.MaxValue - 2);
                drawingCanvas.Children.Add(l);
            }

            drawingCanvas.Children.Add(plus);
            drawingCanvas.Children.Add(minus);

            plus.MouseLeftButtonDown += new MouseButtonEventHandler(plus_MouseLeftButtonDown);
            minus.MouseLeftButtonDown += new MouseButtonEventHandler(minus_MouseLeftButtonDown);
        }

        private static double zoomSpeed = 1.2;


        void minus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Plotter2D plotter2D = associatedPlotter as Plotter2D;
            Point zoomTo = new Point(plotter2D.Viewport.Visible.Width / 2 + plotter2D.Viewport.Visible.X, plotter2D.Viewport.Visible.Height / 2 + plotter2D.Viewport.Visible.Y);
            plotter2D.Viewport.Visible = plotter2D.Viewport.Visible.Zoom(zoomTo, 1 / zoomSpeed);
            e.Handled = true;
        }

        void plus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Plotter2D plotter2D = associatedPlotter as Plotter2D;
            
            Point zoomTo = new Point(plotter2D.Viewport.Visible.Width / 2 + plotter2D.Viewport.Visible.X, plotter2D.Viewport.Visible.Height / 2 + plotter2D.Viewport.Visible.Y);
            plotter2D.Viewport.Visible = plotter2D.Viewport.Visible.Zoom(zoomTo, zoomSpeed);
            e.Handled = true;
        }
    }
}
