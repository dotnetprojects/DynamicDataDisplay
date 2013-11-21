// todo commented because base class is 'Rect'AnimationBase, but animated value is of DataRect type.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Media.Animation;
//using System.Windows;
//using System.Diagnostics;
//using Microsoft.Research.DynamicDataDisplay.Common;

//namespace Microsoft.Research.DynamicDataDisplay.Charts.Navigation
//{
//    internal sealed class EndlessPanningRectAnimation : RectAnimationBase
//    {
//        private DataRect from;
//        public DataRect From
//        {
//            get { return from; }
//            set { from = value; }
//        }

//        private Vector speed;
//        public Vector Speed
//        {
//            get { return speed; }
//            set { speed = value; }
//        }

//        public EndlessPanningRectAnimation(DataRect from, Vector speed)
//        {
//            Duration = Duration.Forever;
//            this.from = from;
//            this.speed = speed;
//        }

//        protected override Rect GetCurrentValueCore(Rect defaultOriginValue, Rect defaultDestinationValue, AnimationClock animationClock)
//        {
//            double time = animationClock.CurrentTime.Value.TotalSeconds;
//            Rect currentValue = Rect.Offset(from, speed * time);
//            return currentValue;
//        }

//        protected override Freezable CreateInstanceCore()
//        {
//            return this;
//        }
//    }
//}
