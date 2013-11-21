using System;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay
{
	public class ResolveLegendItemEventArgs : EventArgs
	{
		public ResolveLegendItemEventArgs(LegendItem legendItem)
		{
			LegendItem = legendItem;
		}

		public LegendItem LegendItem { get; set; }
	}

	public abstract class Description : FrameworkElement
	{

		private LegendItem legendItem;
		public LegendItem LegendItem
		{
			get
			{
				if (legendItem == null)
				{
					legendItem = CreateLegendItem();
				}
				return legendItem;
			}
		}

		private LegendItem CreateLegendItem()
		{
			LegendItem item = CreateLegendItemCore();
			return RaiseResolveLegendItem(item);
		}

		protected virtual LegendItem CreateLegendItemCore()
		{
			return null;
		}

		public event EventHandler<ResolveLegendItemEventArgs> ResolveLegendItem;
		private LegendItem RaiseResolveLegendItem(LegendItem uncustomizedLegendItem)
		{
			if (ResolveLegendItem != null)
			{
				ResolveLegendItemEventArgs e = new ResolveLegendItemEventArgs(uncustomizedLegendItem);
				ResolveLegendItem(this, e);
				return e.LegendItem;
			}
			else
			{
				return uncustomizedLegendItem;
			}
		}

		private UIElement viewportElement;
		public UIElement ViewportElement
		{
			get { return viewportElement; }
		}

		internal void Attach(UIElement element)
		{
			this.viewportElement = element;
			AttachCore(element);
		}

		protected virtual void AttachCore(UIElement element) { }

		internal void Detach()
		{
			viewportElement = null;
		}

		public abstract string Brief { get; }

		public abstract string Full { get; }

		public override string ToString()
		{
			return Brief;
		}
	}
}
