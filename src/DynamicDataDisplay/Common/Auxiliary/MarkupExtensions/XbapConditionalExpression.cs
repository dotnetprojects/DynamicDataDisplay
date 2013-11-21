using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.MarkupExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class XbapConditionalExpression : MarkupExtension
	{
		public XbapConditionalExpression() { }

		public XbapConditionalExpression(object value)
		{
			this.Value = value;
		}

		[ConstructorArgument("value")]
		public object Value { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
#if RELEASEXBAP
			return null;
#else
			return ((ResourceDictionary)Application.LoadComponent(new Uri("/DynamicDataDisplay;component/Themes/Generic.xaml", UriKind.Relative)))[Value];
#endif
		}
	}
}
