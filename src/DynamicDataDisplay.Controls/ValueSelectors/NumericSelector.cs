using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	/// <summary>
	/// Represents a control for precise selecting <see cref="T:System.Int32"/> values.
	/// </summary>
	public class NumericSelector : GenericValueSelector<double>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NumericSelector"/> class.
		/// </summary>
		public NumericSelector()
		{
			var axis = new HorizontalAxis();
			Children.Add(axis);
			ValueConversion = axis;
		}
	}
}
