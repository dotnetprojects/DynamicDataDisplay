using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay
{
	[DebuggerDisplay("Count = {Count}")]
	public sealed class FakePointList : IList<Point> {
		private int first;
		private int last;
		private int count;
		private Point startPoint;
		private bool hasPoints;

		private double leftBound;
		private double rightBound;
		private readonly List<Point> points;

		internal FakePointList(List<Point> points, double left, double right) {
			this.points = points;
			this.leftBound = left;
			this.rightBound = right;

			Calc();
		}

		internal void SetXBorders(double left, double right) {
			this.leftBound = left;
			this.rightBound = right;
			Calc();
		}

		private void Calc() {
			Debug.Assert(leftBound <= rightBound);

			first = points.FindIndex(p => p.X > leftBound);
			if (first > 0)
				first--;

			last = points.FindLastIndex(p => p.X < rightBound);

			if (last < points.Count - 1)
				last++;
			count = last - first;
			hasPoints = first >= 0 && last > 0;

			if (hasPoints) {
				startPoint = points[first];
			}
		}

		public Point StartPoint {
			get { return startPoint; }
		}

		public bool HasPoints {
			get { return hasPoints; }
		}

		#region IList<Point> Members

		public int IndexOf(Point item) {
			throw new NotSupportedException();
		}

		public void Insert(int index, Point item) {
			throw new NotSupportedException();
		}

		public void RemoveAt(int index) {
			throw new NotSupportedException();
		}

		public Point this[int index] {
			get {
				return points[first + 1 + index];
			}
			set {
				throw new NotSupportedException();
			}
		}

		#endregion

		#region ICollection<Point> Members

		public void Add(Point item) {
			throw new NotSupportedException();
		}

		public void Clear() {
			throw new NotSupportedException();
		}

		public bool Contains(Point item) {
			throw new NotSupportedException();
		}

		public void CopyTo(Point[] array, int arrayIndex) {
			throw new NotSupportedException();
		}

		public int Count {
			get { return count; }
		}

		public bool IsReadOnly {
			get { throw new NotSupportedException(); }
		}

		public bool Remove(Point item) {
			throw new NotSupportedException();
		}

		#endregion

		#region IEnumerable<Point> Members

		public IEnumerator<Point> GetEnumerator() {
			for (int i = first + 1; i <= last; i++) {
				yield return points[i];
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion
	}
}
