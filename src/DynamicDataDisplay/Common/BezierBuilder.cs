using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
{
	// todo check a license
	public static class BezierBuilder
	{
		public static IEnumerable<Point> GetBezierPoints(Point[] points)
		{
			if (points == null)
				throw new ArgumentNullException("points");

			Point[] firstControlPoints;
			Point[] secondControlPoints;

			int n = points.Length - 1;
			if (n < 1)
				throw new ArgumentException("At least two knot points required", "points");
			if (n == 1)
			{ // Special case: Bezier curve should be a straight line.
				firstControlPoints = new Point[1];
				// 3P1 = 2P0 + P3
				firstControlPoints[0].X = (2 * points[0].X + points[1].X) / 3;
				firstControlPoints[0].Y = (2 * points[0].Y + points[1].Y) / 3;

				secondControlPoints = new Point[1];
				// P2 = 2P1 – P0
				secondControlPoints[0].X = 2 *
					firstControlPoints[0].X - points[0].X;
				secondControlPoints[0].Y = 2 *
					firstControlPoints[0].Y - points[0].Y;

				return Join(points, firstControlPoints, secondControlPoints);
			}

			// Calculate first Bezier control points
			// Right hand side vector
			double[] rhs = new double[n];

			// Set right hand side X values
			for (int i = 1; i < n - 1; ++i)
				rhs[i] = 4 * points[i].X + 2 * points[i + 1].X;
			rhs[0] = points[0].X + 2 * points[1].X;
			rhs[n - 1] = (8 * points[n - 1].X + points[n].X) / 2.0;
			// Get first control points X-values
			double[] x = GetFirstControlPoints(rhs);

			// Set right hand side Y values
			for (int i = 1; i < n - 1; ++i)
				rhs[i] = 4 * points[i].Y + 2 * points[i + 1].Y;
			rhs[0] = points[0].Y + 2 * points[1].Y;
			rhs[n - 1] = (8 * points[n - 1].Y + points[n].Y) / 2.0;
			// Get first control points Y-values
			double[] y = GetFirstControlPoints(rhs);

			// Fill output arrays.
			firstControlPoints = new Point[n];
			secondControlPoints = new Point[n];
			for (int i = 0; i < n; ++i)
			{
				// First control point
				firstControlPoints[i] = new Point(x[i], y[i]);
				// Second control point
				if (i < n - 1)
					secondControlPoints[i] = new Point(2 * points
						[i + 1].X - x[i + 1], 2 *
						points[i + 1].Y - y[i + 1]);
				else
					secondControlPoints[i] = new Point((points
						[n].X + x[n - 1]) / 2,
						(points[n].Y + y[n - 1]) / 2);
			}

			return Join(points, firstControlPoints, secondControlPoints);
		}

		private static IEnumerable<Point> Join(Point[] points, Point[] firstControlPoints, Point[] secondControlPoints)
		{
			var length = firstControlPoints.Length;
			for (int i = 0; i < length; i++)
			{
				yield return points[i];
				yield return firstControlPoints[i];
				yield return secondControlPoints[i];
			}

			yield return points[length];
		}

		/// <summary>
		/// Solves a tridiagonal system for one of coordinates (x or y)
		/// of first Bezier control points.
		/// </summary>
		/// <param name="rhs">Right hand side vector.</param>
		/// <returns>Solution vector.</returns>
		private static double[] GetFirstControlPoints(double[] rhs)
		{
			int n = rhs.Length;
			double[] x = new double[n]; // Solution vector.
			double[] tmp = new double[n]; // Temp workspace.

			double b = 2.0;
			x[0] = rhs[0] / b;
			for (int i = 1; i < n; i++) // Decomposition and forward substitution.
			{
				tmp[i] = 1 / b;
				b = (i < n - 1 ? 4.0 : 3.5) - tmp[i];
				x[i] = (rhs[i] - x[i - 1]) / b;
			}
			for (int i = 1; i < n; i++)
				x[n - i - 1] -= tmp[n - i] * x[n - i]; // Backsubstitution.

			return x;
		}
	}
}
