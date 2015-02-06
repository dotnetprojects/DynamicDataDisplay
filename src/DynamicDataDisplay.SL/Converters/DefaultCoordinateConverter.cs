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
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay
{
    /// <summary>
    /// This is a default coordinate converter, it applies no additional transformations to points.
    /// </summary>
    public sealed class DefaultCoordinateConverter : ICoordinateConverter
    {
        private Rect screenRect;
        public Rect ScreenRect
        {
            get { return screenRect; }
        }

        private Rect dataRect;
        public Rect DataRect
        {
            get { return dataRect; }
        }

        bool inited = false;
        void ICoordinateConverter.Init(Rect screenRect, Rect dataRect)
        {
            inited = true;

            this.screenRect = screenRect;
            this.dataRect = dataRect;

            rxToScreen = screenRect.Width / dataRect.Width;
            ryToScreen = screenRect.Height / dataRect.Height;
            cxToScreen = dataRect.Left * rxToScreen - screenRect.Left;
            cyToScreen = screenRect.Height + screenRect.Top + dataRect.Top * ryToScreen;

            rxToData = dataRect.Width / screenRect.Width;
            ryToData = dataRect.Height / screenRect.Height;
            cxToData = screenRect.Left * rxToData - dataRect.Left;
            cyToData = dataRect.Height + dataRect.Top + screenRect.Top * ryToData;
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

        #region ICoordinateConverter Members

        public Point ToScreen(Point dataPoint)
        {
            DebugVerify.Is(inited);

            return new Point(dataPoint.X * rxToScreen - cxToScreen, cyToScreen - dataPoint.Y * ryToScreen);
        }

        public Point ToData(Point screenPoint)
        {
            DebugVerify.Is(inited);

            return new Point(screenPoint.X * rxToData - cxToData, cyToData - screenPoint.Y * ryToData);
        }

        #endregion
    }
}
