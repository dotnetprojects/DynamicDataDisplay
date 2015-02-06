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
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay
{
    /// <summary>
    /// A central class in 2d coordinate transformation in DynamicDataDisplay.
    /// Provides methods to transform point from one coordinate system to another.
    /// </summary>
    public sealed class CoordinateTransform
    {
        private CoordinateTransform(Rect visibleRect, Rect screenRect)
        {
            this.visibleRect = visibleRect;
            this.screenRect = screenRect;

            rxToScreen = screenRect.Width / visibleRect.Width;
            ryToScreen = screenRect.Height / visibleRect.Height;
            cxToScreen = visibleRect.Left * rxToScreen - screenRect.Left;
            cyToScreen = screenRect.Height + screenRect.Top + visibleRect.Top * ryToScreen;

            rxToData = visibleRect.Width / screenRect.Width;
            ryToData = visibleRect.Height / screenRect.Height;
            cxToData = screenRect.Left * rxToData - visibleRect.Left;
            cyToData = visibleRect.Height + visibleRect.Top + screenRect.Top * ryToData;
        }

        #region Coeffs
        double rxToScreen;
        double ryToScreen;
        double cxToScreen;
        double cyToScreen;

        double rxToData;
        double ryToData;
        double cxToData;
        double cyToData;
        #endregion

        #region Creation methods

        internal CoordinateTransform WithRects(Rect visibleRect, Rect screenRect)
        {
            CoordinateTransform copy = new CoordinateTransform(visibleRect, screenRect);
            copy.dataTransform = dataTransform;
            return copy;
        }

        /// <summary>
        /// Creates a new instance of CoordinateTransform with the given data transform.
        /// </summary>
        /// <param name="dataTransform">The data transform.</param>
        /// <returns></returns>
        public CoordinateTransform WithDataTransform(DataTransform dataTransform)
        {
            if (dataTransform == null)
                throw new ArgumentNullException("dataTransform");

            CoordinateTransform copy = new CoordinateTransform(visibleRect, screenRect);
            copy.dataTransform = dataTransform;
            return copy;
        }

        internal static CoordinateTransform CreateDefault()
        {
            CoordinateTransform transform = new CoordinateTransform(new Rect(0, 0, 1, 1), new Rect(0, 0, 1, 1));

            return transform;
        }

        #endregion

        #region Transform methods

        /// <summary>
        /// Transforms point from data coordinates to screen.
        /// </summary>
        /// <param name="dataPoint">The point in data coordinates.</param>
        /// <returns></returns>
        public Point DataToScreen(Point dataPoint)
        {
            Point viewportPoint = dataTransform.DataToViewport(dataPoint);

            Point screenPoint = new Point(viewportPoint.X * rxToScreen - cxToScreen,
                cyToScreen - viewportPoint.Y * ryToScreen);

            return screenPoint;
        }

        /// <summary>
        /// Transforms point from screen coordinates to data coordinates.
        /// </summary>
        /// <param name="screenPoint">The point in screen coordinates.</param>
        /// <returns></returns>
        public Point ScreenToData(Point screenPoint)
        {
            Point viewportPoint = new Point(screenPoint.X * rxToData - cxToData,
                cyToData - screenPoint.Y * ryToData);

            Point dataPoint = dataTransform.ViewportToData(viewportPoint);

            return dataPoint;
        }

        /// <summary>
        /// Transforms point from viewport coordinates to screen coordinates.
        /// </summary>
        /// <param name="viewportPoint">The point in viewport coordinates.</param>
        /// <returns></returns>
        public Point ViewportToScreen(Point viewportPoint)
        {
            Point screenPoint = new Point(viewportPoint.X * rxToScreen - cxToScreen,
                cyToScreen - viewportPoint.Y * ryToScreen);

            return screenPoint;
        }

        /// <summary>
        /// Transforms point from screen coordinates to viewport coordinates.
        /// </summary>
        /// <param name="screenPoint">The point in screen coordinates.</param>
        /// <returns></returns>
        public Point ScreenToViewport(Point screenPoint)
        {
            Point viewportPoint = new Point(screenPoint.X * rxToData - cxToData,
                cyToData - screenPoint.Y * ryToData);

            return viewportPoint;
        }

        #endregion

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Rect visibleRect;
        /// <summary>
        /// Gets the viewport rectangle.
        /// </summary>
        /// <value>The viewport rect.</value>
        public Rect ViewportRect
        {
            get { return visibleRect; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Rect screenRect;
        /// <summary>
        /// Gets the screen rectangle.
        /// </summary>
        /// <value>The screen rect.</value>
        public Rect ScreenRect
        {
            get { return screenRect; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataTransform dataTransform = DataTransforms.Identity;
        /// <summary>
        /// Gets the data transform.
        /// </summary>
        /// <value>The data transform.</value>
        public DataTransform DataTransform
        {
            get { return dataTransform; }
        }
    }
}
