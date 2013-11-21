using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls;
using DynamicDataDisplay.Markers;

namespace Microsoft.Research.DynamicDataDisplay.Design
{
	public class PieChartDesignModeValueProvider : DesignModeValueProvider
	{
		public PieChartDesignModeValueProvider()
		{
			Properties.Add(PieChart.ItemsSourceProperty);
			Properties.Add(PieChart.IndependentValuePathProperty);
		}

		public override object TranslatePropertyValue(PropertyIdentifier identifier, object value)
		{
			if (identifier.DependencyProperty == PieChart.ItemsSourceProperty)
			{
				List<PieChartDesignModeData> data = new List<PieChartDesignModeData>();
				data.Add(new PieChartDesignModeData { Value = 1 });
				data.Add(new PieChartDesignModeData { Value = 2 });
				data.Add(new PieChartDesignModeData { Value = 5 });

				return data;
			}
			else if (identifier.DependencyProperty == PieChart.IndependentValuePathProperty)
			{
				return "Value";
			}

			return base.TranslatePropertyValue(identifier, value);
		}
	}

	internal class PieChartDesignModeData
	{
		public double Value { get; set; }
	}
}
