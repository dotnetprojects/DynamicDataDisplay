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

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    internal sealed class DateTimeToDoubleConversion
    {
        public DateTimeToDoubleConversion(double min, DateTime minDate, double max, DateTime maxDate)
        {
            this.min = min;
            this.length = max - min;
            this.ticksMin = minDate.Ticks;
            this.ticksLength = maxDate.Ticks - ticksMin;
        }

        private double min;
        private double length;
        private long ticksMin;
        private long ticksLength;

        internal DateTime FromDouble(double d)
        {
            double ratio = (d - min) / length;
            long tick = (long)(ticksMin + ticksLength * ratio);

            tick = MathHelper.Clamp(tick, DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks);

            return new DateTime(tick);
        }

        internal double ToDouble(DateTime dt)
        {
            double ratio = (dt.Ticks - ticksMin) / (double)ticksLength;
            return min + ratio * length;
        }
    }
}
