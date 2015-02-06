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

namespace Microsoft.Research.DynamicDataDisplay
{
    public struct Int32Rect
    {
        private Int32 x, y, width, height;

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        public int Y {
            get {
                return y;
            }
            set {
                y = value;
            }
        }
        public int Width {
            get {
                return width;
            }
            set {
                height = value;
            }
        }
        public int Height {
            get {
                return height;
            }
            set {
                height = value;
            }
        }

        public Int32Rect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}
