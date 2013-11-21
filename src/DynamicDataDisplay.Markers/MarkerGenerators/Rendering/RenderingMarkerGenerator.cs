using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Markers.MarkerGenerators.Rendering
{
	[ContentProperty("RendererTemplate")]
	public class RenderingMarkerGenerator : MarkerGenerator
	{
		private MarkerRenderer cachedRenderer = null;

		public override FrameworkElement CreateMarker(object dataItem)
		{
			return cachedRenderer;
		}

		private bool isReady = false;
		public override bool IsReady
		{
			get
			{
				return isReady;
			}
		}

		public DataTemplate RendererTemplate
		{
			get { return (DataTemplate)GetValue(RendererTemplateProperty); }
			set { SetValue(RendererTemplateProperty, value); }
		}

		public static readonly DependencyProperty RendererTemplateProperty = DependencyProperty.Register(
		  "RendererTemplate",
		  typeof(DataTemplate),
		  typeof(RenderingMarkerGenerator),
		  new FrameworkPropertyMetadata(null, OnRendererTemplateChanged));

		private static void OnRendererTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RenderingMarkerGenerator owner = (RenderingMarkerGenerator)d;
			owner.OnRendererTemplateChanged();
		}

		private void OnRendererTemplateChanged()
		{
			isReady = true;
			cachedRenderer = (MarkerRenderer)RendererTemplate.LoadContent();
			RaiseChanged();
		}
	}
}
