using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.UndoSystem;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class Viewport2DExtensions
	{
		public static void Zoom(this Viewport2D viewport, double factor)
		{
			DataRect visible = viewport.Visible;
			DataRect oldVisible = visible;
			Point center = visible.GetCenter();
			Vector halfSize = new Vector(visible.Width * factor / 2, visible.Height * factor / 2);
			viewport.Visible = new DataRect(center - halfSize, center + halfSize);

			viewport.Plotter.UndoProvider.AddAction(new DependencyPropertyChangedUndoAction(viewport, Viewport2D.VisibleProperty, oldVisible, visible));
		}
	}
}
