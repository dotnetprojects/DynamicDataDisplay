using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.ViewportRestrictions
{
	/// <summary>
	/// Represents a restriction, which limits the maximal size of <see cref="Viewport"/>'s Visible property.
	/// </summary>
	public class MaximalSizeRestriction : ViewportRestriction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MaximalSizeRestriction"/> class.
		/// </summary>
		public MaximalSizeRestriction() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="MaximalSizeRestriction"/> class with the given maximal size of Viewport's Visible.
		/// </summary>
		/// <param name="maxSize">Maximal size of Viewport's Visible.</param>
		public MaximalSizeRestriction(double maxSize)
		{
			MaxSize = maxSize;
		}

		private double maxSize = 1000;
		/// <summary>
		/// Gets or sets the maximal size of Viewport's Visible.
		/// The default value is 1000.0.
		/// </summary>
		/// <value>The size of the max.</value>
		public double MaxSize
		{
			get { return maxSize; }
			set
			{
				if (maxSize != value)
				{
					maxSize = value;
					RaiseChanged();
				}
			}
		}

		/// <summary>
		/// Applies the specified old data rect.
		/// </summary>
		/// <param name="oldDataRect">The old data rect.</param>
		/// <param name="newDataRect">The new data rect.</param>
		/// <param name="viewport">The viewport.</param>
		/// <returns></returns>
		public override DataRect Apply(DataRect oldDataRect, DataRect newDataRect, Viewport2D viewport)
		{
			bool decreasing = newDataRect.Width < oldDataRect.Width || newDataRect.Height < oldDataRect.Height;
			if (!decreasing && (newDataRect.Width > maxSize || newDataRect.Height > maxSize))
				return oldDataRect;

			return newDataRect;
		}
	}
}
