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

namespace CurrencyExchange
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
