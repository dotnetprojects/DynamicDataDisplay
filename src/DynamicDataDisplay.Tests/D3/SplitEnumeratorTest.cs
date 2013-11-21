using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Diagnostics;

namespace DynamicDataDisplay.Tests.D3
{
	[TestClass]
	public class SplitEnumeratorTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void TestMethod()
		{
			var source = Enumerable.Range(0, 200);
			var split = source.Split(10);
			foreach (var item in split)
			{
				Debug.WriteLine("Item");
				Debug.Indent();
				foreach (var subitem in item)
				{
					Debug.WriteLine(subitem);
				}
				Debug.Unindent();
			}
		}
	}
}
