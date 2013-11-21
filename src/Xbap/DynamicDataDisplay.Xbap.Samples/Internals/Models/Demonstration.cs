using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals.Models
{
	public class Demonstration : DependencyObject
	{
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}

		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
		  "Description",
		  typeof(string),
		  typeof(Demonstration),
		  new FrameworkPropertyMetadata(""));

		public string Uri { get; set; }

		public FlowDocument AboutDocument
		{
			get { return (FlowDocument)GetValue(AboutDocumentProperty); }
			set { SetValue(AboutDocumentProperty, value); }
		}

		public static readonly DependencyProperty AboutDocumentProperty = DependencyProperty.Register(
		  "AboutDocument",
		  typeof(FlowDocument),
		  typeof(Demonstration),
		  new FrameworkPropertyMetadata(null));

	}
}
