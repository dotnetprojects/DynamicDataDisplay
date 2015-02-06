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
    /// <summary>
    /// Base class for DateTime axis.
    /// </summary>
    public class DateTimeAxis : AxisBase<DateTime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeAxis"/> class.
        /// </summary>
        public DateTimeAxis()
            : base(new DateTimeAxisControl(), DoubleToDate,
                dt => dt.Ticks / 10000000000.0)
        { }

        private static readonly long minTicks = DateTime.MinValue.Ticks;
        private static readonly long maxTicks = DateTime.MaxValue.Ticks;
        private static DateTime DoubleToDate(double d)
        {
            long ticks = (long)(d * 10000000000L);

            // todo should we throw an exception if number of ticks is too big or small?
            if (ticks < minTicks)
                ticks = minTicks;
            else if (ticks > maxTicks)
                ticks = maxTicks;

            return new DateTime(ticks);
        }

        /// <summary>
        /// Sets the conversion of ticks.
        /// </summary>
        /// <param name="min">The minimal double value.</param>
        /// <param name="minDate">The minimal date, corresponding to minimal double value.</param>
        /// <param name="max">The maximal double value.</param>
        /// <param name="maxDate">The maximal date, correspondong to maximal double value.</param>
        public void SetConversion(double min, DateTime minDate, double max, DateTime maxDate)
        {
            var conversion = new DateTimeToDoubleConversion(min, minDate, max, maxDate);

            ConvertToDouble = conversion.ToDouble;
            ConvertFromDouble = conversion.FromDouble;
        }
    }
}
