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
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    internal static class RoundHelper
    {
        internal static int GetDifferenceLog(double min, double max)
        {
            return (int)Math.Log(Math.Abs(max - min));
        }

        internal static double Round(double number, int rem)
        {
            if (rem <= 0)
            {
                rem = MathHelper.Clamp(-rem, 0, 15);
                return Math.Round(number, rem);
            }
            else
            {
                double pow = Math.Pow(10, rem - 1);
                double val = pow * Math.Round(number / Math.Pow(10, rem - 1));
                return val;
            }
        }

        internal static RoundingInfo CreateRoundedRange(double min, double max)
        {
            double delta = max - min;

            if (delta == 0)
                return new RoundingInfo { Min = min, Max = max, Log = 0 };

            int log = (int)Math.Round(Math.Log10(Math.Abs(delta))) + 1;

            double newMin = Round(min, log);
            double newMax = Round(max, log);
            if (newMin == newMax)
            {
                log--;
                newMin = Round(min, log);
                newMax = Round(max, log);
            }

            return new RoundingInfo { Min = newMin, Max = newMax, Log = log };
        }
    }

    [DebuggerDisplay("{Min} - {Max}, Log = {Log}")]
    internal sealed class RoundingInfo
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public int Log { get; set; }
    }
}
