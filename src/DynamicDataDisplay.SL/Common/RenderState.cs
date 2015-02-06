using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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
        private readonly Rect visible;

        private readonly Rect output;


        private readonly Rect renderVisible;

        private readonly RenderTo renderingType;

        public Rect RenderVisible
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

        public Rect Visible
        {
            get { return visible; }
        }

        public RenderState(Rect renderVisible, Rect visible, Rect output, RenderTo renderingType)
        {
            this.renderVisible = renderVisible;
            this.visible = visible;
            this.output = output;
            this.renderingType = renderingType;
        }
    }
}
