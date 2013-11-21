using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class ListGenerator {
		public static IEnumerable<Point> GeneratePoints(int length, Func<int, Point> generator) {
			for (int i = 0; i < length; i++) {
				yield return generator(i);
			}
		}

		public static IEnumerable<Point> GeneratePoints(int length, Func<int, double> x, Func<int, double> y) {
			for (int i = 0; i < length; i++) {
				yield return new Point(x(i), y(i));
			}
		}

		public static IEnumerable<T> Generate<T>(int length, Func<int, T> generator) {
			for (int i = 0; i < length; i++) {
				yield return generator(i);
			}
		}
	}
}
