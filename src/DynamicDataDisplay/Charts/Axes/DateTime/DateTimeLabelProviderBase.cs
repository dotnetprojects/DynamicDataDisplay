using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using System.Globalization;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public abstract class DateTimeLabelProviderBase : LabelProviderBase<DateTime>
	{
		private string dateFormat;
		protected string DateFormat
		{
			get { return dateFormat; }
			set { dateFormat = value; }
		}

		protected override string GetStringCore(LabelTickInfo<DateTime> tickInfo)
		{
			return tickInfo.Tick.ToString(dateFormat);
		}

		protected virtual string GetDateFormat(DifferenceIn diff)
		{
			string format = null;

			switch (diff)
			{
				case DifferenceIn.Year:
					format = "yyyy";
					break;
				case DifferenceIn.Month:
					format = "MMM";
					break;
				case DifferenceIn.Day:
					format = "%d";
					break;
				case DifferenceIn.Hour:
					format = "HH:mm";
					break;
				case DifferenceIn.Minute:
					format = "%m";
					break;
				case DifferenceIn.Second:
					format = "ss";
					break;
				case DifferenceIn.Millisecond:
					format = "fff";
					break;
				default:
					break;
			}

			return format;
		}
	}
}
