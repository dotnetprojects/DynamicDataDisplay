using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.ViewportRestrictions
{
	/// <summary>
	/// Represents a restriction in which actual visible rect's proportions depends on 
	/// actual output rect's proportions.
	/// </summary>
	public sealed class PhysicalProportionsRestriction : ViewportRestriction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PhysicalProportionsRestriction"/> class.
		/// </summary>
		public PhysicalProportionsRestriction() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="PhysicalProportionsRestriction"/> class with the given proportion ratio.
		/// </summary>
		/// <param name="proportionRatio">The proportion ratio.</param>
		public PhysicalProportionsRestriction(double proportionRatio)
		{
			ProportionRatio = proportionRatio;
		}

		private double proportionRatio = 1.0;
		/// <summary>
		/// Gets or sets the proportion ratio.
		/// Default value is 1.0.
		/// </summary>
		/// <value>The proportion ratio.</value>
		public double ProportionRatio
		{
			get { return proportionRatio; }
			set
			{
				if (proportionRatio != value)
				{
					proportionRatio = value;
					RaiseChanged();
				}
			}
		}

		/// <summary>
		/// Applies the restriction.
		/// </summary>
		/// <param name="previousDataRect">Previous data rectangle.</param>
		/// <param name="proposedDataRect">Proposed data rectangle.</param>
		/// <param name="viewport">The viewport, to which current restriction is being applied.</param>
		/// <returns>New changed visible rectangle.</returns>
		public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
		{
			Rect output = viewport.Output;
			if (output.Width == 0 || output.Height == 0)
				return proposedDataRect;

			double screenRatio = output.Width / output.Height;
			double viewportRatio = proposedDataRect.Width / proposedDataRect.Height;
			double ratio = screenRatio / viewportRatio;
			double width = proportionRatio * proposedDataRect.Width * ratio;
			double height = proposedDataRect.Height;

			if (width < proposedDataRect.Width)
			{
				height = proposedDataRect.Height / proportionRatio / ratio;
				width = proposedDataRect.Width;
			}

			Point center = proposedDataRect.GetCenter();
			Rect res = RectExtensions.FromCenterSize(center, width, height);

			return res;
		}
	}
}
