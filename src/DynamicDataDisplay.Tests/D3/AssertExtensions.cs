using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicDataDisplay.Test.D3
{
	internal static class AssertExtensions
	{
		public static void AssertIsTrue(this bool expression)
		{
			Assert.IsTrue(expression);
		}
	}
}
