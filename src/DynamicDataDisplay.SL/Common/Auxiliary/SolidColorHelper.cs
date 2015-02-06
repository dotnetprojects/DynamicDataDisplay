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
    public class SolidColorHelper
    {
        private static Color lastColor = new Color() { A=0};
        private static Random random = new Random();

        private static double GetDiff(Color a, Color b) {
            return (Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B-b.B));
        }

        private static byte GetRandomComponent() {
            return (byte)(random.Next(256));
        }

        private static Color GetRandomColor() {
            Color c = new Color();
            c.A = 0xFF;
            c.B = GetRandomComponent();
            c.G = GetRandomComponent();
            c.R = GetRandomComponent();
            return c;
        }

        public static Color Next() {
            if (lastColor.A==0)
            {
                lastColor = GetRandomColor();
                return lastColor;
            }
            else
            {
                Color newColor = Colors.Black;
                bool first = true;
                do
                {
                    if (first)
                        first = false;
                    else lastColor = newColor;
                    newColor = GetRandomColor();
                } while (GetDiff(lastColor, newColor) < 255);
                lastColor = newColor;
                return newColor;
            }
        }
    }
}
