using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.v02.CurrencyExchange
{
	/// <summary>
	/// This is a auxiliary class to hold currencies exchange rate for one date
	/// </summary>
	public struct CurrencyInfo
	{
		/// <summary>
		/// Gets or sets the date for which currency exchange rate is given.
		/// </summary>
		/// <value>The date.</value>
		public DateTime Date { get; set; }
		/// <summary>
		/// Gets or sets the currency exchange rate.
		/// </summary>
		/// <value>The rate.</value>
		public double Rate { get; set; }
	}
}
