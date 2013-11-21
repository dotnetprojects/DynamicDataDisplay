using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.ViewportRestrictions
{
	public sealed class DateTimeHorizontalAxisRestriction : ViewportRestriction
	{
		private readonly double minSeconds = new TimeSpan(DateTime.MinValue.Ticks).TotalSeconds;
		private readonly double maxSeconds = new TimeSpan(DateTime.MaxValue.Ticks).TotalSeconds;

		public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
		{
			DataRect borderRect = DataRect.Create(minSeconds, proposedDataRect.YMin, maxSeconds, proposedDataRect.YMax);
			if (proposedDataRect.IntersectsWith(borderRect))
			{
				DataRect croppedRect = DataRect.Intersect(proposedDataRect, borderRect);
				return croppedRect;
			}

			return previousDataRect;
		}
	}
}
