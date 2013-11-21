using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public enum DifferenceIn
	{
		Biggest = Year,

		Year = 6,
		Month = 5,
		Day = 4,
		Hour = 3,
		Minute = 2,
		Second = 1,
		Millisecond = 0,

		Smallest = Millisecond
	}
}
