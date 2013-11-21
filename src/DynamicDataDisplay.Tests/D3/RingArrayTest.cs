using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace DynamicDataDisplay.Test.D3
{
	[TestClass]
	public class RingArrayTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void TestRingArray()
		{
			RingArray<int> array = new RingArray<int>(10);
			Assert.IsTrue(array.Count == 0);

			array.Add(0);
			Assert.IsTrue(array.Count == 1);
			Assert.IsTrue(array[0] == 0);

			array.Add(1);
			Assert.IsTrue(array.Count == 2);
			Assert.IsTrue(array[0] == 0);
			Assert.IsTrue(array[1] == 1);

			for (int i = 2; i < 10; i++)
			{
				array.Add(i);
			}

			Assert.IsTrue(array.Count == 10);
			Assert.IsTrue(array[9] == 9);

			array.Add(10);
			Assert.IsTrue(array.Count == 10);
			Assert.IsTrue(array.IndexOf(9) == 8);
			Assert.IsTrue(array.IndexOf(10) == 9);
			Assert.IsTrue(array[9] == 10);
			Assert.IsTrue(array[0] == 1);

			array.Clear();
			(array.Count == 0).AssertIsTrue();
			for (int i = 0; i < 100; i++)
			{
				array.Add(i);
			}

			(array.Count == array.Capacity).AssertIsTrue();
			for (int i = 0; i < array.Capacity; i++)
			{
				(array[i] == i + 90).AssertIsTrue();
			}

			var enumerator = array.GetEnumerator();
			int count = 0;
			while (enumerator.MoveNext())
			{
				(enumerator.Current == (count + 90)).AssertIsTrue();
				count++;
			}

			(count == 10).AssertIsTrue();
		}
	}
}
