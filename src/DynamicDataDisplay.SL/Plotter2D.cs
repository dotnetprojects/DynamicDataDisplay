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
    /// <summary>Control for plotting 2d images</summary>
    public class Plotter2D : Plotter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plotter2D"/> class.
        /// </summary>
        public Plotter2D()
        {
            Children.Add(viewport);
        }

        private readonly Viewport2D viewport = new Viewport2D();

        /// <summary>
        /// Gets the viewport.
        /// </summary>
        /// <value>The viewport.</value>
        public Viewport2D Viewport
        {
            get { return viewport; }
        }

        public DataTransform DataTransform
        {
            get { return viewport.Transform.DataTransform; }
            set { viewport.Transform = viewport.Transform.WithDataTransform(value); }
        }

        public void FitToView()
        {
            viewport.FitToView();
        }
    }
}
