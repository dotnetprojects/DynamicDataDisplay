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
using System.Windows.Browser;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
    /// <summary>Provides common methods of mouse navigation around viewport</summary>
    public class MouseNavigation : NavigationBase
    {
        private Rectangle zoomingRect = new Rectangle();
        private bool isPanning = false;
        private bool isZoomRectCreting = false;
        private bool isCtrlPressed = false;
        private Point zoomingRectStartPointInScreen;
        private Point panningStartPointInViewport;
        private Point panningEndPointInViewport;
        private DateTime lastClick = DateTime.Now;
        private const double wheelZoomSpeed = 1.2;

        public MouseNavigation() : base()
        {
            Loaded += new RoutedEventHandler(MouseNavigation_Loaded);
        }

        private Button buttonPlus = new Button();

        void MouseNavigation_Loaded(object sender, RoutedEventArgs e)
        {
            //HtmlPage.Window.AttachEvent("DOMMouseScroll", OnMouseWheel);
            //HtmlPage.Window.AttachEvent("onmousewheel", OnMouseWheel);
            //HtmlPage.Document.AttachEvent("onmousewheel", OnMouseWheel);
        }

        //private void OnMouseWheel(object sender, HtmlEventArgs args) {
        //    if (mouseInViewport)
        //    {
        //        double mouseDelta = 0;
        //        ScriptObject e = args.EventObject;
        //        if (e.GetProperty("detail") != null) // Mozilla & safari
        //        {
        //            mouseDelta = ((double)e.GetProperty("detail"));
        //        }
        //        else if (e.GetProperty("wheelDelta") != null) //IE & Opera
        //        {
        //            mouseDelta=((double)e.GetProperty("wheelDelta"));
        //        }
        //        //MessageBox.Show("mouse delta "+mouseDelta);


        //        Point zoomTo = new Point(Viewport.Visible.Width/2+Viewport.Visible.X, Viewport.Visible.Height / 2+Viewport.Visible.Y);

        //        double zoomSpeed = wheelZoomSpeed;

        //        if (mouseDelta < 0)
        //        {
        //            zoomSpeed = 1 / zoomSpeed;
        //        }
        //        Viewport.Visible = Viewport.Visible.Zoom(zoomTo, zoomSpeed);
        //    }
        //}

        private void relocateVisisble() {
            Rect vis = Viewport.Visible;
            double shiftX = (panningStartPointInViewport.X - panningEndPointInViewport.X);
            double shiftY = (panningStartPointInViewport.Y - panningEndPointInViewport.Y);
            vis.X += shiftX;
            vis.Y += shiftY;
            Viewport.Visible = vis;
            //panningStartPointInViewport = panningEndPointInViewport;
            //debugTextblock.Text = PerformanceCounter.GetString();
            //MessageBox.Show("Shifting visible to "+shiftX+" "+shiftY+" from "+panningStartPointInViewport+" to "+panningEndPointInViewport);
        }

        

        public override void OnPlotterAttached(Plotter plotter)
        {
            base.OnPlotterAttached(plotter);

            plotter.CentralGrid.MouseLeftButtonDown += new MouseButtonEventHandler(CentralGrid_MouseLeftButtonDown);
            plotter.CentralGrid.MouseLeftButtonUp += new MouseButtonEventHandler(CentralGrid_MouseLeftButtonUp);
            plotter.CentralGrid.MouseMove += new MouseEventHandler(CentralGrid_MouseMove);
            //plotter.CentralGrid.MouseLeave += new MouseEventHandler(CentralGrid_MouseLeave);
            //plotter.CentralGrid.MouseEnter += new MouseEventHandler(CentralGrid_MouseEnter);
            plotter.KeyDown += new KeyEventHandler(plotter_KeyDown);
            plotter.KeyUp += new KeyEventHandler(plotter_KeyUp);

            zoomingRect.Stroke = new SolidColorBrush(Colors.LightGray);
            Color fillColor= new Color();
            fillColor.A=40;
            fillColor.R=0x80;
            fillColor.G = 0x80;
            fillColor.B = 0x80;
            zoomingRect.RadiusX = 2;
            zoomingRect.RadiusY = 2;

            zoomingRect.Fill = new SolidColorBrush(fillColor);
        }

        void plotter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Ctrl)
            {
                isCtrlPressed = false;
                isZoomRectCreting = false;
                if (Plotter.MainCanvas.Children.Contains(zoomingRect)) Plotter.MainCanvas.Children.Remove(zoomingRect);
            }
        }

        void plotter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Ctrl) {
                
                isCtrlPressed = true;
                }
        }

       
        void CentralGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isPanning && !isZoomRectCreting) return;
            else if (isZoomRectCreting)
            {
                Point currentMousePosition = e.GetPosition(Plotter.CentralGrid);
                if (currentMousePosition.X > zoomingRectStartPointInScreen.X)
                {
                    zoomingRect.Width = currentMousePosition.X - zoomingRectStartPointInScreen.X;
                }
                else
                {
                    Canvas.SetLeft(zoomingRect, currentMousePosition.X);
                    zoomingRect.Width = zoomingRectStartPointInScreen.X - currentMousePosition.X;
                }
                if (currentMousePosition.Y > zoomingRectStartPointInScreen.Y)
                {
                    zoomingRect.Height = currentMousePosition.Y - zoomingRectStartPointInScreen.Y;
                }
                else
                {
                    Canvas.SetTop(zoomingRect, currentMousePosition.Y);
                    zoomingRect.Height = zoomingRectStartPointInScreen.Y - currentMousePosition.Y;
                }
            }
            else
            {
                panningEndPointInViewport = e.GetPosition(Plotter.CentralGrid).ScreenToViewport(Viewport.Transform);
                relocateVisisble();
            }
            //Point p = e.GetPosition(Plotter.CentralGrid);
            //Point p2 = p.ScreenToViewport(Viewport.Transform);
            //if(debugTextblock!=null) debugTextblock.Text = "screen x:" + p.X + " y:" + p.Y+"; vis x: "+p2.X+" y:"+p2.Y;
        }

        public override void OnPlotterDetaching(Plotter plotter)
        {
            plotter.CentralGrid.MouseLeftButtonDown -= CentralGrid_MouseLeftButtonDown;
            plotter.CentralGrid.MouseLeftButtonUp -= CentralGrid_MouseLeftButtonUp;
            plotter.CentralGrid.MouseMove -= CentralGrid_MouseMove;
            //plotter.CentralGrid.MouseLeave -= CentralGrid_MouseLeave;

            base.OnPlotterDetaching(plotter);
        }

        void CentralGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                //Point panningEndPointInScreen;
                //panningEndPointInScreen = e.GetPosition(Plotter.CentralGrid);
                //panningEndPointInViewport = panningEndPointInScreen.ScreenToViewport(Viewport.Transform);
                //relocateVisisble();
                isPanning = false;
            }
        }

        void CentralGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isCtrlPressed) {
                Plotter.MainCanvas.Children.Remove(zoomingRect);
                Point currPosution = e.GetPosition(Plotter.CentralGrid);
                Point p1 = zoomingRectStartPointInScreen.ScreenToViewport(Viewport.Transform);
                Point p2 = new Point();
                if (currPosution.X < zoomingRectStartPointInScreen.X)
                    p2.X = zoomingRectStartPointInScreen.X - zoomingRect.Width;
                else
                    p2.X = currPosution.X;
                if (currPosution.Y < zoomingRectStartPointInScreen.Y)
                    p2.Y = zoomingRectStartPointInScreen.Y - zoomingRect.Height;
                else
                    p2.Y = currPosution.Y;
                p2 = p2.ScreenToViewport(Viewport.Transform);
                Rect newVisible = new Rect(p1, p2);
                Viewport.Visible = newVisible;
            }
            else
            if (isPanning)
            {
                //Point panningEndPointInScreen;
                //panningEndPointInScreen = e.GetPosition(Plotter.CentralGrid);
                //panningEndPointInViewport = panningEndPointInScreen.ScreenToViewport(Viewport.Transform);
                //relocateVisisble();
                isPanning = false;
            }
        }

        void CentralGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isCtrlPressed)
            {
                zoomingRectStartPointInScreen=e.GetPosition(Plotter.CentralGrid);
                if(!Plotter.MainCanvas.Children.Contains(zoomingRect)) Plotter.MainCanvas.Children.Add(zoomingRect);
                Canvas.SetZIndex(zoomingRect,Int16.MaxValue-3);
                Canvas.SetLeft(zoomingRect,zoomingRectStartPointInScreen.X);
                Canvas.SetTop(zoomingRect, zoomingRectStartPointInScreen.Y);
                zoomingRect.Width = 0;
                zoomingRect.Height = 0;
                isZoomRectCreting = true;
            }
            else
            {
                Point panningStartPointInScreen;
                if ((DateTime.Now - lastClick).Milliseconds < 200) {
                    Viewport.FitToView();
                    return;
                }
                isPanning = true;
                panningStartPointInScreen = e.GetPosition(Plotter.CentralGrid);
                panningStartPointInViewport = panningStartPointInScreen.ScreenToViewport(Viewport.Transform);
                Viewport.AutoFitToView = false;
                
                lastClick = DateTime.Now;
            }
            //MessageBox.Show("vis loc "+Viewport.Visible.X+" "+Viewport.Visible.Y+" size "+Viewport.Visible.Width+" "+Viewport.Visible.Height);
            //MessageBox.Show("Coords screen "+panningStartPointInScreen.X+" "+panningStartPointInScreen.Y+" viewport "+panningStartPointInViewport.X+" "+panningStartPointInViewport.Y);
        }

      
    }
}
