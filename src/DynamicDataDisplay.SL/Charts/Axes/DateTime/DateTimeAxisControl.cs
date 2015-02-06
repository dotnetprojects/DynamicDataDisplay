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
    /// AxisControl for DateTime axes.
    /// </summary>
    public class DateTimeAxisControl : AxisControl<DateTime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeAxisControl"/> class.
        /// </summary>
        public DateTimeAxisControl()
        {
            LabelProvider = new DateTimeLabelProvider();
            TicksProvider = new DateTimeTicksProvider();
            MayorLabelProvider = new MayorDateTimeLabelProvider();
            //this.DrawMinorTicks = false;
            ConvertToDouble = dt => dt.Ticks;

            Range = new Range<DateTime>(DateTime.Now, DateTime.Now.AddYears(1));
        }
    }
}
