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
    internal static class Verify
    {
        private const string assertionFailed = "Assertion failed - search for reasons in code, that is calling method, where this assertion is performed";
        [DebuggerStepThrough]
        public static void IsTrue(this bool condition)
        {
            if (!condition)
            {
                throw new ArgumentException(assertionFailed);
            }
        }

        [DebuggerStepThrough]
        public static void IsTrue(this bool condition, string paramName)
        {
            if (!condition)
            {
                throw new ArgumentException(assertionFailed, paramName);
            }
        }

        public static void IsTrueWithMessage(this bool condition, string message)
        {
            if (!condition)
                throw new ArgumentException(message);
        }

        [DebuggerStepThrough]
        public static void AssertNotNull(object obj)
        {
            Verify.IsTrue(obj != null);
        }

        public static void VerifyNotNull(this object obj, string paramName)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }

        public static void VerifyNotNull(this object obj)
        {
            VerifyNotNull(obj != null, "value");
        }

        [DebuggerStepThrough]
        public static void AssertIsNotNaN(this double d)
        {
            Verify.IsTrue(!Double.IsNaN(d));
        }

        [DebuggerStepThrough]
        public static void AssertIsFinite(this double d)
        {
            Verify.IsTrue(!Double.IsInfinity(d) && !(Double.IsNaN(d)));
        }
    }
}
