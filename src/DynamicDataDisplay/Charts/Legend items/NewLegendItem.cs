using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class NewLegendItem : Control
	{
		static NewLegendItem()
		{
			var thisType = typeof(NewLegendItem);
			DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
		}

		//public object VisualContent
		//{
		//    get { return NewLegend.GetVisualContent(this); }
		//    set { NewLegend.SetVisualContent(this, value); }
		//}

		//[Bindable(true)]
		//public object Description
		//{
		//    get { return NewLegend.GetDescription(this); }
		//    set { NewLegend.SetDescription(this, value); }
		//}
	}
}
