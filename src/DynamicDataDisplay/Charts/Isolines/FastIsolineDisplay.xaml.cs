using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Isolines
{
	public partial class FastIsolineDisplay : IsolineGraphBase
	{
		public FastIsolineDisplay()
		{
			InitializeComponent();
		}

		protected override Panel HostPanel
		{
			get
			{
				return Plotter2D.CentralGrid;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var isolineRenderer = (FastIsolineRenderer)Template.FindName("PART_IsolineRenderer", this);
			//Binding contentBoundsBinding = new Binding { Path = new PropertyPath("(0)", Viewport2D.ContentBoundsProperty), Source = isolineRenderer };
			//SetBinding(Viewport2D.ContentBoundsProperty, contentBoundsBinding);

			if (isolineRenderer != null)
			{
				isolineRenderer.AddHandler(Viewport2D.ContentBoundsChangedEvent, new RoutedEventHandler(OnRendererContentBoundsChanged));
				UpdateContentBounds(isolineRenderer);
			}
		}

		private void OnRendererContentBoundsChanged(object sender, RoutedEventArgs e)
		{
			UpdateContentBounds((DependencyObject)sender);
		}

		private void UpdateContentBounds(DependencyObject source)
		{
			var contentBounds = Viewport2D.GetContentBounds(source);
			Viewport2D.SetContentBounds(this, contentBounds);
		}
	}
}
