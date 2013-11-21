using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.ViewportRestrictions
{
	/// <summary>
	/// Represents a base class for all restrictions that are being applied to viewport's visible rect.
	/// </summary>
	public abstract class ViewportRestriction
	{
		#region IViewportRestriction Members

		/// <summary>
		/// Applies the restriction.
		/// </summary>
		/// <param name="previousDataRect">Previous data rectangle.</param>
		/// <param name="proposedDataRect">Proposed data rectangle.</param>
		/// <param name="viewport">The viewport, to which current restriction is being applied.</param>
		/// <returns>New changed visible rectangle.</returns>
		public abstract DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport);

		/// <summary>
		/// Raises the changed event.
		/// </summary>
		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}
		/// <summary>
		/// Occurs when restriction changes.
		/// Causes update of <see cref="Viewport"/>'s Visible property.
		/// </summary>
		public event EventHandler Changed;

		#endregion
	}
}
