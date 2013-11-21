using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	internal static class DebugVerify
	{
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public static void Is(bool condition)
		{
			if (!condition)
			{
				throw new ArgumentException(Strings.Exceptions.AssertionFailed);
			}
		}

		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public static void IsNotNaN(double d)
		{
			DebugVerify.Is(!Double.IsNaN(d));
		}

		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public static void IsNotNaN(Vector vec)
		{
			DebugVerify.IsNotNaN(vec.X);
			DebugVerify.IsNotNaN(vec.Y);
		}

		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public static void IsNotNaN(Point point)
		{
			DebugVerify.IsNotNaN(point.X);
			DebugVerify.IsNotNaN(point.Y);
		}

		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public static void IsFinite(double d)
		{
			DebugVerify.Is(!Double.IsInfinity(d) && !(Double.IsNaN(d)));
		}

		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public static void IsNotNull(object obj)
		{
			DebugVerify.Is(obj != null);
		}
	}
}
