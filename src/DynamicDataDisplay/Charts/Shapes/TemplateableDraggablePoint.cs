using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	public class TemplateableDraggablePoint : DraggablePoint
	{
		private readonly Control marker = new Control { Focusable = false };
		public TemplateableDraggablePoint()
		{
			marker.SetBinding(Control.TemplateProperty, new Binding { Source = this, Path = new PropertyPath("MarkerTemplate") });
			Content = marker;	
		}

		public ControlTemplate MarkerTemplate
		{
			get { return (ControlTemplate)GetValue(MarkerTemplateProperty); }
			set { SetValue(MarkerTemplateProperty, value); }
		}

		public static readonly DependencyProperty MarkerTemplateProperty = DependencyProperty.Register(
		  "MarkerTemplate",
		  typeof(ControlTemplate),
		  typeof(TemplateableDraggablePoint),
		  new FrameworkPropertyMetadata(null));

	}
}
