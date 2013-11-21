using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Target of rendering
	/// </summary>
	public enum RenderTo
	{
		/// <summary>
		/// Rendering directly to screen
		/// </summary>
		Screen,
		/// <summary>
		/// Rendering to bitmap, which will be drawn to screen later.
		/// </summary>
		Image
	}

	public sealed class RenderState
	{
		private readonly DataRect visible;
		private readonly Rect output;
		private readonly DataRect renderVisible;
		private readonly RenderTo renderingType;

		public DataRect RenderVisible
		{
			get { return renderVisible; }
		}

		public RenderTo RenderingType
		{
			get { return renderingType; }
		}

		public Rect Output
		{
			get { return output; }
		}

		public DataRect Visible
		{
			get { return visible; }
		}

		internal RenderState(DataRect renderVisible, DataRect visible, Rect output, RenderTo renderingType)
		{
			this.renderVisible = renderVisible;
			this.visible = visible;
			this.output = output;
			this.renderingType = renderingType;
		}
	}
}
