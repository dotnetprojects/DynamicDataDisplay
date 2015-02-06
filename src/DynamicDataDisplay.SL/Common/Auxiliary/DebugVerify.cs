using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

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
                throw new ArgumentException("Assertion failed");
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
