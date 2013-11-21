using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Represents a logarithmic transform of both x- and y-values.
	/// </summary>
	public sealed class Log10Transform : DataTransform
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Log10Transform"/> class.
		/// </summary>
		public Log10Transform() { }

		/// <summary>
		/// Transforms the point in data coordinates to viewport coordinates.
		/// </summary>
		/// <param name="pt">The point in data coordinates.</param>
		/// <returns>
		/// Transformed point in viewport coordinates.
		/// </returns>
		public override Point DataToViewport(Point pt)
		{
			double x = pt.X;
			double y = pt.Y;

			x = x < 0 ? Double.MinValue : Math.Log10(x);
			y = y < 0 ? Double.MinValue : Math.Log10(y);

			return new Point(x, y);
		}

		/// <summary>
		/// Transforms the point in viewport coordinates to data coordinates.
		/// </summary>
		/// <param name="pt">The point in viewport coordinates.</param>
		/// <returns>Transformed point in data coordinates.</returns>
		public override Point ViewportToData(Point pt)
		{
			return new Point(Math.Pow(10, pt.X), Math.Pow(10, pt.Y));
		}

		/// <summary>
		/// Gets the data domain of this dataTransform.
		/// </summary>
		/// <value>The data domain of this dataTransform.</value>
		public override DataRect DataDomain
		{
			get { return DataDomains.XYPositive; }
		}
	}
}
