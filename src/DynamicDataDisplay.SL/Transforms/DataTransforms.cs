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
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay
{
    /// <summary>
    /// Base class for all data transforms.
    /// Defines methods to transform point from data coordinate system to viewport coordinates and vice versa.
    /// Should be ????? TODO I FORGOT THIS TERM !!!
    /// </summary>
    public abstract class DataTransform
    {
        /// <summary>
        /// Transforms the point in data coordinates to viewport coordinates.
        /// </summary>
        /// <param name="pt">The point in data coordinates.</param>
        /// <returns></returns>
        public abstract Point DataToViewport(Point pt);
        /// <summary>
        /// Transforms the point in viewport coordinates to data coordinates.
        /// </summary>
        /// <param name="pt">The point in viewport coordinates.</param>
        /// <returns></returns>
        public abstract Point ViewportToData(Point pt);

        private static readonly DataRect defaultDomain = DataRect.Empty;
        /// <summary>
        /// Gets the data domain.
        /// </summary>
        /// <value>The data domain.</value>
        public virtual DataRect DataDomain { get { return defaultDomain; } }
    }

    /// <summary>
    /// Represents identity data transform, which applies no transformation.
    /// is by default in CoordinateTransform.
    /// </summary>
    public sealed class IdentityTransform : DataTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityTransform"/> class.
        /// </summary>
        public IdentityTransform() { }

        /// <summary>
        /// Transforms the point in data coordinates to viewport coordinates.
        /// </summary>
        /// <param name="pt">The point in data coordinates.</param>
        /// <returns></returns>
        public override Point DataToViewport(Point pt)
        {
            return pt;
        }

        /// <summary>
        /// Transforms the point in viewport coordinates to data coordinates.
        /// </summary>
        /// <param name="pt">The point in viewport coordinates.</param>
        /// <returns></returns>
        public override Point ViewportToData(Point pt)
        {
            return pt;
        }
    }

    /// <summary>
    /// Represents a logarithmic transform of x-values of points.
    /// </summary>
    public sealed class Log10XTransform : DataTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Log10XTransform"/> class.
        /// </summary>
        public Log10XTransform() { }

        /// <summary>
        /// Transforms the point in data coordinates to viewport coordinates.
        /// </summary>
        /// <param name="pt">The point in data coordinates.</param>
        /// <returns></returns>
        public override Point DataToViewport(Point pt)
        {
            double x = pt.X;

            if (x < 0)
                x = Double.MinValue;
            else
                x = Math.Log10(x);

            return new Point(x, pt.Y);
        }

        /// <summary>
        /// Transforms the point in viewport coordinates to data coordinates.
        /// </summary>
        /// <param name="pt">The point in viewport coordinates.</param>
        /// <returns></returns>
        public override Point ViewportToData(Point pt)
        {
            return new Point(Math.Pow(10, pt.X), pt.Y);
        }

        private DataRect dataDomain = DataRect.FromPoints(Double.Epsilon, Double.MinValue / 2, Double.MaxValue, Double.MaxValue / 2);
        /// <summary>
        /// Gets the data domain.
        /// </summary>
        /// <value>The data domain.</value>
        public override DataRect DataDomain
        {
            get { return dataDomain; }
        }
    }

    /// <summary>
    /// Represents a mercator transform, used in maps.
    /// Transforms y coordinates.
    /// </summary>
    public sealed class MercatorTransform : DataTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MercatorTransform"/> class.
        /// </summary>
        public MercatorTransform()
        {
            CalcScale(maxLatitude);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MercatorTransform"/> class.
        /// </summary>
        /// <param name="maxLatitude">The maximal latitude.</param>
        public MercatorTransform(double maxLatitude)
        {
            this.maxLatitude = maxLatitude;
            CalcScale(maxLatitude);
        }

        private void CalcScale(double maxLatitude)
        {
            double maxLatDeg = maxLatitude;
            double maxLatRad = maxLatDeg * Math.PI / 180;
            scale = maxLatDeg / Math.Log(Math.Tan(maxLatRad / 2 + Math.PI / 4));
        }

        private double scale;
        /// <summary>
        /// Gets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public double Scale
        {
            get { return scale; }
        }

        private double maxLatitude = 85;
        /// <summary>
        /// Gets the maximal latitude.
        /// </summary>
        /// <value>The max latitude.</value>
        public double MaxLatitude
        {
            get { return maxLatitude; }
        }

        /// <summary>
        /// Transforms the point in data coordinates to viewport coordinates.
        /// </summary>
        /// <param name="pt">The point in data coordinates.</param>
        /// <returns></returns>
        public sealed override Point DataToViewport(Point pt)
        {
            double y = pt.Y;
            if (-maxLatitude <= y && y <= maxLatitude)
            {
                y = scale * Math.Log(Math.Tan(Math.PI * (pt.Y + 90) / 360));
            }

            return new Point(pt.X, y);
        }

        /// <summary>
        /// Transforms the point in viewport coordinates to data coordinates.
        /// </summary>
        /// <param name="pt">The point in viewport coordinates.</param>
        /// <returns></returns>
        public sealed override Point ViewportToData(Point pt)
        {
            double y = pt.Y;
            if (-maxLatitude <= y && y <= maxLatitude)
            {
                double e = Math.Exp(y / scale);
                y = 360 * Math.Atan(e) / Math.PI - 90;
            }

            return new Point(pt.X, y);
        }
    }

    /// <summary>
    /// Represents transform from polar coordinate system to rectangular coordinate system.
    /// </summary>
    public sealed class PolarToRectTransform : DataTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolarToRectTransform"/> class.
        /// </summary>
        public PolarToRectTransform() { }

        /// <summary>
        /// Transforms the point in data coordinates to viewport coordinates.
        /// </summary>
        /// <param name="pt">The point in data coordinates.</param>
        /// <returns></returns>
        public override Point DataToViewport(Point pt)
        {
            double r = pt.X;
            double phi = pt.Y;

            double x = r * Math.Cos(phi);
            double y = r * Math.Sin(phi);

            return new Point(x, y);
        }

        /// <summary>
        /// Transforms the point in viewport coordinates to data coordinates.
        /// </summary>
        /// <param name="pt">The point in viewport coordinates.</param>
        /// <returns></returns>
        public override Point ViewportToData(Point pt)
        {
            double x = pt.X;
            double y = pt.Y;
            double r = Math.Sqrt(x * x + y * y);
            double phi = Math.Atan2(y, x);

            return new Point(r, phi);
        }
    }

    /// <summary>
    /// Represents a data transform which applies rotation around specified center at specified angle.
    /// </summary>
    public sealed class RotateDataTransform : DataTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateDataTransform"/> class.
        /// </summary>
        /// <param name="angleInRadians">The angle in radians.</param>
        public RotateDataTransform(double angleInRadians)
        {
            this.center = new Point();
            this.angle = angleInRadians;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateDataTransform"/> class.
        /// </summary>
        /// <param name="angleInRadians">The angle in radians.</param>
        /// <param name="center">The center of rotation.</param>
        public RotateDataTransform(double angleInRadians, Point center)
        {
            this.center = center;
            this.angle = angleInRadians;
        }

        private readonly Point center;
        /// <summary>
        /// Gets the center of rotation.
        /// </summary>
        /// <value>The center.</value>
        public Point Center
        {
            get { return center; }
        }

        private readonly double angle;
        /// <summary>
        /// Gets the rotation angle.
        /// </summary>
        /// <value>The angle.</value>
        public double Angle
        {
            get { return angle; }
        }

        /// <summary>
        /// Transforms the point in data coordinates to viewport coordinates.
        /// </summary>
        /// <param name="pt">The point in data coordinates.</param>
        /// <returns></returns>
        public override Point DataToViewport(Point pt)
        {
            return Transform(pt, angle);
        }

        /// <summary>
        /// Transforms the point in viewport coordinates to data coordinates.
        /// </summary>
        /// <param name="pt">The point in viewport coordinates.</param>
        /// <returns></returns>
        public override Point ViewportToData(Point pt)
        {
            return Transform(pt, -angle);
        }

        private Point Transform(Point pt, double angle)
        {
            Vector vec = new Vector(pt.X - center.X,pt.Y-center.Y);
            double currAngle = Math.Atan2(vec.Y, vec.X);
            currAngle += angle;

            Vector rotatedVec = new Vector(Math.Cos(currAngle), Math.Sin(currAngle)) * vec.Length;

            return new Point(center.X + rotatedVec.X,center.Y+rotatedVec.Y);
        }
    }

    /// <summary>
    /// Represents data transform performed by multiplication on given matrix.
    /// </summary>
    public sealed class MatrixDataTransform : DataTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixDataTransform"/> class.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        public MatrixDataTransform(Matrix matrix)
        {
            this.matrix = matrix;
            //this.invertedMatrix = matrix;
            //invertedMatrix.Invert();
        }

        private readonly Matrix matrix;
        private Matrix invertedMatrix {
            get {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Transforms the point in data coordinates to viewport coordinates.
        /// </summary>
        /// <param name="pt">The point in data coordinates.</param>
        /// <returns></returns>
        public override Point DataToViewport(Point pt)
        {
            return matrix.Transform(pt);
        }

        /// <summary>
        /// Transforms the point in viewport coordinates to data coordinates.
        /// </summary>
        /// <param name="pt">The point in viewport coordinates.</param>
        /// <returns></returns>
        public override Point ViewportToData(Point pt)
        {
            return invertedMatrix.Transform(pt);
        }
    }
    
    /// <summary>
    /// Represents a data transform, performed by given delegate.
    /// </summary>
    public sealed class DelegateDataTransform : DataTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateDataTransform"/> class.
        /// </summary>
        /// <param name="dataToViewport">The data to viewport transform delegate.</param>
        /// <param name="viewportToData">The viewport to data transform delegate.</param>
        public DelegateDataTransform(Func<Point, Point> dataToViewport, Func<Point, Point> viewportToData)
        {
            if (dataToViewport == null)
                throw new ArgumentNullException("dataToViewport");
            if (viewportToData == null)
                throw new ArgumentNullException("viewportToData");

            this.dataToViewport = dataToViewport;
            this.viewportToData = viewportToData;
        }

        private readonly Func<Point, Point> dataToViewport;
        /// <summary>
        /// Gets the data to viewport transform delegate.
        /// </summary>
        /// <value>The data to viewport func.</value>
        public Func<Point, Point> DataToViewportFunc
        {
            get { return dataToViewport; }
        }

        private readonly Func<Point, Point> viewportToData;
        /// <summary>
        /// Gets the viewport to data transform delegate.
        /// </summary>
        /// <value>The viewport to data func.</value>
        public Func<Point, Point> ViewportToDataFunc
        {
            get { return viewportToData; }
        }

        /// <summary>
        /// Transforms the point in data coordinates to viewport coordinates.
        /// </summary>
        /// <param name="pt">The point in data coordinates.</param>
        /// <returns></returns>
        public override Point DataToViewport(Point pt)
        {
            return dataToViewport(pt);
        }

        /// <summary>
        /// Transforms the point in viewport coordinates to data coordinates.
        /// </summary>
        /// <param name="pt">The point in viewport coordinates.</param>
        /// <returns></returns>
        public override Point ViewportToData(Point pt)
        {
            return viewportToData(pt);
        }
    }

    /// <summary>
    /// Contains default data transforms.
    /// </summary>
    public static class DataTransforms
    {
        private static readonly IdentityTransform identity = new IdentityTransform();
        /// <summary>
        /// Gets the default identity data transform.
        /// </summary>
        /// <value>The identity.</value>
        public static IdentityTransform Identity
        {
            get
            {
                return identity;
            }
        }
    }
}
