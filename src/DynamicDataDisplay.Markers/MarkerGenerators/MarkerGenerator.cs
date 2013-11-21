using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace DynamicDataDisplay.Markers
{
	public abstract class MarkerGenerator : FrameworkElement, IDisposable
	{
		static MarkerGenerator()
		{
			Type thisType = typeof(MarkerGenerator);
			LiveToolTipService.IsPropertyProxyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(true));
		}

		public virtual bool IsReady { get { return true; } }

		public virtual FrameworkElement CreateMarker(object dataItem)
		{
			FrameworkElement marker = null;
			if (pool.Count > 0)
			{
				marker = pool.Get();
			}
			else
			{
				marker = CreateMarkerCore(dataItem);
			}

			// todo properly use binding to set a DataContext.
			marker.DataContext = dataItem;

			return marker;
		}

		protected virtual FrameworkElement CreateMarkerCore(object dataItem) { throw new NotImplementedException(); }

		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}
		public event EventHandler Changed;

		private readonly ResourcePool<FrameworkElement> pool = new ResourcePool<FrameworkElement>();
		public virtual void Release(FrameworkElement marker)
		{
			pool.Put(marker);
		}

		#region IDisposable Members

		public void Dispose()
		{
			pool.Clear();
		}

		#endregion
	}
}
