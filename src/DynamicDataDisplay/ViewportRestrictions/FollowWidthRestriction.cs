using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.ViewportRestrictions
{
	/// <summary>
	/// Represents a viewport restriction which modifies x coordinates of result visible rect to be adjacent to the right border of initial rect and have a fixed given width.
	/// Probably is better to add to FitToViewRestrictions collection of <see cref="Viewport"/>.
	/// </summary>
	public class FollowWidthRestriction : ViewportRestriction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FollowWidthRestriction"/> class.
		/// </summary>
		public FollowWidthRestriction() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="FollowWidthRestriction"/> class with the given width.
		/// </summary>
		/// <param name="width">The width.</param>
		public FollowWidthRestriction(double width)
		{
			Width = width;
		}

		private double width = 1;
		/// <summary>
		/// Gets or sets the width of result visible rectangle.
		/// Default value is 1.0.
		/// </summary>
		/// <value>The width.</value>
		[DefaultValue(1.0)]
		public double Width
		{
			get { return width; }
			set
			{
				if (width != value)
				{
					width = value;
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
			if (proposedDataRect.IsEmpty)
				return proposedDataRect;

			double followWidth = proposedDataRect.Width;
			if (!viewport.UnitedContentBounds.IsEmpty)
			{
				followWidth = Math.Min(width, viewport.UnitedContentBounds.Width);
			}
			if (followWidth.IsInfinite())
				followWidth = width;

			Rect visible = new Rect(proposedDataRect.XMin + proposedDataRect.Width - followWidth, proposedDataRect.YMin, followWidth, proposedDataRect.Height);

			return visible;
		}
	}
}
