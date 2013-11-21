using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.ViewportRestrictions
{
	public sealed class ProportionsRestriction : ViewportRestriction
	{
		private double widthToHeightRatio = 1;
		public double WidthToHeightRatio
		{
			get { return widthToHeightRatio; }
			set
			{
				if (widthToHeightRatio != value)
				{
					widthToHeightRatio = value;
					RaiseChanged();
				}
			}
		}

		public override DataRect Apply(DataRect oldDataRect, DataRect newDataRect, Viewport2D viewport)
		{
			double ratio = newDataRect.Width / newDataRect.Height;
			double coeff = Math.Sqrt(ratio);

			double newWidth = newDataRect.Width / coeff;
			double newHeight = newDataRect.Height * coeff;

			Point center = newDataRect.GetCenter();
			DataRect res = DataRect.FromCenterSize(center, newWidth, newHeight);
			return res;
		}
	}
}
