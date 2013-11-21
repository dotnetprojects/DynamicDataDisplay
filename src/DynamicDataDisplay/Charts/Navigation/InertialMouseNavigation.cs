// todo look to comment for EndlessAnimation

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Research.DynamicDataDisplay.Navigation;
//using System.Windows.Input;
//using System.Windows;
//using System.Windows.Media.Animation;
//using System.Windows.Media;
//using System.Diagnostics;

//namespace Microsoft.Research.DynamicDataDisplay.Charts.Navigation
//{
//    [Obsolete("Unready because Viewport became non-visual and no longer has BeginAnimation method.", true)]
//    public class InertialMouseNavigation : MouseNavigation
//    {
//        public InertialMouseNavigation()
//        {
//        }

//        Point visibleStart;
//        Point panningStartPointInScreen;
//        int panningStartTime;
//        protected override void StartPanning(MouseButtonEventArgs e)
//        {
//            visibleStart = Viewport.Visible.Location;
//            panningStartPointInScreen = e.GetPosition(this);
//            base.StartPanning(e);
//            panningStartTime = e.Timestamp;
//        }

//        bool runningAnimation = false;
//        protected override void StopPanning(MouseButtonEventArgs e)
//        {
//            Point panningStopPointInScreen = e.GetPosition(this);

//            base.StopPanning(e);


//            // mouse click with no panning
//            if (panningStartPointInScreen == panningStopPointInScreen)
//            {
//                // remove all animations on Viewport.Visible
//                BeginVisibleAnimation(null);
//                return;
//            }

//            double panningDuration = (e.Timestamp - panningStartTime) / 1000.0;

//            double animationStartMinDuration = 0.5; // seconds
//            double animationStartMinMouseShift = 50; // pixels

//            // mouse moved short enough and for rather long distance.
//            if (panningDuration < animationStartMinDuration && (panningStopPointInScreen - panningStartPointInScreen).Length > animationStartMinMouseShift)
//            {
//                Vector visibleShift = Viewport.Visible.Location - visibleStart;

//                EndlessPanningRectAnimation panningAnimation = new EndlessPanningRectAnimation(
//                    Viewport.Visible,
//                    visibleShift / panningDuration);

//                BeginVisibleAnimation(panningAnimation);
//            }
//        }

//        private void BeginVisibleAnimation(AnimationTimeline animation)
//        {
//            if (animation != null)
//            {
//                if (runningAnimation)
//                {
//                    BeginVisibleAnimation(null);
//                }

//                //Plotter2D.Viewport.BeginAnimation(Viewport2D.VisibleProperty, animation);
//                runningAnimation = true;
//            }
//            else
//            {
//                if (!runningAnimation)
//                    return;

//                Debug.WriteLine("Removing animation");

//                Rect visible = Plotter2D.Viewport.Visible;
//                //Plotter2D.Viewport.BeginAnimation(Viewport2D.VisibleProperty, null);
//                Plotter2D.Viewport.Visible = visible;
//                runningAnimation = false;
//            }
//        }
//    }
//}
