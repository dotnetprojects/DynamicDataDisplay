using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.ViewportRestrictions
{
	/// <summary>
	/// Represents a restriction which is capable to attach to or detach from <see cref="Viewport"/>, to which it is applied.
	/// </summary>
	public interface ISupportAttachToViewport
	{
		/// <summary>
		/// Attaches the specified viewport to a restriction.
		/// </summary>
		/// <param name="viewport">The viewport.</param>
		void Attach(Viewport2D viewport);

		/// <summary>
		/// Detaches the specified viewport from a restriction.
		/// </summary>
		/// <param name="viewport">The viewport.</param>
		void Detach(Viewport2D viewport);
	}
}
