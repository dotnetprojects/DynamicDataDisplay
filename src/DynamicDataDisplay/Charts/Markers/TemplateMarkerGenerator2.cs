using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Windows.Markup;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Markers
{
	[ContentProperty("Template")]
	public class TemplateMarkerGenerator2 : OldMarkerGenerator
	{
		private DataTemplate template;
		[NotNull]
		public DataTemplate Template
		{
			get { return template; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (template != value)
				{
					template = value;
					pool.Clear();
					RaiseChanged();
				}
			}
		}

		private readonly ResourcePool<FrameworkElement> pool = new ResourcePool<FrameworkElement>();

		protected override FrameworkElement CreateMarkerCore(object dataItem)
		{
			if (template == null)
				throw new InvalidOperationException(Strings.Exceptions.TemplateShouldNotBeNull);

			FrameworkElement marker = pool.Get();
			if (marker == null)
			{
				marker = (FrameworkElement)template.LoadContent();
			}

			return marker;
		}

		public override void ReleaseMarker(FrameworkElement element)
		{
			pool.Put(element);
		}
	}
}
