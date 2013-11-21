using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	internal static class IEnumerableExtensions
	{
		public static bool CountGreaterOrEqual<T>(this IEnumerable<T> enumerable, int count)
		{
			int counter = 0;
			using (var enumerator = enumerable.GetEnumerator())
			{
				while (counter < count && enumerator.MoveNext())
				{
					counter++;
				}
			}

			return counter == count;
		}

		public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int maxCount)
		{
			using (var enumerator = new FixedEnumeratorWrapper<T>(source.GetEnumerator()))
			{
				do
				{
					var enumerable = new FixedEnumerable<T>(enumerator);
					yield return enumerable.Take(maxCount);
				}
				while (enumerator.CanMoveNext);
			}
		}

		private sealed class FixedEnumeratorWrapper<T> : IEnumerator<T>
		{
			private readonly IEnumerator<T> enumerator;

			public FixedEnumeratorWrapper(IEnumerator<T> enumerator)
			{
				this.enumerator = enumerator;
			}

			#region IEnumerator<T> Members

			public T Current
			{
				get { return enumerator.Current; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				//enumerator.Dispose();
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get { throw new NotImplementedException(); }
			}

			private bool canMoveNext = false;
			public bool CanMoveNext
			{
				get { return canMoveNext; }
			}

			public bool MoveNext()
			{
				canMoveNext = enumerator.MoveNext();
				return canMoveNext;
			}

			public void Reset()
			{
				enumerator.Reset();
			}

			#endregion
		}

		private sealed class FixedEnumerable<T> : IEnumerable<T>
		{
			private readonly IEnumerator<T> enumerator;
			public FixedEnumerable(IEnumerator<T> enumerator)
			{
				this.enumerator = enumerator;
			}

			#region IEnumerable<T> Members

			public IEnumerator<T> GetEnumerator()
			{
				return enumerator;
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotSupportedException();
			}

			#endregion
		}
	}
}
