using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class TriangleMath
	{
		public static bool TriangleContains(Point a, Point b, Point c, Point m)
		{
			double a0 = a.X - c.X;
			double a1 = b.X - c.X;
			double a2 = a.Y - c.Y;
			double a3 = b.Y - c.Y;

			if (AreClose(a0 * a3, a1 * a2))
			{
				// determinant is too close to zero => apexes are on one line
				Vector ab = a - b;
				Vector ac = a - c;
				Vector bc = b - c;
				Vector ax = a - m;
				Vector bx = b - m;
				bool res = AreClose(ab.X * ax.Y, ab.Y * ax.X) && !AreClose(ab.LengthSquared, 0) ||
					AreClose(ac.X * ax.Y, ac.Y * ax.X) && !AreClose(ac.LengthSquared, 0) ||
					AreClose(bc.X * bx.Y, bc.Y * bx.X) && !AreClose(bc.LengthSquared, 0);
				return res;
			}
			else
			{
				double b1 = m.X - c.X;
				double b2 = m.Y - c.Y;

				// alpha, beta and gamma - are baricentric coordinates of v 
				// in triangle with apexes a, b and c
				double beta = (b2 / a2 * a0 - b1) / (a3 / a2 * a0 - a1);
				double alpha = (b1 - a1 * beta) / a0;
				double gamma = 1 - beta - alpha;
				return alpha >= 0 && beta >= 0 && gamma >= 0;
			}
		}

		private const double eps = 0.00001;
		private static bool AreClose(double x, double y)
		{
			return Math.Abs(x - y) < eps;
		}

		public static Vector3D GetBaricentricCoordinates(Point a, Point b, Point c, Point m)
		{
			double Sac = GetSquare(a, c, m);
			double Sbc = GetSquare(b, c, m);
			double Sab = GetSquare(a, b, m);

			double sum = (Sab + Sac + Sbc) / 3;

			return new Vector3D(Sbc / sum, Sac / sum, Sab / sum);
		}

		public static double GetSquare(Point a, Point b, Point c)
		{
			double ab = (a - b).Length;
			double ac = (a - c).Length;
			double bc = (b - c).Length;

			double p = 0.5 * (ab + ac + bc); // half of perimeter
			return Math.Sqrt(p * (p - ab) * (p - ac) * (p - bc));
		}
	}
}
